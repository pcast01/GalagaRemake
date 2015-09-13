using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour {

    // 8x8 size of one enemy
    [Header("Enemy Movement")]
    public GameObject enemyPrefab;
    [Header("Gizmo Settings")]
    public float width = 10f;
    public float height = 5f;
    public float padding = 1f;
    [Header("Spawn Settings")]
    public Transform currentSpawnPos;
    public float spawnDelay = 0.5f;
    public bool moveFormation = false;
    public bool isStartFormation = false;
    public bool spawnEntranceRight = false;
    [Header("Formation")]
    public EnemySpawner round1Phase2spawner;
    public bool isFormationUp = false;
    public int enemiesInPlace = 0;

	void Start () {
        if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase1 && gameObject.name == "Round1Phase1EnemyFormation")
        {
            SpawnUntilFull();
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase2 && gameObject.name == "Round1Phase2EnemyFormation")
        {
            SpawnUntilFull();
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase3_1 && gameObject.name == "Round1Phase3_1EnemyFormation")
        {
            SpawnUntilFull();
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase4_1 && gameObject.name == "Round1Phase4_1EnemyFormation")
        {
            SpawnUntilFull();
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase5_1 && gameObject.name == "Round1Phase5_1EnemyFormation")
        {
            SpawnUntilFull();
        }
	}

	void Update () {
        enemiesInPlace = isEnemyInPlace();
        Debug.Log(gameObject.name + " - enemies in place: " + enemiesInPlace + " Enemies Spawned: " + GalagaHelper.EnemiesSpawned);
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
	}

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

    public int isEnemyInPlace()
    {
        int x = 0;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).childCount == 1)
            {
                //Debug.Log("child count = " + transform.GetChild(i).childCount);
                //Debug.Log("Parent pos: " + transform.GetChild(i).position + " Child pos: " + transform.GetChild(i).GetChild(0).position);
                int childZ = (int)Math.Round(transform.GetChild(i).GetChild(0).position.z, 0);
                int parentZ = (int)Math.Round(transform.GetChild(i).position.z, 0);
                //if (gameObject.name == "Round1Phase4_1EnemyFormation")
                //{
                //    Debug.Log("Enemies spawned: " + GalagaHelper.EnemiesSpawned + " Parent Pos: " + parentZ + " Child Pos: " + childZ);
                //}
                if (childZ == parentZ)
                {
                    //Debug.Log("Child in place.");
                    x += 1;
                }
            }
        }
        return x;
    }

    void SpawnUntilFull()
    {
        Transform freePosition = NextFreePosition();
        Transform spawnPoint = GalagaHelper.RespawnPoint(gameObject.name);
        if (freePosition)
        {
            GameObject enemy = Instantiate(enemyPrefab, spawnPoint.position, enemyPrefab.transform.rotation) as GameObject;
            currentSpawnPos = freePosition;
            if (spawnEntranceRight)
            {
                spawnEntranceRight = false;
            }
            else
            {
                spawnEntranceRight = true;
            }
            //Debug.Log("Enemy spawned." + "free pos=" + freePosition.position.z);
            enemy.transform.parent = freePosition;
            Debug.Log("Enemy parent name: " + enemy.transform.parent.name + " FreePos: " + freePosition.name);
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
