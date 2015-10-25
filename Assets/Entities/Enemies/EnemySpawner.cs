using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

public class EnemySpawner : MonoBehaviour {

    // 8x8 size of one enemy
    [Header("Enemy Movement")]
    public GameObject enemy1Prefab;
    public GameObject enemy2Prefab;
    public GameObject enemy3Prefab;
    public GameObject enemy4Prefab;
    [Header("Gizmo Settings")]
    public float width = 10f;
    public float height = 5f;
    public float padding = 1f;
    [Header("Spawn Settings")]
    public Transform currentSpawnPos;
    public float spawnDelay = 0.5f;
    public bool spawnEntranceRight = false;
    [Header("Formation")]
    //private EnemySpawner round1Phase2spawner;
    public bool isFormationUp = false;
    public bool isFull = false;
    private List<Vector3> _waypoints;
    //private int enemiesInPlace = 0;

    void Awake()
    {
        //SimplePool.Preload(enemy1Prefab, 8);
    }

	void Start () {
        // Execute Spawn function
        SpawnUntilFull();
        GalagaHelper.TimeToSpawn = Time.time;
	}

	void Update () {
        
        // Get the number of enemies in place on the current Formation
        //enemiesInPlace = isEnemyInPlace();
        //+ " - enemies in place: " + enemiesInPlace.ToString().Colored(Colors.red

        if (gameObject.name == "Round1Phase1EnemyFormation")
        {
            Debug.Log(gameObject.name.Bold() + " Enemies Spawned: " + GalagaHelper.EnemiesSpawned + " Enemies Killed: " + GalagaHelper.EnemiesKilled + " Enemies Disabled: "+ GalagaHelper.DisabledEnemies + " PlayerLifes: " + GalagaHelper.numOfPlayers);
            // Check if 8 enemies have spawned then run them
            GalagaHelper.StartRound1();
            if (GalagaHelper.EnemiesKilled >= GalagaHelper.EnemiesSpawned)
            {
                Debug.Log("Round2 started".Bold().Sized(11));
                // Reset Variables
                GalagaHelper.ResetFormations();
                GalagaHelper.CurrentRoundPhase = GalagaHelper.Formations.Round1Phase1;
                GalagaHelper.RoundNumber = 1;
                MainEnemyFormation main = GameObject.FindGameObjectWithTag("MainFormation").GetComponent<MainEnemyFormation>();
                main.RestartRound();
                GalagaHelper.EnemiesKilled = 0;
                GalagaHelper.EnemiesSpawned = 0;
                SpawnUntilFull();
            }
        }

        // Enable the next script to be able to run based on which script is running and if all
        // enemies are in place in the formation.
        //if (GalagaHelper.EnemiesSpawned > 7 && gameObject.name == "Round1Phase1EnemyFormation")
        //{
        //    GalagaHelper.TimeDone = Time.time;
        //    //Debug.Log("Time to get to position: ".Bold() + (GalagaHelper.TimeDone - GalagaHelper.TimeToSpawn));
        //    GameObject pt2 = GameObject.FindGameObjectWithTag("phase2").gameObject;
        //    pt2.GetComponent<EnemySpawner>().enabled = true;
        //    //Debug.Log("*** ALL Enemies in place. ***");
        //}
        //else if (GalagaHelper.EnemiesSpawned > 15 && gameObject.name == "Round1Phase2EnemyFormation")
        //{
        //    GameObject pt3 = GameObject.FindGameObjectWithTag("phase31").gameObject;
        //    pt3.GetComponent<EnemySpawner>().enabled = true;
        //    //var message = "Enemy formation 3 on";
        //    //Debug.Log(message.Bold().Sized(8));
        //}
        //else if (GalagaHelper.EnemiesSpawned > 23 && gameObject.name == "Round1Phase3_1EnemyFormation")
        //{
        //    GameObject pt4 = GameObject.FindGameObjectWithTag("phase41").gameObject;
        //    pt4.GetComponent<EnemySpawner>().enabled = true;
        //}
        //else if (GalagaHelper.EnemiesSpawned > 31 && gameObject.name == "Round1Phase4_1EnemyFormation")
        //{
        //    GameObject pt5 = GameObject.FindGameObjectWithTag("phase51").gameObject;
        //    pt5.GetComponent<EnemySpawner>().enabled = true;
        //}
        //else if (GalagaHelper.EnemiesSpawned > 40 && gameObject.name == "Round1Phase5_1EnemyFormation")
        //{
        //    GameObject mef = GameObject.FindGameObjectWithTag("MainFormation").gameObject;
        //    //mef.GetComponent<MainEnemyFormation>().moveFormation = true;

        //}
	}

