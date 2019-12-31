using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType { Normal, Scout, Tank, Sniper }

public class EnemyScript : MonoBehaviour
{

    [SerializeField]
    private GameObject bulletPrefab;
    private GameObject goal;
    private Rigidbody2D rb;
    private bool canShoot;

    private int damage;
    private int health;
    private float speed;
    private EnemyType type;
    private static float SHOOT_DELAY;
    private static float TURN_DELAY;


    void setupEnemy()
    {
        switch (type) {
            case EnemyType.Normal:
                damage = 1;
                speed = 50f;
                health = 6;
                SHOOT_DELAY = 1f;
                TURN_DELAY = 3f;
                canShoot = true;
                break;

            case EnemyType.Scout:
                damage = 1;
                speed = 65f;
                health = 2;
                SHOOT_DELAY = 0.5f;
                TURN_DELAY = 2f;
                canShoot = true;
                break;

            case EnemyType.Tank:
                damage = 2;
                speed = 25f;
                health = 10;
                SHOOT_DELAY = 1.5f;
                TURN_DELAY = 4f;
                canShoot = true;
                break;

            case EnemyType.Sniper:
                damage = 3;
                speed = 40f;
                health = 4;
                SHOOT_DELAY = 2.0f;
                TURN_DELAY = 3.0f;
                canShoot = true;
                break;
        } 
    }

    public int GetHealth()
    {
        return health;
    }

    public int GetDamage()
    {
        return damage;
    }

    // Start is called before the first frame update
    void Start()
    {
        goal = GameObject.FindGameObjectWithTag("Player");
        rb = GetComponent<Rigidbody2D>();
    }

    void FixedUpdate()
    {
        // movement
        Vector2 updateGoal = new Vector2(goal.transform.position.x, goal.transform.position.y);
        Vector2 td = updateGoal - new Vector2(transform.position.x, transform.position.y);
        transform.up = td;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(td), EnemyScript.TURN_DELAY * Time.fixedDeltaTime * GameStateManager.GetTimeMulti());
        rb.velocity = transform.forward * speed * GameStateManager.GetTimeMulti();


        // shooting
        if (canShoot)
        {
            canShoot = false;
            GameObject bullet = Instantiate(bulletPrefab, transform.position + transform.forward * 10, transform.rotation);
            EnemyBulletBehavior eb = bullet.GetComponent<EnemyBulletBehavior>();
            eb.SetDamage(this.damage);

           //  Debug.Log("After setting damage, damage is "+eb.GetDamage());

            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.AddForce(bullet.transform.up * 1000);
            
            StartCoroutine(DelayShots());
        }
    }

    void Update()
    {
        if (health <= 0)
        {
            SpawnManager.RemoveEnemy(gameObject);
            Destroy(gameObject);
        }
    }

    private IEnumerator DelayShots()
    {
        yield return new WaitForSeconds(EnemyScript.SHOOT_DELAY);
        canShoot = true;
    }

    public void DamageEnemy(int damage)
    {
        health -= damage;
    }

    public void SetEnemyType(EnemyType t)
    {
        type = t;
        setupEnemy();
    }

    public EnemyType GetEnemyType()
    {
        return type;
    }
}
