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
    private int enemyAttacks = 0;
    public bool enemy1Picked = false;
    public bool enemy2Picked = false;
    public bool isEnemy1Done = false;
    public bool isEnemy2Done = false;
    public GameObject[] enemy1;
    public GameObject[] enemy2;
    public GameObject form1;
    private GameObject playerText;
    private GameObject roundText;
    private GameObject playerTextHigh;

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
        int pickedAtRandom = Random.Range(0, enemy1.Length);
        //Debug.Log(enemy1[pickedAtRandom].transform.parent.name.Bold() + " Num: " + pickedAtRandom);
        Enemy1Controller enemyOne = enemy1[pickedAtRandom].GetComponent<Enemy1Controller>();
        if (enemyOne)
        {
            //Debug.Log("Found EnemyOne");
            //enemy1[pickedAtRandom]
            enemyOne.CreatePath();
            enemy1Picked = false;
        }
    }

    void PickRandomEnemyTwo()
    {
        enemy2 = GameObject.FindGameObjectsWithTag("enemy2");
        //Enemy1Controller enemyOne = GameObject.FindGameObjectWithTag("enemy1").GetComponent<Enemy1Controller>();
        int pickedAtRandom = Random.Range(0, enemy1.Length);
        //Debug.Log(enemy2[pickedAtRandom].transform.parent.name.Bold() + " Num: " + pickedAtRandom);
        Enemy2Controller enemyTwo = enemy2[pickedAtRandom].GetComponent<Enemy2Controller>();
        if (enemyTwo)
        {
            Debug.Log("Found EnemyTwo");
            //enemy1[pickedAtRandom]
            enemyTwo.AttackPlayer = true;
            enemy2Picked = false;
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

        // Set the player text to show like Galaga
        GalagaHelper.TimeToSpawn = Time.time;
        //Debug.Log(GalagaHelper.TimeToSpawn.ToString().Bold());
        if (GalagaHelper.TimeToSpawn > 0f && GalagaHelper.TimeToSpawn < 2.0f)
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
        }
        else if (GalagaHelper.TimeToSpawn > 3.5f && GalagaHelper.TimeToSpawn < 5.3f)
        {
            // Show both
            playerText.transform.position = playerTextHigh.transform.position;
            playerText.SetActive(true);
            roundText.SetActive(true);
        }
        else
        {
            playerText.SetActive(false);
            roundText.SetActive(false);
        }

        //if (GalagaHelper.EnemiesSpawned > 8 && enemy1Picked == false && form1.GetComponent<EnemySpawner>().isFormationUp == true)
        //{
        //    PickRandomEnemyOne();
        //}

        //if (GalagaHelper.EnemiesSpawned > 24 && enemy2Picked == false)
        //{
        //    PickRandomEnemyTwo();
        //}

        // Move formation left and right
        if (moveFormation)
        {
            //if (isMovingRight)
            //{
            //    transform.position += Vector3.right * speed * Time.deltaTime;
            //}
            //else
            //{
            //    transform.position += Vector3.left * speed * Time.deltaTime;
            //}

            //float rightEdgeOfFormation = transform.position.x + (0.5f * width);
            //float leftEdgeOfFormation = transform.position.x - (0.5f * width);
            //if (leftEdgeOfFormation < xMin)
            //{
            //    isMovingRight = true;
            //}
            //else if (rightEdgeOfFormation > xMax)
            //{
            //    isMovingRight = false;
            //}

            GalagaHelper.SetAttackinMotion();
            
        }

        if (enemyAttacks == 0)
        {
            enemyAttacks = 1;
            isEnemy1Done = true;
            isEnemy2Done = true;    
        }

        if (enemy1Picked && isEnemy1Done == true)
        {
            PickRandomEnemyOne();
            enemy1Picked = false;
            isEnemy1Done = false;
        }

        if (enemy2Picked && isEnemy2Done == true)
        {
            PickRandomEnemyTwo();
            enemy2Picked = false;
            isEnemy2Done = false;
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
