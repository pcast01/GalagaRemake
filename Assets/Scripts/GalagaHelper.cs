#pragma warning disable 0162 // Switch code warning: undetectable code
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GalagaHelper {


    /// <summary>
    /// Wave Patterns for Round1.
    /// </summary>
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

    // Second wave path
    public static Vector3[] SecondWavePath = new Vector3[11];
    public static Vector3[] FourthWavePath = new Vector3[8];

    // 1st Wave collect and set on 2 waves together
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
#region formation

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
#endregion
    public static void Get2ndwave() 
    {
        // Get formation2 script
        EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
        if (SecondWavePath[0].x == 0f)
        {
            // Reset WaveDelay to zero
            Wave1Delay = 0.0f;
            Vector3[] circleNodes = new Vector3[7];
            circleNodes = iTweenPath.GetPath("LeftCircle");
            Debug.Log("<color=red> path nodes:</color> " + iTweenPath.GetPath("LeftCircle").GetUpperBound(0));
            GameObject spawnPoint = new GameObject();
            spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");

            GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
            //EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
            Vector3[] LeftPath = new Vector3[11];
            LeftPath[0] = spawnPoint.transform.position;
            LeftPath[1] = circleNodes[0];
            LeftPath[2] = circleNodes[1];
            LeftPath[3] = circleNodes[2];
            LeftPath[4] = circleNodes[3];
            LeftPath[5] = circleNodes[4];
            LeftPath[6] = circleNodes[5];
            LeftPath[7] = circleNodes[6];
            LeftPath[8] = middlePt.transform.position;
            LeftPath[9] = form2Spawn.transform.position;
            LeftPath[10] = form2Spawn.currentSpawnPos.position;
            SecondWavePath = LeftPath;
        }
        // Change last position to the formation's last position.
        SecondWavePath[10] = form2Spawn.currentSpawnPos.position;
        Debug.Log("<color=green>Current spawn pos: </color>" + form2Spawn.currentSpawnPos);
    }

    public static void Get3rdwave()
    {
        // Get formation3 script
        EnemySpawner form3Spawn = GameObject.FindGameObjectWithTag("phase31").GetComponent<EnemySpawner>();
        if (SecondWavePath[0].x == 0f)
        {
            // Reset WaveDelay to zero
            Wave1Delay = 0.0f;
            Vector3[] circleNodes = new Vector3[7];
            circleNodes = iTweenPath.GetPath("RightCircle");
            Debug.Log("<color=red> path nodes:</color> " + iTweenPath.GetPath("RightCircle").GetUpperBound(0));
            GameObject spawnPoint = new GameObject();
            spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomRight");

            GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
            //EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
            Vector3[] LeftPath = new Vector3[11];
            LeftPath[0] = spawnPoint.transform.position;
            LeftPath[1] = circleNodes[0];
            LeftPath[2] = circleNodes[1];
            LeftPath[3] = circleNodes[2];
            LeftPath[4] = circleNodes[3];
            LeftPath[5] = circleNodes[4];
            LeftPath[6] = circleNodes[5];
            LeftPath[7] = circleNodes[6];
            LeftPath[8] = middlePt.transform.position;
            LeftPath[9] = form3Spawn.transform.position;
            LeftPath[10] = form3Spawn.currentSpawnPos.position;
            SecondWavePath = LeftPath;
        }
        // Change last position to the formation's last position.
        SecondWavePath[10] = form3Spawn.currentSpawnPos.position;
        Debug.Log("<color=green>Current spawn pos: </color>" + form3Spawn.currentSpawnPos);
    }

    public static void Get4thwave()
    {
        // Get formation3 script
        EnemySpawner form4Spawn = GameObject.FindGameObjectWithTag("phase41").GetComponent<EnemySpawner>();
        if (FourthWavePath[0].x == 0f)
        {
            // Reset WaveDelay to zero
            Wave1Delay = 0.0f;
            Vector3[] circleNodes = new Vector3[7];
            circleNodes = iTweenPath.GetPath("LeftCircle");
            Debug.Log("<color=red> path nodes:</color> " + iTweenPath.GetPath("LeftCircle").GetUpperBound(0));
            GameObject spawnPoint = new GameObject();
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");

            GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
            //EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
            Vector3[] LeftPath = new Vector3[8];
            LeftPath[0] = spawnPoint.transform.position;
            LeftPath[1] = circleNodes[3];
            LeftPath[2] = circleNodes[4];
            LeftPath[3] = circleNodes[5];
            LeftPath[4] = circleNodes[6];
            LeftPath[5] = middlePt.transform.position;
            LeftPath[6] = form4Spawn.transform.position;
            LeftPath[7] = form4Spawn.currentSpawnPos.position;
            FourthWavePath = LeftPath;
        }
        // Change last position to the formation's last position.
        FourthWavePath[7] = form4Spawn.currentSpawnPos.position;
        Debug.Log("<color=green>Current spawn pos: </color>" + form4Spawn.currentSpawnPos);
    }

    public static void Get5thwave()
    {
        // Get formation3 script
        EnemySpawner form5Spawn = GameObject.FindGameObjectWithTag("phase51").GetComponent<EnemySpawner>();
        if (FourthWavePath[0].x == 0f)
        {
            // Reset WaveDelay to zero
            Wave1Delay = 0.0f;
            Vector3[] circleNodes = new Vector3[7];
            circleNodes = iTweenPath.GetPath("RightCircle");
            Debug.Log("<color=red> path nodes:</color> " + iTweenPath.GetPath("RightCircle").GetUpperBound(0));
            GameObject spawnPoint = new GameObject();
            spawnPoint = GameObject.FindGameObjectWithTag("Respawn");

            GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
            //EnemySpawner form2Spawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
            Vector3[] LeftPath = new Vector3[8];
            LeftPath[0] = spawnPoint.transform.position;
            LeftPath[1] = circleNodes[3];
            LeftPath[2] = circleNodes[4];
            LeftPath[3] = circleNodes[5];
            LeftPath[4] = circleNodes[6];
            LeftPath[5] = middlePt.transform.position;
            LeftPath[6] = form5Spawn.transform.position;
            LeftPath[7] = form5Spawn.currentSpawnPos.position;
            FourthWavePath = LeftPath;
        }
        // Change last position to the formation's last position.
        FourthWavePath[7] = form5Spawn.currentSpawnPos.position;
        Debug.Log("<color=green>Current spawn pos: </color>" + form5Spawn.currentSpawnPos);
    }

    public static void CollectEnemyPaths(GameObject gm, Hashtable gmParams)
    {
        enemyObjects.Add(gm);
        //Debug.Log("Added Enemy object");
        EnemyPathParams.Add(gmParams);
        Debug.Log("<color=green>Current Spawn POS - CollectEnemyPaths" + gmParams);
    }

    public static void StartPaths()
    {
        if (NumEnemyObjects() == 8)
        {
            for (int i = 0; i < 8; i++)
            {
                Debug.Log("Last pos:" + GalagaHelper.SecondWavePath[10]);
                iTween.MoveTo(enemyObjects[i], EnemyPathParams[i]);
                //EnemyPathParams[i].Keys
                //Debug.Log("enemy paths: " + EnemyPathParams[i].Values);
                isWaveOneStarted = true;
            }
        }
    }

    public static void RemovePaths()
    {
        isWaveOneStarted = false;
        enemyObjects.Clear();
        EnemyPathParams.Clear();
        Debug.Log("<color=red>ENEMY OBJECTS CLEARED</color>");
    }

    public static void StartRound1()
    {
        if (EnemiesSpawned > 7 && enemyObjects.Count == 8 && isWaveOneStarted == false)
        {
            try
            {
                StartPaths();
                Debug.Log("Start path.");
                RemovePaths();
            }
            catch (System.Exception ex)
            {
                Debug.Log("<color=red>Exception: </color>" + ex.Message + ex.Source + ex.ToString());
                throw ex;
            }
        }
    }

    public static void ClearWavePath()
    {
        for (int i = 0; i < 11; i++)
        {
            SecondWavePath[i] = new Vector3(0, 0, 0);
        }

        for (int i = 0; i < 8; i++)
        {
            FourthWavePath[i] = new Vector3(0, 0, 0);   
        }
    }
}
