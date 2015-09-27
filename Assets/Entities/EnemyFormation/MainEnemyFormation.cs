using UnityEngine;
using System.Collections;

public class MainEnemyFormation : MonoBehaviour {

    public bool isMovingRight;
    //public bool isStartFormation;
    public bool moveFormation;
    public float padding = 1f;
    public float speed = 5.0f;
    public float width = 10f;
    public float height = 5f;
    private float xMin;
    private float xMax;
    private bool enemy1Picked = false;
    private bool enemy2Picked = false;
    public GameObject[] enemy1;
    public GameObject[] enemy2;
    public GameObject form1;
    private GameObject playerText;
    private GameObject roundText;
    private GameObject playerTextHigh;

	// Use this for initialization
	void Start () {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
        moveFormation = false;
        GalagaHelper.RoundNumber = 1;
        Invoke("StartRound", 3.0f);
        playerText = GameObject.Find("PlayerText");
        roundText = GameObject.Find("RoundTitle");
        playerTextHigh = GameObject.Find("PlayerTextHigh");
        playerText.SetActive(true);
        roundText.SetActive(false);
        //playerTextHigh.SetActive(false);
        //roundTextPos = playerText.transform.position;
	}

    void PickRandomEnemyOne()
    {
        enemy1 = GameObject.FindGameObjectsWithTag("enemy1");
        //Enemy1Controller enemyOne = GameObject.FindGameObjectWithTag("enemy1").GetComponent<Enemy1Controller>();
        int pickedAtRandom = Random.Range(0, enemy1.Length);
        Debug.Log(enemy1[pickedAtRandom].transform.parent.name.Bold() + " Num: " + pickedAtRandom);
        Enemy1Controller enemyOne = enemy1[pickedAtRandom].GetComponent<Enemy1Controller>();
        if (enemyOne)
        {
            //Debug.Log("Found EnemyOne");
            //enemy1[pickedAtRandom]
            enemyOne.CreatePath();
            enemy1Picked = true;
        }
    }

    void PickRandomEnemyTwo()
    {
        enemy2 = GameObject.FindGameObjectsWithTag("enemy2");
        //Enemy1Controller enemyOne = GameObject.FindGameObjectWithTag("enemy1").GetComponent<Enemy1Controller>();
        int pickedAtRandom = Random.Range(0, enemy1.Length);
        Debug.Log(enemy2[pickedAtRandom].transform.parent.name.Bold() + " Num: " + pickedAtRandom);
        Enemy2Controller enemyTwo = enemy2[pickedAtRandom].GetComponent<Enemy2Controller>();
        if (enemyTwo)
        {
            Debug.Log("Found EnemyTwo");
            //enemy1[pickedAtRandom]
            enemyTwo.AttackPlayer = true;
            enemy2Picked = true;
        }
    }

    void StartRound()
    {
        form1 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        form1.GetComponent<EnemySpawner>().enabled = true;
        Debug.Log("Starting Round 1".Colored(Colors.purple).Bold());
    }

	void Update () {
        //GameObject pt2 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        GalagaHelper.TimeToSpawn = Time.time;
        //Debug.Log(GalagaHelper.TimeToSpawn.ToString().Bold());
        if (GalagaHelper.TimeToSpawn > 0f && GalagaHelper.TimeToSpawn < 3.6f)
        {
            //Debug.Log("See player text?".Bold());
            // show player1 first
            playerText.SetActive(true);
            playerTextHigh.SetActive(false);
        }
        else if (GalagaHelper.TimeToSpawn > 2.0f && GalagaHelper.TimeToSpawn < 3.5f)
        {
            // show round title same place
            playerText.SetActive(false);
            roundText.transform.position = playerText.transform.position;
            roundText.SetActive(true);
            
            //GameObject.Find("PlayerText").SetActive(false);
            //playerText.SetActive(true);
            //GameObject.Find("RoundTitle").SetActive(true);
        }
        else if (GalagaHelper.TimeToSpawn > 3.5f && GalagaHelper.TimeToSpawn < 5.3f)
        {
            // Show both
            //Vector3 newPos = playerText.transform.position + Vector3.up;
            playerText.transform.position = playerTextHigh.transform.position;
            //roundText.transform.position = roundTextPos;
            playerText.SetActive(true);
            roundText.SetActive(true);
            //playerText.SetActive(false);
        }
        else
        {
            playerText.SetActive(false);
            roundText.SetActive(false);
        }


        if (GalagaHelper.EnemiesSpawned > 8 && enemy1Picked == false && form1.GetComponent<EnemySpawner>().isFormationUp == true)
        {
            PickRandomEnemyOne();
        }

        if (GalagaHelper.EnemiesSpawned > 24 && enemy2Picked == false)
        {
            PickRandomEnemyTwo();
        }

        if (moveFormation)
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
        // Check to see if enemies have all been killed.
        //if (GalagaHelper.EnemiesSpawned == 0)
        //{
        //    Debug.Log("Round 2 about to begin".Colored(Colors.green));
        //    // Reset Formations
        //    GalagaHelper.ResetFormations();
        //    GalagaHelper.RoundNumber += 1;
        //}
        //Debug.Log("Enemies Currently Spawned: " + GalagaHelper.EnemiesSpawned + " Wave#: " + GalagaHelper.CurrentRoundPhase + " Round #: " + GalagaHelper.RoundNumber);
	}

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
