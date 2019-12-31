using UnityEngine;
using System.Collections;
using System;

public class EnemyBulletBehavior : MonoBehaviour
{
    [SerializeField] GameObject reflectorOutlet;
    [SerializeField] GameObject playerBullet;
    private static float MAX_SPEED = 15f;
    private int damage;
    private Rigidbody2D rb;
    PlayerController player;

    // Start is called before the first frame update
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
      
    }

    // Update is called once per frame
    void Update()
    {
        rb.velocity = MAX_SPEED * GameStateManager.GetTimeMulti() * transform.up;
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }

    // Basically once we figure out all of the object in the game we can see if the bullet hits one of those things, the bullet breaks
    void OnTriggerEnter2D(Collider2D collision)
    {
        // Debug.Log("Hit "+collision.tag+" for " + damage);

        if (collision.gameObject.tag == "Player Bullet")
        {
            // Debug.Log("Player Bullet");
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "GrabAble")
        {
            // Debug.Log("Block");
            GrappleObjectScript obj = collision.gameObject.GetComponent<GrappleObjectScript>();
            obj.DamageObject(damage);
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "GrabAbleReflector")
        {
            // Debug.Log("Block");
            ReflectorScript obj = collision.gameObject.GetComponent<ReflectorScript>();
            obj.DamageObject(damage);
            Destroy(gameObject);
            GameObject bullet = Instantiate(playerBullet, collision.transform.position + collision.transform.forward * 10, collision.transform.rotation);
            //EnemyBulletBehavior eb = bullet.GetComponent<EnemyBulletBehavior>();
            //eb.SetDamage(this.damage);

            //  Debug.Log("After setting damage, damage is "+eb.GetDamage());

            Rigidbody2D bulletBody = bullet.GetComponent<Rigidbody2D>();
            bulletBody.AddForce(bullet.transform.up * -1000);

            //Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Player")
        {

            player = collision.gameObject.GetComponent<PlayerController>();
            if (!player.GetIsInvincible()) {
                PlayerController player = collision.gameObject.GetComponent<PlayerController>();
                player.damagePlayer(damage);
                Destroy(gameObject);
            }
        }
        if (collision.gameObject.tag == "HorizWall" || collision.gameObject.tag == "VerticalWall")
        {
            // Debug.Log("Wall");
            Destroy(gameObject);
        }
    }

    public void SetDamage(int d)
    {
        damage = d;
        // Debug.Log("Set enemy bullet damage to " + damage);
    }

    public int GetDamage()
    {
        return damage;
    }
}


