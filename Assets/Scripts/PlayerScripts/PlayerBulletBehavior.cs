using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBulletBehavior : MonoBehaviour
{
    private Rigidbody2D rb;
    private int damage;
    private static float MAX_SPEED = 20f;

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

        if (collision.gameObject.tag == "Enemy Bullet")
        {
            // Debug.Log("Enemy Bullet");
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "HorizWall" || collision.gameObject.tag == "VerticalWall")
        {
            // Debug.Log("Wall");
            Destroy(gameObject);
        }
        if (collision.gameObject.tag == "Enemy")
        {
            // Debug.Log("Shot");
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
            enemy.DamageEnemy(damage);
           //  Debug.Log("Enemy health left: " + enemy.GetHealth());
        }
    }

    public void SetDamage(int d)
    {
        damage = d;
        // Debug.Log("Set player bullet damage " + damage);
    }

    public int GetDamage()
    {
        return damage;
    }
}
