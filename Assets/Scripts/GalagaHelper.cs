#pragma warning disable 0162 // Switch code warning: undetectable code
using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public static class GalagaHelper
{
    #region Enum variables
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
        Round1Phase1 = 1,
        Round1Phase2 = 2,
        Round1Phase3 = 3,
        Round1Phase4 = 4,
        Round1Phase5 = 5
    }
    #endregion

    #region Variables
    // Second wave path
    public static Vector3[] SecondWavePath = new Vector3[11];
    public static Vector3[] FourthWavePath = new Vector3[8];

    // 1st Wave collect and set on 2 waves together
    public static List<GameObject> enemyObjects = new List<GameObject>();
    public static List<GameObject> scorpionObjects = new List<GameObject>();
    public static List<Hashtable> EnemyPathParams = new List<Hashtable>();
    public static List<Hashtable> ScorpionPathParams = new List<Hashtable>();
    public static Quaternion enemyFourOrigRotation;
    public static bool isScorpionAttackOn = false;
    public static bool isTractorBeamOn = false;
    public static int numOfPlayers=3;
    public static bool isPlayerCaptured;
    public static int numOfPlayersCaptured;
    public static bool isWaveOneStarted;
    public static float Wave1Delay = 0.0f;
    public static int RoundNumber;
    public static int NumEnemyObjects()
    {
        return enemyObjects.Count;
    }
    public static int NumScorpionObjects()
    {
        return scorpionObjects.Count;
    }

    /// <summary>
    /// Total number of enemies spawned thus far.
    /// </summary>
    public static int EnemiesSpawned;
    public static int EnemiesKilled;
    public static int DisabledEnemies;
    public static int JustSpawned;
    /// <summary>
    /// Gets the current wave of enemy.
    /// </summary>
    public static Formations CurrentRoundPhase = Formations.Round1Phase1;

    // Timer testing
    public static float TimeToSpawn; //Time started
    public static float StartTime;
    public static float TimeDone;
    // Player Icons
    public static GameObject[] PlayerIcons;
    // Enemy2 Random / Enemy2 Path ends
    public static int Enemy2Random;
    public static Vector3 Enemy2PathEnd;
    public static Transform Enemy2LookAtTransform;

    //Function to get random number
    private static readonly System.Random random = new System.Random();
    private static readonly object syncLock = new object();
    public static int RandomNumber(int min, int max)
    {
        lock (syncLock)
        { // synchronize
            return random.Next(min, max);
        }
    }
    #endregion

    #region Enemy 2 Path Ends
    public static void SetEnemy2Random()
    {
        if (Enemy2Random == 0)
        {
            Enemy2Random = GalagaHelper.RandomNumber(1, 3);
        }
    }

    public static Vector3 Enemy2PathDirection()
    {
        Vector3 targetPosition;
        //int endSpawnRandom = GalagaHelper.RandomNumber(0, 3);
        if (Enemy2Random == 1)
        {
            targetPosition = GameObject.FindGameObjectWithTag("EnemyEndLeft").GetComponent<Transform>().position;
        }
        else if (Enemy2Random == 2)
        {
            targetPosition = GameObject.FindGameObjectWithTag("EnemyEndRight").GetComponent<Transform>().position;
        }
        else
        {
            targetPosition = GameObject.Find("EnemyProjectileWall").GetComponent<Transform>().position;
        }
        return targetPosition;
    }

    public static void Enemy2LookAt()
    {
        if (Enemy2Random == 1)
        {
            Enemy2LookAtTransform = GameObject.FindGameObjectWithTag("EnemyEndLeft").transform;
        }
        else if (Enemy2Random == 2)
        {
            Enemy2LookAtTransform = GameObject.FindGameObjectWithTag("EnemyEndRight").transform;
        }
        else
        {
            Enemy2LookAtTransform = GameObject.Find("EnemyProjectileWall").GetComponent<Transform>();
        }
    }
    #endregion 

    #region Waves 3 thru Waves 5 Functions

    /// <summary>
    /// Disable all formations except 1st formation.
    /// </summary>
    public static void ResetFormations()
    {
        EnemySpawner form = GetFormationScript(GalagaHelper.Formations.Round1Phase1);
        form.DisownChildren();
        form.isFormationUp = false;
        form.isFull = false;

        form = GetFormationScript(GalagaHelper.Formations.Round1Phase2);
        form.DisownChildren();
        form.enabled = false;
        form.isFormationUp = false;
        form.isFull = false;

        form = GetFormationScript(GalagaHelper.Formations.Round1Phase3);
        form.DisownChildren();
        form.enabled = false;
        form.isFormationUp = false;
        form.isFull = false;

        form = GetFormationScript(GalagaHelper.Formations.Round1Phase4);
        form.DisownChildren();
        form.enabled = false;
        form.isFormationUp = false;
        form.isFull = false;

        form = GetFormationScript(GalagaHelper.Formations.Round1Phase5);
        form.DisownChildren();
        form.enabled = false;
        form.isFormationUp = false;
        form.isFull = false;
        GalagaHelper.TimeToSpawn = 0;

        // Reset all Occupied bools in Positions
        foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.GetComponent<Position>())
            {
                obj.GetComponent<Position>().isOccupied = false;
            }
        }
    }

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
        GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");
        GameObject rightEntrancePt = GameObject.FindGameObjectWithTag("begin_Right");
        GameObject leftEntrancePt = GameObject.FindGameObjectWithTag("begin_Left");

        // Get next spawn position from spawner script on current formation.
        Transform nextSpawnPos = spawner.currentSpawnPos;

        switch (pattern)
        {
            case EntranceFlightPatterns.round1_DownLeft:
                Transform[] rd1_Flight1 = new Transform[5];
                rd1_Flight1[0] = RespawnPoint("Round1Phase1EnemyFormation",true);
                //Debug.Log("<color=green>SpawnPoint:</color> " + spawnPt.transform);
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
            default:
                Transform[] test = new Transform[2];
                //test[0] = spawnPt.transform;
                //test[1] = middlePt.transform;
                return test;
                break;
        }
    }

    /// <summary>
    /// Gets EnemySpawner Formation Script object for positioning each enemy.
    /// </summary>
    /// <param name="form"></param>
    /// <returns></returns>
    public static EnemySpawner GetFormationScript(Formations form)
    {
        EnemySpawner formSpawn = GameObject.FindGameObjectWithTag("phase1").GetComponent<EnemySpawner>();
        switch (form)
        {
            case Formations.Round1Phase1:
                //formSpawn = GameObject.FindGameObjectWithTag("phase1").GetComponent<EnemySpawner>();
                break;
            case Formations.Round1Phase2:
                formSpawn = GameObject.FindGameObjectWithTag("phase2").GetComponent<EnemySpawner>();
                break;
            case Formations.Round1Phase3:
                formSpawn = GameObject.FindGameObjectWithTag("phase31").GetComponent<EnemySpawner>();
                break;
            case Formations.Round1Phase4:
                formSpawn = GameObject.FindGameObjectWithTag("phase41").GetComponent<EnemySpawner>();
                break;
            case Formations.Round1Phase5:
                formSpawn = GameObject.FindGameObjectWithTag("phase51").GetComponent<EnemySpawner>();
                break;
        }
        return formSpawn;
    }
    
    /// <summary>
    /// Creates the paths for waves 2 - 5.
    /// </summary>
    /// <param name="form"></param>
    public static void GetWavePaths(Formations form, int RoundNumber)
    {
        Vector3[] path = null;
        // Get formation script
        Debug.Log("Current Spawn: ".Bold().Colored(Colors.green) + CurrentRoundPhase + " Wave #:".Bold() + form);
        //Debug.Log("GetWavePaths Round#: ".Bold().Colored(Colors.green) + GalagaHelper.RoundNumber);
        EnemySpawner formSpawn = GetFormationScript(form);
        // FourthWavePath=8 is used for waves 4 & 5.
        // SecondWavePath=11 is used for waves 2 & 3.
        // Check to see if paths are clear before putting path in.
        if (FourthWavePath[0].x == 0f)
        {
            // Reset WaveDelay to zero
            Wave1Delay = 0.0f;
            Vector3[] circleNodes = new Vector3[7];
            GameObject middlePt = GameObject.FindGameObjectWithTag("Point2");

            //Even number traits Left Circle, Odd number Right Circle
            if ((int)form % 2 == 0)
            {
                // Get Right circle
                circleNodes = iTweenPath.GetPath("LeftCircle");
                //Debug.Log("<color=red> path nodes:</color> " + iTweenPath.GetPath("LeftCircle").GetUpperBound(0));
            }
            else
            {
                // Get Right circle
                circleNodes = iTweenPath.GetPath("RightCircle");
                //Debug.Log("<color=red> path nodes:</color> " + iTweenPath.GetPath("RightCircle").GetUpperBound(0));
            }

            // Spawning and Positions
            // Wave # > 3 == Respawn && 8 positions, Wave == 3 == spawn_bottomRight, Wave == 2 = spawn_bottomleft
            if ((int)form > 3)
            {
                // Spawn point
                GameObject spawnPoint = new GameObject();
                spawnPoint = GameObject.FindGameObjectWithTag("Respawn");
                if (RoundNumber == 2 && formSpawn.spawnEntranceRight)
                {
                    Vector3 offset = new Vector3(20, 0, 0);
                    // Path Positions
                    path = new Vector3[8];
                    path[0] = spawnPoint.transform.position;
                    path[1] = circleNodes[3] + offset;
                    path[2] = circleNodes[4] + offset;
                    path[3] = circleNodes[5] + offset;
                    path[4] = circleNodes[6] + offset;
                    path[5] = middlePt.transform.position + offset;
                    path[6] = formSpawn.transform.position + offset;
                    path[7] = formSpawn.currentSpawnPos.position;
                }
                else 
                {
                    // Path Positions
                    path = new Vector3[8];
                    path[0] = spawnPoint.transform.position;
                    path[1] = circleNodes[3];
                    path[2] = circleNodes[4];
                    path[3] = circleNodes[5];
                    path[4] = circleNodes[6];
                    path[5] = middlePt.transform.position;
                    path[6] = formSpawn.transform.position;
                    path[7] = formSpawn.currentSpawnPos.position;
                }
            }
            if ((int)form == 3)
            {
                // Spawn point
                GameObject spawnPoint = new GameObject();
                spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomRight");
                if (RoundNumber == 2 && formSpawn.spawnEntranceRight)
                {
                    Vector3 offset = new Vector3(20, 0, 0);
                    // Path Positions
                    path = new Vector3[11];
                    path[0] = spawnPoint.transform.position;
                    path[1] = circleNodes[0] + offset;
                    path[2] = circleNodes[1] + offset;
                    path[3] = circleNodes[2] + offset;
                    path[4] = circleNodes[3] + offset;
                    path[5] = circleNodes[4] + offset;
                    path[6] = circleNodes[5] + offset;
                    path[7] = circleNodes[6] + offset;
                    path[8] = middlePt.transform.position + offset;
                    path[9] = formSpawn.transform.position + offset;
                    path[10] = formSpawn.currentSpawnPos.position;
                }
                else
                {
                    // Path Positions
                    path = new Vector3[11];
                    path[0] = spawnPoint.transform.position;
                    path[1] = circleNodes[0];
                    path[2] = circleNodes[1];
                    path[3] = circleNodes[2];
                    path[4] = circleNodes[3];
                    path[5] = circleNodes[4];
                    path[6] = circleNodes[5];
                    path[7] = circleNodes[6];
                    path[8] = middlePt.transform.position;
                    path[9] = formSpawn.transform.position;
                    path[10] = formSpawn.currentSpawnPos.position;
                    //Debug.Log(formSpawn.currentSpawnPos.position.ToString().Bold());
                }
            }
            else if ((int)form == 2)
	        {
                // Spawn Point
                GameObject spawnPoint = new GameObject();
                spawnPoint = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");

                if (RoundNumber == 2 && formSpawn.spawnEntranceRight)
                {
                    // Path positions slight right
                    Vector3 offset = new Vector3(20, 0, 0);
                    path = new Vector3[11];
                    path[0] = spawnPoint.transform.position;
                    path[1] = circleNodes[0] + offset;
                    path[2] = circleNodes[1] + offset;
                    path[3] = circleNodes[2] + offset;
                    path[4] = circleNodes[3] + offset;
                    path[5] = circleNodes[4] + offset;
                    path[6] = circleNodes[5] + offset;
                    path[7] = circleNodes[6] + offset;
                    path[8] = middlePt.transform.position + offset;
                    path[9] = formSpawn.transform.position + offset;
                    path[10] = formSpawn.currentSpawnPos.position;
                }
                else
                {
                    // Path positions
                    path = new Vector3[11];
                    path[0] = spawnPoint.transform.position;
                    path[1] = circleNodes[0];
                    path[2] = circleNodes[1];
                    path[3] = circleNodes[2];
                    path[4] = circleNodes[3];
                    path[5] = circleNodes[4];
                    path[6] = circleNodes[5];
                    path[7] = circleNodes[6];
                    path[8] = middlePt.transform.position;
                    path[9] = formSpawn.transform.position;
                    path[10] = formSpawn.currentSpawnPos.position;
                }
	        }

            // SecondWavePath == 11
            // FourthWavePath == 8
            if (path.Length == 8)
            {
                FourthWavePath = path;
            }
            else
	        {
                SecondWavePath = path;
	        }
        }

        // Change last position to the formation's last position.
        if (path.Length == 8)
        {
            FourthWavePath[7] = formSpawn.currentSpawnPos.position;
            Debug.Log(formSpawn.currentSpawnPos.position.ToString().Bold());
        }
        else
	    {
            SecondWavePath[10] = formSpawn.currentSpawnPos.position;
            Debug.Log(formSpawn.currentSpawnPos.position.ToString().Bold());
	    }
    }
    #endregion

    #region FirstAndSecondWaves
    /// <summary>
    /// Stores the game object and the Path Parameters for the iTween paths.
    /// </summary>
    /// <param name="gm"></param>
    /// <param name="gmParams"></param>
    public static void CollectEnemyPaths(GameObject gm, Hashtable gmParams)
    {
        enemyObjects.Add(gm);
        //Debug.Log("Added Enemy object");
        EnemyPathParams.Add(gmParams);
        //Debug.Log("<color=green>Current Spawn POS - CollectEnemyPaths</color>" + gmParams);
    }

    public static void CollectScorpionPaths(GameObject gm, Hashtable gmParams)
    {
        scorpionObjects.Add(gm);
        ScorpionPathParams.Add(gmParams);
    }

    public static void RemoveScorpionPaths()
    {
        scorpionObjects.Clear();
        ScorpionPathParams.Clear();
    }

    /// <summary>
    /// Sets the first 2 waves in motion to start the game.
    /// </summary>
    public static void StartPaths()
    {
        if (NumEnemyObjects() == 8)
        {
            for (int i = 0; i < 8; i++)
            {
                //Debug.Log("Last pos:" + GalagaHelper.SecondWavePath[10]);
                iTween.MoveTo(enemyObjects[i], EnemyPathParams[i]);
                //Debug.Log("enemy paths: " + EnemyPathParams[i].Values);
                isWaveOneStarted = true;
            }
        }
    }

    /// <summary>
    /// Clears out both Enemy object list and Enemy Path Params list.
    /// </summary>
    public static void RemovePaths()
    {
        isWaveOneStarted = false;
        enemyObjects.Clear();
        EnemyPathParams.Clear();
        Debug.Log("<color=red>ENEMY OBJECTS CLEARED</color>");
        CurrentRoundPhase += 1;
    }

    /// <summary>
    /// Starts Round 1 Wave 1
    /// </summary>
    public static void StartRound1()
    {
        if (enemyObjects.Count == 8 && isWaveOneStarted == false)
        {
            try
            {
                //GameObject.Find("PlayerText").SetActive(false);
                //GameObject.Find("RoundTitle").SetActive(false);
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

    public static void StartScorpionPaths()
    {
        if (NumScorpionObjects() == 3)
        {
            for (int i = 0; i < 3; i++)
            {
                //Debug.Log("Last pos:" + GalagaHelper.SecondWavePath[10]);
                iTween.MoveTo(scorpionObjects[i], ScorpionPathParams[i]);
                //Debug.Log("enemy paths: " + EnemyPathParams[i].Values);
                //isWaveOneStarted = true;
            }
        }
    }

    /// <summary>
    /// Clears the static wave paths.
    /// </summary>
    public static void ClearWavePath()
    {
        for (int i = 0; i < 11; i++)
        {
            SecondWavePath[i] = new Vector3(0, 0, 0); // might need 0f
        }

        for (int i = 0; i < 8; i++)
        {
            FourthWavePath[i] = new Vector3(0, 0, 0);   
        }
    }
    #endregion

    #region Start Enemy Attacks after formed
    public static void SetAttackinMotion()
    {
        MainEnemyFormation mainEnemyForm = GameObject.FindGameObjectWithTag("MainFormation").GetComponent<MainEnemyFormation>();
        if (mainEnemyForm && GalagaHelper.EnemiesKilled < 41)
	    {
            int x = GalagaHelper.RandomNumber(0, 7);
            if (x == 1)
	        {
                mainEnemyForm.enemy1Picked = true;
	        }
            else if (x == 2)
            {
                mainEnemyForm.enemy2Picked = true;
            }
            else if (x == 3)
            {
                mainEnemyForm.enemy3Picked = true;
            }
            else if (x == 4)
            {
                PrintAllGhostObjects();
            }
            else if (x == 5)
            {
                mainEnemyForm.enemy1Picked = true;
            }
            else
	        {
		        mainEnemyForm.enemy1Picked = true;
	        }
            //Debug.Log("Ran random attacks: ".Colored(Colors.green) + x.ToString().Bold());
	    }
    }
    #endregion

    #region Player Icon Functions
    public static int NumberOfPlayerIcons()
    {
        GameObject[] icons = GameObject.FindGameObjectsWithTag("PlayerIcon");
        return icons.Length;
    }

    public static void SetPlayerIcons()
    {
        GameObject[] icons = GameObject.FindGameObjectsWithTag("PlayerIcon");
        PlayerIcons = icons;
    }

    public static void PlacePlayerIcons()
    {
        switch (numOfPlayers)
        {
            case 1:
                for (int i = 0; i < PlayerIcons.Length; i++)
                {
                    Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                    rend.enabled = false;
                }
                break;
            case 2:
                for (int i = 0; i < PlayerIcons.Length; i++)
                {
                    if (PlayerIcons[i].name.Equals("PlayerIcon"))
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = true;
                    }
                    else
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = false;
                    }
                }
                break;
            case 3:
                for (int i = 0; i < PlayerIcons.Length; i++)
                {
                    if (PlayerIcons[i].name.Equals("PlayerIcon") || PlayerIcons[i].name.Equals("PlayerIcon (1)"))
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = true;
                    }
                    else
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = false;
                    }
                }
                break;
            case 4:
                for (int i = 0; i < PlayerIcons.Length; i++)
                {
                    if (PlayerIcons[i].name.Equals("PlayerIcon") || PlayerIcons[i].name.Equals("PlayerIcon (1)") || PlayerIcons[i].name.Equals("PlayerIcon (2)"))
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = true;
                    }
                    else
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = false;
                    }
                }
                break;
            case 5:
                for (int i = 0; i < PlayerIcons.Length; i++)
                {
                    if (PlayerIcons[i].name.Equals("PlayerIcon") || PlayerIcons[i].name.Equals("PlayerIcon (1)") || PlayerIcons[i].name.Equals("PlayerIcon (2)") || PlayerIcons[i].name.Equals("PlayerIcon (3)"))
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = true;
                    }
                    else
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = false;
                    }
                }
                break;
            case 6:
                for (int i = 0; i < PlayerIcons.Length; i++)
                {
                    if (PlayerIcons[i].name.Equals("PlayerIcon") || PlayerIcons[i].name.Equals("PlayerIcon (1)") || PlayerIcons[i].name.Equals("PlayerIcon (2)") || PlayerIcons[i].name.Equals("PlayerIcon (3)") || PlayerIcons[i].name.Equals("PlayerIcon (4)"))
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = true;
                    }
                    else
                    {
                        Renderer rend = PlayerIcons[i].GetComponent<Renderer>();
                        rend.enabled = false;
                    }
                }
                break;
        }
        
    }
    #endregion

    #region Delete Empty GameObjects in Scene
    /// <summary>
    /// Finds and deletes all gameobjects named "New Game Object"
    /// </summary>
    public static void PrintAllGhostObjects()
    {
        int x = 0;
        foreach (GameObject obj in Object.FindObjectsOfType(typeof(GameObject)))
        {
            if (obj.name == "New Game Object")
            {
                //Debug.Log(obj.name.Bold().Sized(10));
                x += 1;
                Object.Destroy(obj.gameObject);
            }
            if (obj.name == "EnemyExplosion")
            {
                Object.Destroy(obj.gameObject);
            }

            if (obj.activeSelf == false && obj.transform.parent != null)
            {
                obj.transform.parent = null;
            }

            if (obj.name.StartsWith("Enemy") && obj.gameObject.name.Contains("projectile") == false && obj.gameObject.name.Contains("Wall") == false && obj.gameObject.activeSelf == true && obj.gameObject.transform.parent == null)
            {
                //Object.Destroy(obj.gameObject);
                x += 1;
                //GalagaHelper.EnemiesKilled += 1;
                //Debug.Log(obj.name.Bold() + " is destoyed.");
            }
        }
        Debug.Log("*** Count of enemies found: ".Bold().Colored(Colors.red) + x.ToString().Bold().Colored(Colors.red));
    }
    #endregion
}
