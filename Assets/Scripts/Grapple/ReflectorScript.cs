using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReflectorScript : MonoBehaviour
{
    private int health;
    private bool beenShot;
    private GameObject player;
    private PlayerController playerScript;
    [SerializeField] GameObject otherGrab;
    [SerializeField] GameObject otherGrabReflector;
    // Start is called before the first frame update
    private void Start()
    {
        health = 20;
        beenShot = false;
        player = GameObject.FindGameObjectWithTag("Player");
        playerScript = player.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0)
        {
            Debug.Log("Blasted");
            SpawnManager.RemoveObject(gameObject);
            Destroy(gameObject);
        }
    }

    public void DamageObject(int damage)
    {
        health -= damage;
    }

    public void Shot(bool shot)
    {
        beenShot = shot;
    }


    public void OnCollisionEnter2D(Collision2D collision)
    {
        if ((collision.gameObject.tag == "HorizWall" || collision.gameObject.tag == "VerticalWall" || collision.gameObject.tag == "GrabAble" || collision.gameObject.tag == "GrabAbleReflector") && beenShot)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.tag == "Enemy" && beenShot)
        {
            EnemyScript enemy = collision.gameObject.GetComponent<EnemyScript>();
            enemy.DamageEnemy(10);
        }

    }


}
