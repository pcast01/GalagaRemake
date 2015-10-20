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
            Debug.Log(gameObject.name.Bold() + " Enemies Spawned: " + GalagaHelper.EnemiesSpawned + " Enemies Killed: " + GalagaHelper.EnemiesKilled + " Enemies Disabled: "+ GalagaHelper.DisabledEnemies);
            // Check if 8 enemies have spawned then run them
            GalagaHelper.StartRound1();
            if (GalagaHelper.EnemiesKilled == 40)
            {
                Debug.Log("Round2 started".Bold().Sized(11));
                // Reset Variables
                GalagaHelper.ResetFormations();
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

        if (NextFreePosition())
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
        enemy.transform.position = spawn.position;
        enemy.transform.parent = freePos;
        GalagaHelper.EnemiesSpawned += 1;
    }

    public void CreateEnemy4Trio(Transform spawn, Transform freepos)
    {
        Hashtable tweenPath = new Hashtable();
        GameObject[] scorpionTrio = new GameObject[3];
        scorpionTrio[0] = SimplePool.Spawn(enemy4Prefab, spawn.position, enemy4Prefab.transform.rotation, true) as GameObject;
        scorpionTrio[0].transform.position = spawn.position;
        scorpionTrio[0].transform.parent = freepos;

        scorpionTrio[1] = SimplePool.Spawn(enemy4Prefab, spawn.position, enemy4Prefab.transform.rotation, true) as GameObject;
        scorpionTrio[1].transform.position = spawn.position;
        scorpionTrio[1].transform.parent = freepos;

        scorpionTrio[2] = SimplePool.Spawn(enemy4Prefab, spawn.position, enemy4Prefab.transform.rotation, true) as GameObject;
        scorpionTrio[2].transform.position = spawn.position;
        scorpionTrio[2].transform.parent = freepos;

        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        float scorpionWaveDelay = 0.6f;
        //tweenPath.Clear();
        _waypoints = new List<Vector3>();

        if (player)
        {
            for (int i = 0; i < scorpionTrio.Length; i++)
            {
                tweenPath.Clear();
                _waypoints.Clear();
                
                _waypoints.Add(scorpionTrio[i].transform.position);

                player.GetCirclePath();
                Vector3[] pathToPlayer = new Vector3[9];
                pathToPlayer = player.circlePath;

                for (int x = 0; x < 9; x++)
                {
                    _waypoints.Add(pathToPlayer[x]);
                }
                //Debug.Log("Waypoints Count: " + _waypoints.Count);
                Vector3[] newVect3 = new Vector3[_waypoints.Count];
                //Debug.Log(_waypoints.Count.ToString().Bold().Italics());
                for (int y = 0; y < _waypoints.Count; y++)
                {
                    newVect3[y] = _waypoints[y];
                }

                tweenPath.Add("path", newVect3);
                tweenPath.Add("delay", scorpionWaveDelay);
                tweenPath.Add("time", 2.0f);
                tweenPath.Add("easetype", "linear");
                tweenPath.Add("orienttopath", true);
                //tweenPath.Add("onComplete", "CircleComplete");
                //tweenPath.Add("onCompleteTarget", gameObject);
                //iTween.MoveTo(gameObject, tweenPath);

                GalagaHelper.CollectEnemyPaths(scorpionTrio[i], tweenPath);
                scorpionWaveDelay += 0.6f;
            }
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
