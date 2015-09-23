using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour {

    // 8x8 size of one enemy
    [Header("Enemy Movement")]
    public GameObject enemy1Prefab;
    [Header("Gizmo Settings")]
    public float width = 10f;
    public float height = 5f;
    public float padding = 1f;
    [Header("Spawn Settings")]
    public Transform currentSpawnPos;
    public float spawnDelay = 0.5f;
    //public bool moveFormation = false;
    //public bool isStartFormation = false;
    public bool spawnEntranceRight = false;
    [Header("Formation")]
    private EnemySpawner round1Phase2spawner;
    private bool isFormationUp = false;
    private int enemiesInPlace = 0;

    void Awake()
    {
        SimplePool.Preload(enemy1Prefab, 8);
    }

	void Start () {
        // Execute Spawn function
        SpawnUntilFull();
	}

	void Update () {
        
        // Get the number of enemies in place on the current Formation
        enemiesInPlace = isEnemyInPlace();
        Debug.Log(gameObject.name.Bold() + " - enemies in place: " + enemiesInPlace.ToString().Colored(Colors.red) + " Enemies Spawned: " + GalagaHelper.EnemiesSpawned);

        if (gameObject.name == "Round1Phase1EnemyFormation")
        {
            // Check if 8 enemies have spawned then run them
            GalagaHelper.StartRound1();
            if (GalagaHelper.EnemiesSpawned == 0)
            {
                Debug.Log("Round2 started".Bold().Sized(11));
                SpawnUntilFull();
            }
        }

        // Enable the next script to be able to run based on which script is running and if all
        // enemies are in place in the formation.
        if (enemiesInPlace == 8 && gameObject.name == "Round1Phase1EnemyFormation")
        {
            GameObject pt2 = GameObject.FindGameObjectWithTag("phase2").gameObject;
            pt2.GetComponent<EnemySpawner>().enabled = true;
            //Debug.Log("*** ALL Enemies in place. ***");
        }
        else if (enemiesInPlace == 8 && gameObject.name == "Round1Phase2EnemyFormation")
        {
            GameObject pt3 = GameObject.FindGameObjectWithTag("phase31").gameObject;
            pt3.GetComponent<EnemySpawner>().enabled = true;
            //var message = "Enemy formation 3 on";
            //Debug.Log(message.Bold().Sized(8));
        }
        else if (enemiesInPlace == 8 && gameObject.name == "Round1Phase3_1EnemyFormation")
        {
            GameObject pt4 = GameObject.FindGameObjectWithTag("phase41").gameObject;
            pt4.GetComponent<EnemySpawner>().enabled = true;
        }
        else if (enemiesInPlace == 8 && gameObject.name == "Round1Phase4_1EnemyFormation")
        {
            GameObject pt5 = GameObject.FindGameObjectWithTag("phase51").gameObject;
            pt5.GetComponent<EnemySpawner>().enabled = true;
        }
        else if (enemiesInPlace == 8 && gameObject.name == "Round1Phase5_1EnemyFormation")
        {
            GameObject mef = GameObject.FindGameObjectWithTag("MainFormation").gameObject;
            mef.GetComponent<MainEnemyFormation>().enabled = true;
        }
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
            Debug.Log(transform.name + " Child count: " + transform.GetChild(i).childCount);
            if (transform.GetChild(i).childCount >= 1)
            {
                //Debug.Log("child count = " + transform.GetChild(i).childCount);
                //Debug.Log("Parent pos: " + transform.GetChild(i).position + " Child pos: " + transform.GetChild(i).GetChild(0).position);
                int childZ = (int)Math.Round(transform.GetChild(i).GetChild(0).position.z, 0);
                int parentZ = (int)Math.Round(transform.GetChild(i).position.z, 0);
                Transform childEnemy = transform.GetChild(i).GetChild(0);
                EnemyController childEnemyScript = childEnemy.GetComponent<EnemyController>();
                
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
            //SimplePool.Spawn()
            //GameObject enemy = Instantiate(enemy1Prefab, spawnPoint.position, enemy1Prefab.transform.rotation) as GameObject;
            GameObject enemy = SimplePool.Spawn(enemy1Prefab, spawnPoint.position, enemy1Prefab.transform.rotation, true) as GameObject;
            enemy.transform.position = spawnPoint.position;
            //Debug.Log("Enemy spawned." + "free pos=" + freePosition.position.z);
            // Set free position's Parent
            enemy.transform.parent = freePosition;
            //Debug.Log("Enemy parent name: " + enemy.transform.parent.name + " FreePos: " + freePosition.name);
        }

        if (NextFreePosition())
        {
            //Debug.Log("Free position");
            Invoke("SpawnUntilFull", spawnDelay);
        }
        else
        {
            isFormationUp = false;
            GalagaHelper.CurrentRoundPhase += 1;
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
