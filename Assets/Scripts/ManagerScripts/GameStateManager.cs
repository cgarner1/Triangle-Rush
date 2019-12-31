using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameStateManager : MonoBehaviour
{
	private static float timeMultiplier;

	public static int OBJECT_MAX = 10;
	public static int ENEMY_MAX = 10;
    public static float THRESHOLD = 0.3f;
    private static float DECAY_VALUE = .02f;

    public bool isSlowed;

    private static bool decay;
    private static bool restore;

    // Levels
    private static Stack<EnemyType> level1 = new Stack<EnemyType>();
    private static Stack<EnemyType> level2 = new Stack<EnemyType>();
    private static Stack<EnemyType> level3 = new Stack<EnemyType>();
    private static Stack<EnemyType> level4 = new Stack<EnemyType>();
    private static Stack<EnemyType> level5 = new Stack<EnemyType>();

    private static List<Stack<EnemyType>> levels = new List<Stack<EnemyType>>() { level1, level2, level3, level4, level5 };
    public static int currentLevelIndex;

    private static float maxTimeLevel1 = 5f;
    private static float maxTimeLevel2 = 4f;
    private static float maxTimeLevel3 = 3f;
    private static float maxTimeLevel4 = 2f;
    private static float maxTimeLevel5 = 1f;

    private static List<float> maxLevelTimes = new List<float>() { maxTimeLevel1, maxTimeLevel2, maxTimeLevel2, maxTimeLevel3, maxTimeLevel4, maxTimeLevel5 };


    private static float SpawnEnemyTimer;
    private static float SpawnObjectTimer;
    private static float SpawnEnemyTimerMax;
    private static float SpawnObjectTimerMax;

    private static float SpawnEnemyTimerMin = 1f;
    private static float SpawnObjectTimerMin = 2f;

    public static bool SpawnEnemy;
    public static bool SpawnObject;

    public static bool StartLevel;

    public static bool campaignMode;
    private static bool playing;

    void Start()
	{
		timeMultiplier = 1.0f;
        decay = false;
        restore = false;
        isSlowed = false;
        SpawnEnemyTimer = 0f;
        SpawnObjectTimer = 0f;

        currentLevelIndex = 0;
        SpawnEnemy = false;
        SpawnObject = false;
        playing = false;

        SetupLevels();
    }

    private static void SetupLevels()
    {
        // Level 1
        for (int i=0;i<10;i++) { level1.Push(EnemyType.Normal); }

        // Level 2
        for (int i = 0; i < 10; i++) { level2.Push(EnemyType.Normal); }
        for (int i = 0; i < 3; i++) { level2.Push(EnemyType.Tank); }
        for (int i = 0; i < 2; i++) { level2.Push(EnemyType.Scout); }

        // Level 3
        for (int i = 0; i < 15; i++) { level3.Push(EnemyType.Normal); }
        for (int i = 0; i < 5; i++) { level3.Push(EnemyType.Tank); }
        for (int i = 0; i < 10; i++) { level3.Push(EnemyType.Scout); }

        //Level 4
        for (int i = 0; i < 13; i++) { level4.Push(EnemyType.Normal); }
        for (int i = 0; i < 7; i++) { level4.Push(EnemyType.Tank); }
        for (int i = 0; i < 10; i++) { level4.Push(EnemyType.Scout); }
        for (int i = 0; i < 5; i++) { level4.Push(EnemyType.Sniper); }

        //Level 5
        for (int i = 0; i < 10; i++) { level5.Push(EnemyType.Normal); }
        for (int i = 0; i < 10; i++) { level5.Push(EnemyType.Tank); }
        for (int i = 0; i < 15; i++) { level5.Push(EnemyType.Scout); }
        for (int i = 0; i < 10; i++) { level5.Push(EnemyType.Sniper); }
    }

    public static void SetMode(bool campaign)
    {
        campaignMode = campaign;

        if (campaignMode)
        {
            StartLevel = true;
        } else
        {
            SpawnEnemyTimerMax = 5f;
            SpawnObjectTimerMax = 10f;
        }

        playing = true;

    }

    public static Stack<EnemyType> GetCurrentLevel()
    {
        return levels[currentLevelIndex];
    }

    public static EnemyType GetNextEnemy()
    {
        if (campaignMode)
        {
            return GameStateManager.GetCurrentLevel().Pop();
        }
        else
        {
            return (EnemyType)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(EnemyType)).Length);
        }
    }

    void Update()
    {
        if (playing)
        {
            SpawnEnemyTimer += Time.deltaTime;
            SpawnObjectTimer += Time.deltaTime;

            if (campaignMode)
            {
                if (decay)
                {
                    timeMultiplier -= GameStateManager.DECAY_VALUE;

                    if (timeMultiplier <= GameStateManager.THRESHOLD)
                    {
                        timeMultiplier = GameStateManager.THRESHOLD;
                        decay = false;
                        isSlowed = true;
                    }
                }

                if (restore)
                {
                    timeMultiplier += DECAY_VALUE;

                    if (timeMultiplier >= 1.0f)
                    {
                        timeMultiplier = 1.0f;
                        restore = false;
                        isSlowed = false;
                    }
                }

                if (GameStateManager.GetCurrentLevel().Count == 0)
                {
                    currentLevelIndex++;
                    StartLevel = true;
                }

                if (SpawnEnemyTimer >= maxLevelTimes[currentLevelIndex])
                {
                    SpawnEnemyTimer = 0f;
                    SpawnEnemy |= GameStateManager.GetCurrentLevel().Count > 0;
                    // Debug.Log("enemy timer");
                }
                
                if (SpawnObjectTimer >= maxLevelTimes[currentLevelIndex] * 2)
                {
                    SpawnObjectTimer = 0f;
                    SpawnObject |= GameStateManager.GetCurrentLevel().Count > 0;
                    // Debug.Log("object timer");
                }
            }
            else
            {
                if (decay)
                {
                    timeMultiplier -= GameStateManager.DECAY_VALUE;

                    if (timeMultiplier <= GameStateManager.THRESHOLD)
                    {
                        timeMultiplier = GameStateManager.THRESHOLD;
                        decay = false;
                        isSlowed = true;
                    }
                }

                if (restore)
                {
                    timeMultiplier += DECAY_VALUE;

                    if (timeMultiplier >= 1.0f)
                    {
                        timeMultiplier = 1.0f;
                        restore = false;
                        isSlowed = false;
                    }
                }


                if (SpawnEnemyTimer >= SpawnEnemyTimerMax)
                {
                    if ((0.01f * SpawnEnemyTimerMax) >= SpawnEnemyTimerMin)
                    {
                        SpawnEnemyTimerMax -= 0.01f * SpawnEnemyTimerMax;
                        SpawnEnemy = true;
                        SpawnEnemyTimer = 0f;
                    } else
                    {
                        SpawnEnemyTimerMax = SpawnEnemyTimerMin;
                        SpawnEnemy = true;
                        SpawnEnemyTimer = 0f;
                    }
                }
                if (SpawnObjectTimer >= SpawnObjectTimerMax)
                {
                    if((0.02f * SpawnObjectTimerMax) >= SpawnObjectTimerMin)
                    {
                        SpawnObjectTimerMax -= 0.02f * SpawnObjectTimerMax;
                        SpawnObject = true;
                        SpawnObjectTimer = 0f;
                    } else
                    {
                        SpawnObjectTimerMax = SpawnObjectTimerMin;
                        SpawnObject = true;
                        SpawnObjectTimer = 0f;
                    }
                }
            }
        }
    }


    public void RestartGame()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }


    public bool IsSlowed()
    {
        return isSlowed;
    }

    public static void SetTimeMultiTo(float f)
	{
		timeMultiplier = f;
	}
    
	public static void ReturnToNormalTime()
	{
		timeMultiplier = 1f;
	}

	public static float GetTimeMulti()
	{
		return timeMultiplier;
	}

    public static void BeginMultiDecay()
    {
        decay = true;
    }

    public static void RestoreMultiDecay()
    {
        restore = true;
    }
}
