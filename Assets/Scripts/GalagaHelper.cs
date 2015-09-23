#pragma warning disable 0162 // Switch code warning: undetectable code
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GalagaHelper {

    public enum EntranceFlightPatterns
    {
        round1_DownLeft,
        round1_DownRight,
        round1_LeftUp,
        round1_RightUp,
        round1_TopLeft,
        round1_TopRight,
        round1_Left,
        round1_Right
    }

    /// <summary>
    /// Formations for Level1 which contain 8 positions each.
    /// </summary>
    public enum Formations
    {
        Round1Phase1,
        Round1Phase2,
        Round1Phase3,
        Round1Phase4,
        Round1Phase5
    }

    public static List<GameObject> enemyObjects = new List<GameObject>();
    public static List<Hashtable> EnemyPathParams = new List<Hashtable>();

    public static bool isWaveOneStarted;

    public static int NumEnemyObjects()
    {
        return enemyObjects.Count;
    }

    public static float Wave1Delay = 0.0f;

    /// <summary>
    /// Total number of enemies spawned thus far.
    /// </summary>
    public static int EnemiesSpawned;

    /// <summary>
    /// Gets the current wave of enemy.
    /// </summary>
    public static Formations CurrentRoundPhase = Formations.Round1Phase1;

    /// <summary>
    /// Gets the Beginning Spawn point of entry for enemy entrance flight.
    /// </summary>
    /// <param name="formationName"></param>
    /// <returns></returns>
    public static Transform RespawnPoint(string formationName, bool spawnEntranceRight)
    {
        GameObject spawnPoint = new GameObject();
        if (formationName == "Round1Phase1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
            //if (spawnEntranceRight)
            //{
            //    spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x - 10, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
            //}
            //else
            //{
            //    spawnPoint.transform.position = new Vector3(spawnPoint.transform.position.x + 10, spawnPoint.transform.position.y, spawnPoint.transform.position.z);
            //}
        }
        else if (formationName == "Round1Phase2EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");
        }
        else if (formationName == "Round1Phase3_1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomRight");
        }
        else if (formationName == "Round1Phase4_1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        }
        else if (formationName == "Round1Phase5_1EnemyFormation")
        {
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
        }
        return spawnPoint.transform; 
    }

    /// <summary>
    /// Gets an array of type Transform returning the flight path used by the enemy.
    /// </summary>
    /// <param name="pattern"></param>
    /// <returns></returns>
    public static Transform[] EntrancePatterns(EntranceFlightPatterns pattern)
    {
        EnemySpawner spawner = GameObject.Find("Round1Phase1EnemyFormation").GetComponent<EnemySpawner>();
        GameObject form2 = GameObject.FindGameObjectWithTag("phase2").gameObject;
        GameObject form31 = GameObject.FindGameObjectWithTag("phase31").gameObject;
        GameObject spawnPt = GameObject.FindGameObjectWithTag("Respawn");
        GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
        GameObject rightEntrancePt = GameObject.FindGameObjectWithTag("begin_Right");
        GameObject leftEntrancePt = GameObject.FindGameObjectWithTag("begin_Left");
        GameObject bottomLeftPt = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");
        GameObject bottomRightPt = GameObject.FindGameObjectWithTag("Spawn_BottomRight");

        // Get next spawn position from spawner script on current formation.
        Transform nextSpawnPos = spawner.currentSpawnPos;
        if (EnemiesSpawned > 8 && EnemiesSpawned < 17) // Round 2
        {
            EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
            nextSpawnPos = form2Spawn.currentSpawnPos;
        }
        else if(EnemiesSpawned > 16 && EnemiesSpawned < 25) // Round 3_1
        {
            EnemySpawner form31Spawn = GameObject.FindGameObjectWithTag("phase31").GetComponent<EnemySpawner>();
            nextSpawnPos = form31Spawn.currentSpawnPos;
        }
        else if (EnemiesSpawned > 24 && EnemiesSpawned < 33) // Round 4_1
        {
            EnemySpawner form41Spawn = GameObject.FindGameObjectWithTag("phase41").GetComponent<EnemySpawner>();
            nextSpawnPos = form41Spawn.currentSpawnPos;
        }
        else if (EnemiesSpawned > 32 && EnemiesSpawned < 41) // Round 5_1
        {
            EnemySpawner form51Spawn = GameObject.FindGameObjectWithTag("phase51").GetComponent<EnemySpawner>();
            nextSpawnPos = form51Spawn.currentSpawnPos;
        }

        switch (pattern)
        {
            case EntranceFlightPatterns.round1_DownLeft:
                Transform[] rd1_Flight1 = new Transform[5];
                rd1_Flight1[0] = RespawnPoint("Round1Phase1EnemyFormation",true);
                Debug.Log("<color=green>SpawnPoint:</color> " + spawnPt.transform);
                rd1_Flight1[1] = middlePt.transform;
                rd1_Flight1[2] = leftEntrancePt.transform;
                rd1_Flight1[3] = spawner.gameObject.transform;
                rd1_Flight1[4] = nextSpawnPos;
                return rd1_Flight1;
                break;
            case EntranceFlightPatterns.round1_DownRight:
                Transform[] rd1_Flight2 = new Transform[5];
                rd1_Flight2[0] = RespawnPoint("Round1Phase1EnemyFormation", false);
                rd1_Flight2[1] = middlePt.transform;
                rd1_Flight2[2] = rightEntrancePt.transform;
                rd1_Flight2[3] = spawner.gameObject.transform;
                rd1_Flight2[4] = nextSpawnPos;
                return rd1_Flight2;
                break;
            case EntranceFlightPatterns.round1_Left:
                Transform[] rd1_Flight3 = new Transform[5];
                rd1_Flight3[0] = bottomLeftPt.transform;
                rd1_Flight3[1] = leftEntrancePt.transform;
                rd1_Flight3[2] = middlePt.transform;
                rd1_Flight3[3] = form2.transform;
                rd1_Flight3[4] = nextSpawnPos;
                return rd1_Flight3;
                break;
            case EntranceFlightPatterns.round1_Right:
                Transform[] rd_Flight4 = new Transform[5];
                rd_Flight4[0] = bottomRightPt.transform;
                rd_Flight4[1] = rightEntrancePt.transform;
                rd_Flight4[2] = middlePt.transform;
                rd_Flight4[3] = form31.transform;
                rd_Flight4[4] = nextSpawnPos;
                return rd_Flight4;
                break;
            case EntranceFlightPatterns.round1_TopLeft:
                Transform[] rd1_Flight5 = new Transform[5];
                rd1_Flight5[0] = spawnPt.transform;
                rd1_Flight5[1] = middlePt.transform;
                rd1_Flight5[2] = bottomLeftPt.transform;
                rd1_Flight5[3] = leftEntrancePt.transform;
                rd1_Flight5[4] = nextSpawnPos;
                return rd1_Flight5;
                break;
            case EntranceFlightPatterns.round1_TopRight:
                Transform[] rd1_Flight6 = new Transform[5];
                rd1_Flight6[0] = spawnPt.transform;
                rd1_Flight6[1] = middlePt.transform;
                rd1_Flight6[2] = bottomRightPt.transform;
                rd1_Flight6[3] = rightEntrancePt.transform;
                rd1_Flight6[4] = nextSpawnPos;
                return rd1_Flight6;
                break;
            default:
                Transform[] test = new Transform[2];
                test[0] = spawnPt.transform;
                test[1] = middlePt.transform;
                return test;
                break;
        }
    }

    public static void CollectEnemyPaths(GameObject gm, Hashtable gmParams)
    {
        enemyObjects.Add(gm);
        Debug.Log("Added Enemy object");
        EnemyPathParams.Add(gmParams);
    }

    public static void StartPaths()
    {
        if (NumEnemyObjects() == 8)
        {
            for (int i = 0; i < 9; i++)
            {
                iTween.MoveTo(enemyObjects[i], EnemyPathParams[i]);
                isWaveOneStarted = true;
            }
            RemovePaths();
        }
    }

    private static void RemovePaths()
    {
        enemyObjects.Clear();
        EnemyPathParams.Clear();
    }

    public static void StartRound1()
    {
        if (EnemiesSpawned == 8 && enemyObjects.Count == 8 && isWaveOneStarted == false)
        {
            StartPaths();
            Debug.Log("Start path.");
        }
    }
}
