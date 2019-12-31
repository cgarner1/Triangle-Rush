using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    private static List<GameObject> enemies;
    private static List<GameObject> objects;

    [SerializeField]
    private GameObject enemyPrefab;

    [SerializeField]
    private GameObject[] objectPrefab;

    [SerializeField]
    private GameObject[] powerUps;


    [SerializeField] private PlayerController player;

    public static bool enemyKilled;

    // Start is called before the first frame update
    void Start()
    {
        enemies = new List<GameObject>();
        objects = new List<GameObject>();

        enemyKilled = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (GameStateManager.StartLevel)
        {
            for (int i=0;i<4;i++) {
                SpawnEnemy();
            }
        }

        if (GameStateManager.SpawnEnemy)
        {
            GameStateManager.SpawnEnemy = false;
            SpawnEnemy();
        }

        if (GameStateManager.SpawnObject)
        {
            GameStateManager.SpawnObject = false;
            SpawnObject();
            SpawnPowerup();
        }

        if (GameStateManager.StartLevel)
        {
            InvokeRepeating("SpawnPowerup", 5, 15.0f);
        }
    }

    public static bool enemiesOnScreen()
    {
        return enemies.Count > 0;
    }

    public static void RemoveEnemy(GameObject g)
    {
        enemies.Remove(g);
        enemyKilled = true;
    }

    public static void RemoveObject(GameObject g)
    {
        objects.Remove(g);
    }

    private void SpawnEnemy()
    {
        Vector3 loc;
        do
        {
            loc = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), 0);
        } while ((player.transform.position - loc).magnitude < 15);

        GameObject e = Instantiate(enemyPrefab, loc, Quaternion.identity);
        EnemyScript ens = e.GetComponent<EnemyScript>();
        EnemyType t = GameStateManager.GetNextEnemy();
        ens.SetEnemyType(t);
        enemies.Add(e);
    }

    private void SpawnObject()
    {
        Vector3 loc;
        do
        {
            loc = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), 0);
        } while ((player.transform.position - loc).magnitude < 7);


        int grabIndex = Random.Range(0, 3);
        GameObject o = Instantiate(objectPrefab[grabIndex], loc, Quaternion.identity);
        objects.Add(o);
    }

    private void SpawnPowerup()
    {
        Vector3 spawnLoc = new Vector3(Random.Range(-40, 40), Random.Range(-40, 40), 0);
        int powerUpIdx = Random.Range(0,2);
        
        Instantiate(powerUps[powerUpIdx],spawnLoc, Quaternion.identity);
    }


}