    /// <summary>
    /// Remove child from positions in formation.
    /// </summary>
    public void DisownChildren()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount == 1)
            {
                transform.GetChild(i).GetChild(0).parent = null;
            }
        }
    }

    /// <summary>
    /// Gets the next Free position in the formation for the enemy to fly to.
    /// </summary>
    /// <returns></returns>
    public Transform NextFreePosition()
    {
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount == 0)
            {
                return transform.GetChild(i);
            }
        }
        return null;
    }

    /// <summary>
    /// Gets the number of Enemies that are setup in the current formation.
    /// </summary>
    /// <returns></returns>
    public int isEnemyInPlace()
    {
        int x = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            //Debug.Log(transform.name + " Child count: " + transform.GetChild(i).childCount);
            if (transform.GetChild(i).childCount >= 1)
            {
                //Debug.Log("child count = " + transform.GetChild(i).childCount);
                //Debug.Log("Parent pos: " + transform.GetChild(i).position + " Child pos: " + transform.GetChild(i).GetChild(0).position);
                int childZ = (int)Math.Round(transform.GetChild(i).GetChild(0).position.z, 0);
                int parentZ = (int)Math.Round(transform.GetChild(i).position.z, 0);
                Transform childEnemy = transform.GetChild(i).GetChild(0);
                //EnemyController childEnemyScript = childEnemy.GetComponent<EnemyController>();
                
                // if the child is in position or is dead from bullets
                if (childZ == parentZ || childEnemy.gameObject.activeSelf == false)
                {
                    if (childEnemy.gameObject.activeSelf == false)
                    {
                        Debug.Log("Enemy obj disabled.".Italics().Colored(Colors.red));
                    }
                    x += 1;
                }
            }
            else
            {
                //Debug.Log("Grandchild not present".Bold());
            }
        }
        return x;
    }

    /// <summary>
    /// Spawn every enemy in the formation that you are in and then switch to the next wave. Uses Invoke
    /// Command to call it self and when full then it sets the currentroundphase to the next wave.
    /// </summary>
    void SpawnUntilFull()
    {
        // Get the ntxt Free position that is empty in the formation.
        Transform freePosition = NextFreePosition();
        Transform spawnPoint = GalagaHelper.RespawnPoint(gameObject.name, spawnEntranceRight);
        if (freePosition)
        {
            currentSpawnPos = freePosition;
            // Alternate between Left and Right entrance.
            if (spawnEntranceRight)
            {
                spawnEntranceRight = false;
            }
            else
            {
                spawnEntranceRight = true;
            }
            
            // Spawn enemy in enemy1Prefab.
            SpawnEnemy(spawnPoint, freePosition);
        }

        if (NextFreePosition() && this.isFull == false)
        {
            Debug.Log(gameObject.name.Bold() + " Free position");
            Invoke("SpawnUntilFull", spawnDelay);
        }
        else
        {
            isFormationUp = true;
            Debug.Log(gameObject.name + " is formUp");
            //GalagaHelper.CurrentRoundPhase += 1;
        }
    }

    void SpawnEnemy(Transform spawn, Transform freePos)
    {
        GameObject defaultEnemyPrefab = new GameObject();

        if (GalagaHelper.RoundNumber == 1)
        {
            switch (GalagaHelper.CurrentRoundPhase)
            {
                case GalagaHelper.Formations.Round1Phase1:
                    if (GalagaHelper.EnemiesSpawned < 4)
                    {
                        defaultEnemyPrefab = enemy1Prefab;
                        //Debug.Log("enemy1 spawned".Colored(Colors.yellow));
                    }
                    else if (GalagaHelper.EnemiesSpawned > 3 && GalagaHelper.EnemiesSpawned < 9)
                    {
                        defaultEnemyPrefab = enemy2Prefab;
                        //Debug.Log("enemy2 spawned".Colored(Colors.red));
                    }
                    break;
                case GalagaHelper.Formations.Round1Phase2:
                    //Debug.Log("Free pos == " + freePos.gameObject.name);
                    if (freePos.gameObject.name.Equals("Position") || freePos.gameObject.name.Equals("Position (1)") || freePos.gameObject.name.Equals("Position (6)") || freePos.gameObject.name.Equals("Position (7)"))
                    {
                        defaultEnemyPrefab = enemy3Prefab;
                    }
                    else
                    {
                        defaultEnemyPrefab = enemy2Prefab;
                    }
                    //if (spawnEntranceRight)
                    //{
                    //    defaultEnemyPrefab = enemy2Prefab;
                    //}
                    //else
                    //{
                    //    defaultEnemyPrefab = enemy3Prefab;
                    //}
                    break;
                case GalagaHelper.Formations.Round1Phase3:
                    Debug.Log("Free pos == " + freePos.gameObject.name);
                    defaultEnemyPrefab = enemy2Prefab;
                    break;
                case GalagaHelper.Formations.Round1Phase4:
                    Debug.Log("Free pos == " + freePos.gameObject.name);
                    defaultEnemyPrefab = enemy1Prefab;
                    break;
                case GalagaHelper.Formations.Round1Phase5:
                    defaultEnemyPrefab = enemy1Prefab;
                    break;
                default:
                    defaultEnemyPrefab = enemy1Prefab;
                    break;
            }
        }
        else if (GalagaHelper.RoundNumber == 2)
        {
            switch (GalagaHelper.CurrentRoundPhase)
            {
                case GalagaHelper.Formations.Round1Phase1:
                    if (GalagaHelper.EnemiesSpawned < 4)
                    {
                        defaultEnemyPrefab = enemy1Prefab;
                        //Debug.Log("enemy1 spawned".Colored(Colors.yellow));
                    }
                    else if (GalagaHelper.EnemiesSpawned > 3 && GalagaHelper.EnemiesSpawned < 9)
                    {
                        defaultEnemyPrefab = enemy2Prefab;
                        //Debug.Log("enemy2 spawned".Colored(Colors.red));
                    }
                    break;
                case GalagaHelper.Formations.Round1Phase2:
                    //Debug.Log("Free pos == " + freePos.gameObject.name);
                    if (freePos.gameObject.name.Equals("Position") || freePos.gameObject.name.Equals("Position (1)") || freePos.gameObject.name.Equals("Position (6)") || freePos.gameObject.name.Equals("Position (7)"))
                    {
                        defaultEnemyPrefab = enemy3Prefab;
                    }
                    else
                    {
                        defaultEnemyPrefab = enemy2Prefab;
                    }
                    //if (spawnEntranceRight)
                    //{
                    //    defaultEnemyPrefab = enemy2Prefab;
                    //}
                    //else
                    //{
                    //    defaultEnemyPrefab = enemy3Prefab;
                    //}
                    break;
                case GalagaHelper.Formations.Round1Phase3:
                    defaultEnemyPrefab = enemy2Prefab;
                    break;
                case GalagaHelper.Formations.Round1Phase4:
                    defaultEnemyPrefab = enemy1Prefab;
                    break;
                case GalagaHelper.Formations.Round1Phase5:
                    defaultEnemyPrefab = enemy1Prefab;
                    break;
                default:
                    defaultEnemyPrefab = enemy1Prefab;
                    break;
            }
        }

        GameObject enemy = SimplePool.Spawn(defaultEnemyPrefab, spawn.position, defaultEnemyPrefab.transform.rotation, true) as GameObject;
        enemy.GetComponent<MeshCollider>().enabled = true;
        enemy.GetComponent<Renderer>().enabled = true;
        enemy.transform.position = spawn.position;
        enemy.transform.parent = freePos;
        Debug.Log("Enemy Name: " + enemy.name + " Parent: " + enemy.transform.parent.parent.name.Colored(Colors.blue) + " Position: " + enemy.transform.parent.name.Colored(Colors.blue));
        GalagaHelper.EnemiesSpawned += 1;
    }

    public void CreateEnemy4Trio(Transform spawn, Transform freepos)
    {
        Hashtable tweenPath = new Hashtable();
        Hashtable tweenPath1 = new Hashtable();
        Hashtable tweenPath2 = new Hashtable();
        GameObject[] scorpionTrio = new GameObject[3];
        scorpionTrio[0] = SimplePool.Spawn(enemy4Prefab, spawn.position, enemy4Prefab.transform.rotation, true) as GameObject;
        scorpionTrio[0].transform.position = spawn.position;
        scorpionTrio[0].transform.parent = freepos;
        scorpionTrio[0].GetComponent<MeshCollider>().enabled = true;
        GalagaHelper.enemyFourOrigRotation = scorpionTrio[0].transform.rotation;

        scorpionTrio[1] = SimplePool.Spawn(enemy4Prefab, spawn.position, enemy4Prefab.transform.rotation, true) as GameObject;
        scorpionTrio[1].transform.position = spawn.position;
        scorpionTrio[1].transform.parent = freepos;
        scorpionTrio[1].GetComponent<MeshCollider>().enabled = true;

        scorpionTrio[2] = SimplePool.Spawn(enemy4Prefab, spawn.position, enemy4Prefab.transform.rotation, true) as GameObject;
        scorpionTrio[2].transform.position = spawn.position;
        scorpionTrio[2].transform.parent = freepos;
        scorpionTrio[2].GetComponent<MeshCollider>().enabled = true;

        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _waypoints = new List<Vector3>();
        tweenPath.Clear();
        tweenPath1.Clear();
        tweenPath2.Clear();
        if (player)
        {
            for (int i = 0; i < scorpionTrio.Length; i++)
            {
                _waypoints.Clear();
                _waypoints.Add(scorpionTrio[i].transform.position);
                _waypoints.Add(GameObject.FindGameObjectWithTag("Point2").GetComponent<Transform>().position);
                player.GetCirclePathScorpions();
                Vector3[] pathToPlayer = new Vector3[5];
                pathToPlayer = player.scorpionCirclePath;

                for (int x = 0; x < 5; x++)
                {
                    _waypoints.Add(pathToPlayer[x]);
                }
                _waypoints.Add(GameObject.FindGameObjectWithTag("Point2").GetComponent<Transform>().position);
                _waypoints.Add(scorpionTrio[i].transform.position);
                // Copy Vector3 to a new Vector3 array
                //Debug.Log("Waypoints Count: " + _waypoints.Count);
                Vector3[] newVect3 = new Vector3[_waypoints.Count];
                //Debug.Log(_waypoints.Count.ToString().Bold().Italics());
                for (int y = 0; y < _waypoints.Count; y++)
                {
                    newVect3[y] = _waypoints[y];
                }

                if (i == 0)
                {
                    tweenPath.Add("path", newVect3);
                    tweenPath.Add("delay", 0.05f);
                    tweenPath.Add("time", 8.0f);
                    tweenPath.Add("easetype", "linear");
                    tweenPath.Add("orienttopath", true);
                    GalagaHelper.CollectScorpionPaths(scorpionTrio[i], tweenPath);
                }
                else if (i == 1)
                {
                    tweenPath1.Add("path", newVect3);
                    tweenPath1.Add("delay", 0.6f);
                    tweenPath1.Add("time", 8.0f);
                    tweenPath1.Add("easetype", "linear");
                    tweenPath1.Add("orienttopath", true);
                    GalagaHelper.CollectScorpionPaths(scorpionTrio[i], tweenPath1);
                }
                else if (i == 2)
                {
                    tweenPath2.Add("path", newVect3);
                    tweenPath2.Add("delay", 1.0f);
                    tweenPath2.Add("time", 8.0f);
                    tweenPath2.Add("easetype", "linear");
                    tweenPath2.Add("orienttopath", true);
                    tweenPath2.Add("onComplete", "EnemyBack");
                    tweenPath2.Add("onCompleteTarget", gameObject);
                    GalagaHelper.CollectScorpionPaths(scorpionTrio[i], tweenPath2);
                }
            }
        }
    }

    public void EnemyBack()
    {
        GameObject[] enemyFours = new GameObject[3];
        enemyFours = GameObject.FindGameObjectsWithTag("enemy4");
        for (int i = 0; i < enemyFours.Length; i++)
        {
            enemyFours[i].GetComponent<Transform>().rotation = GalagaHelper.enemyFourOrigRotation;
            if (i > 0)
            {
                SimplePool.Despawn(enemyFours[i]);
                enemyFours[i].transform.parent = null;
            }
        }
        GalagaHelper.RemoveScorpionPaths();
        GameObject.FindGameObjectWithTag("MainFormation").GetComponent<MainEnemyFormation>().isEnemy1Done = true;
        //main.isEnemy1Done = true;
        //Debug.Log("Enemy 4 orig rotation".Colored(Colors.red));
    }

    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            iTween.DrawPathGizmos(_waypoints.ToArray());
        }
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
