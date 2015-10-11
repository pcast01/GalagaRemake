using UnityEngine;
using System.Collections;

public class MainEnemyFormation : MonoBehaviour {

    public bool isMovingRight;
    //public bool isStartFormation;
    public bool moveFormation;
    private bool isTextDone = false;
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
    private GameObject readyText;
    private PlayerController playerController;
    private ParticleSystem starfield;

	void Start () {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
        moveFormation = false;
        GalagaHelper.RoundNumber = 1;
        // Starts the Game.
        //Invoke("StartRound", 3.0f);
        playerText = GameObject.Find("PlayerText");
        roundText = GameObject.Find("RoundTitle");
        playerTextHigh = GameObject.Find("PlayerTextHigh");
        readyText = GameObject.Find("ReadyText");
        //playerController = GameObject.FindGameObjectWithTag("CapturedPlayer").GetComponent<PlayerController>();
        starfield = GameObject.FindGameObjectWithTag("Starfield").GetComponent<ParticleSystem>();
        playerText.SetActive(true);
        roundText.SetActive(false);
        playerTextHigh.SetActive(false);
        readyText.SetActive(false);
        //roundTextPos = playerText.transform.position;
	}


    #region RandomEnemyAttacks
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
    #endregion

    void StartRound()
    {
        form1 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        form1.GetComponent<EnemySpawner>().enabled = true;
        Debug.Log("Starting Round 1".Colored(Colors.purple).Bold());
    }

	void Update () {
        //GameObject pt2 = GameObject.FindGameObjectWithTag("phase1").gameObject;

        #region SetBeginningText
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
           // Debug.Log("See player text?".Bold());
            playerText.SetActive(false);
            roundText.transform.position = playerText.transform.position;
            roundText.SetActive(true);
        }
        else if (GalagaHelper.TimeToSpawn > 3.5f && GalagaHelper.TimeToSpawn < 5.3f)
        {
            //Debug.Log("See player text?".Bold());
            // Show both
            playerText.transform.position = playerTextHigh.transform.position;
            playerText.SetActive(true);
            roundText.SetActive(true);
        }
        else
        {
            playerText.SetActive(false);
            roundText.SetActive(false);
            isTextDone = true;
            //Debug.Log("isTextDone eq true");
        }
        #endregion

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

        // If found a player captured then set ready text.
        if (GalagaHelper.isPlayerCaptured == true)
        {
            // turn on Ready text
            readyText.SetActive(true);
            // Pause starfield
            if (starfield)
            {
                starfield.Pause();
                Debug.Log("Paused starfield");
            }
        }
	}

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
