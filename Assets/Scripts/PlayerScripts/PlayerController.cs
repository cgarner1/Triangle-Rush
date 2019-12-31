using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    /*
     * Controls:
     *  
     *  WASD        --->  Up, Left, Down, Right
     *  Right Mouse ---> Shoot
     *  Left Mouse  ---> Pull/Release Obj
     *  Shift       ---> Start/Stop TimeSlow
     *  Spacebar    ---> If slow, Dash
     *  
     */

    private const float INVINCIBILITY_TIME = .8f;
    private const float DASH_TIME = 0.2f;
    private const float POWERUP_INVINCIBLE_TIME = 7.0f;
    private const float POWERUP_ENERGY_TIME = 5.0f;

    // Dependencies
    [SerializeField] GameObject gameState;
    GameStateManager gameStateManagerScript;
    PlayerStatus playerStatus;

    // Movement values
    [SerializeField] float moveSpeed;
    [SerializeField] float shoveForce;
    private float horizInput;
    private float verticalInput;
    private bool onlySlowEnemies;
    
    // Player status
    private bool isInvincible;
    private bool canAct = true;
    private bool shovable;
    private bool decaying;
    private bool holdingObject;
    private bool isDead;


    //Some public ints for health. I'm assuming the game state manager might have some way to keep track, but I have no idea how it works so ¯\_(ツ)_/¯
    public float maxHealth = 10;
    public float currentHealth;

    bool isFiring = false;
    private int damage = 1;
    private int score = 0;
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private GameObject gunOutletLeft;
    [SerializeField] private GameObject gunOutletRight;
    [SerializeField] private float timeBeforeShoot;
    [SerializeField] float shootCoolDown;
    Rigidbody2D rb;
    SpriteRenderer playerRenderer;
    Color startColor;
    


    void Start()
    {
        moveSpeed = 650f;
        gameStateManagerScript = gameState.GetComponent<GameStateManager>();
        rb = GetComponent<Rigidbody2D>();
        playerStatus = GetComponent<PlayerStatus>();
        currentHealth = maxHealth;
        playerRenderer = GetComponent<SpriteRenderer>();
        startColor = playerRenderer.color;
    }

    public int addAndGetScore(int scoreToAdd)
    {
        return score+=scoreToAdd;
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    void Update()
    {
        isDead = (currentHealth <= 0); // I've alwayds liked brackets for booleans
        if (!isDead){
            if (Input.GetKey(KeyCode.Mouse0))
            {
                isFiring = true;
            }

            PointToMouse();
            bool noMeter = playerStatus.IsOutOfEnergy();
            bool atMaxEnergy = playerStatus.IsAtMaxEnergy();
            if (Input.GetKeyDown(KeyCode.LeftShift) && !decaying && atMaxEnergy)
            {
                GameStateManager.BeginMultiDecay();
                decaying = true;
            } else if (noMeter && decaying)
            {
                GameStateManager.RestoreMultiDecay();
                decaying = false;
            }


            shovable = (GameStateManager.GetTimeMulti() <= GameStateManager.THRESHOLD && !noMeter);


            //Making sure the player health can't go above the max if they collected too many health ups or something
            if (currentHealth > maxHealth)
            {
                currentHealth = maxHealth;
            }
        }
        else
        {
            //this.gameObject.SetActive(false);
            rb.velocity = new Vector2(0,0);
            GetComponent<SpriteRenderer>().color = new Color(1f,1f,1f,0.0f);
            SpriteRenderer[]childrenRenderers=  GetComponentsInChildren<SpriteRenderer>();
            foreach(SpriteRenderer renderer in childrenRenderers)
            {
                renderer.color = new Color(1f, 1f, 1f, 0.0f);
            }
        }

        if (SpawnManager.enemyKilled)
        {
            score++;
            SpawnManager.enemyKilled = false;
        }

    }

    public void FixedUpdate()
    {
        if (!isDead)
        {
            // only allow player push given time is completley slowed
            if (Input.GetKeyDown(KeyCode.Space) && shovable)
            {
                playerStatus.SetEnergyToMin();
                StartCoroutine(ShovePlayer());

            }
            else if (canAct)
            {
                MovePlayer();
            }

            timeBeforeShoot += 1f;
            if (isFiring && (timeBeforeShoot > shootCoolDown))
            {
                isFiring = false;
                GameObject bulletLeft = Instantiate(bulletPrefab, gunOutletLeft.transform.position, gunOutletLeft.transform.rotation);
                GameObject bulletRight = Instantiate(bulletPrefab, gunOutletRight.transform.position, gunOutletRight.transform.rotation);

                PlayerBulletBehavior p = bulletLeft.GetComponent<PlayerBulletBehavior>();
                p.SetDamage(this.damage);
                PlayerBulletBehavior p2 = bulletRight.GetComponent<PlayerBulletBehavior>();
                p2.SetDamage(this.damage);

                // Debug.Log("After setting damage, damage is "+p.GetDamage());

                Rigidbody2D bulletBodyLeft = bulletLeft.GetComponent<Rigidbody2D>();
                Rigidbody2D bulletBodyRight = bulletRight.GetComponent<Rigidbody2D>();
                bulletBodyLeft.AddForce(bulletLeft.transform.up * 1000);
                bulletBodyRight.AddForce(bulletRight.transform.up * 1000);
                timeBeforeShoot = 0f;
            }
        }
        
    }

   
    public bool GetDecaying()
    {
        return decaying;
    }


    private void PointToMouse()
    {
        // force player to look towards mouse at all times
        Vector3 mousePos = Input.mousePosition;
        mousePos = Camera.main.ScreenToWorldPoint(mousePos);
        Vector2 direction = new Vector2(mousePos.x - transform.position.x,
                                        mousePos.y - transform.position.y);
        transform.up = direction;
    }


    private void MovePlayer()
    {
        float currentTimeMulti =1;
        if (!onlySlowEnemies)
        {
            currentTimeMulti = GameStateManager.GetTimeMulti();
        }
        
        horizInput = Input.GetAxis("Horizontal");
        verticalInput = Input.GetAxis("Vertical");
        float horizMovement = horizInput * moveSpeed * Time.fixedDeltaTime;
        float verticalMovement = verticalInput * moveSpeed * Time.fixedDeltaTime;
        rb.velocity = new Vector2(horizMovement* currentTimeMulti,
                                  verticalMovement* currentTimeMulti);
    }


    private IEnumerator ShovePlayer()
    {
        rb.velocity = (transform.up * shoveForce * Time.deltaTime);
        canAct = false;
        isInvincible = true;
        yield return new WaitForSeconds(DASH_TIME);
        canAct = true;
        yield return new WaitForSeconds(INVINCIBILITY_TIME);
        isInvincible = false;
    }

    private IEnumerator TriggerInvincibility()
    {
        isInvincible = true;
        yield return new WaitForSeconds(INVINCIBILITY_TIME);
        isInvincible = false;
        CancelInvoke();
        playerRenderer.color = startColor; 
    }

    public void damagePlayer(int damage)
    {
        currentHealth -= damage;
        StartCoroutine(TriggerInvincibility());
        InvokeRepeating("ColorSwap", 0, 0.1f);
    }

    private void ColorSwap()
    {
        if (playerRenderer.color == startColor)
        {
            playerRenderer.color = new Color(255,0,0);
        }
        else
        {
            playerRenderer.color = startColor;
        }

    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if(other.gameObject.tag == "InvincibilityPower")
        {
            
            StartCoroutine("MakeInvincible");
            Destroy(other.gameObject);
        }
        if (other.gameObject.tag == "SlowdownPower")
        {
            StartCoroutine("SlowDownEnemies");
            Destroy(other.gameObject);

        }
    }

    private IEnumerator MakeInvincible()
    {
        this.isInvincible = true;
        playerRenderer.color = new Color(0,255,0);
        yield return new WaitForSeconds(POWERUP_INVINCIBLE_TIME);
        this.isInvincible = false;
        playerRenderer.color = startColor;
    }

    private IEnumerator SlowDownEnemies()
    {
        onlySlowEnemies = true;
        GameStateManager.BeginMultiDecay();
        yield return new WaitForSeconds(POWERUP_ENERGY_TIME);
        GameStateManager.RestoreMultiDecay();
        onlySlowEnemies = false;
    }


    public bool GetIsDead()
    {
        return isDead;
    }

    public bool GetIsInvincible()
    {
        return isInvincible;
    }

}
