using UnityEngine;
using System.Collections;
using System;

public class EnemySpawner : MonoBehaviour {

    // 8x8 size of one enemy
    [Header("Enemy Movement")]
    public GameObject enemyPrefab;
    public float speed = 5.0f;
    public bool isMovingRight;
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
    private float xMin;
    private float xMax;

	void Start () {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
        SpawnUntilFull();
	}
	
    public void StartMovement()
    {
        transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        moveFormation = true;
    }

	void Update () {
        int x = isEnemyInPlace();
        Debug.Log("enemies in place: " + x);
        if (x == 6 && moveFormation == false)
        {
            if (!isStartFormation)
            {
                Invoke("StartMovement", 4f);
                isStartFormation = true;
            }
            //StartMovement();
            Debug.Log("*** ALL Enemies in place. ***");
        }

        if (moveFormation && isStartFormation)
        {
            if (isMovingRight)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }

            float rightEdgeOfFormation = transform.position.x + (0.5f * width);
            float leftEdgeOfFormation = transform.position.x - (0.5f * width);
            if (leftEdgeOfFormation < xMin)
            {
                isMovingRight = true;
            }
            else if (rightEdgeOfFormation > xMax)
            {
                isMovingRight = false;
            }
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
                //Debug.Log("Parent: " + parentZ + " Child: " + childZ);
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
        if (freePosition)
        {
            // Top Spawn Point
            GameObject spawnPt = GameObject.FindGameObjectWithTag("Respawn");
            //GameObject bottomLeftPt = GameObject.FindGameObjectWithTag("Spawn_BottomLeft");
            GameObject enemy = Instantiate(enemyPrefab, spawnPt.transform.position, Quaternion.identity) as GameObject;
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
            //StartMovement();
            //Debug.Log("Start Movement started...");
        }
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
