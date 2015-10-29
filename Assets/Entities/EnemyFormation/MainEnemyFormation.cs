using UnityEngine;
using System.Collections;
using UnityEngine.UI;

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
    public bool enemy3Picked = false;
    public bool isEnemy1Done = false;
    public bool isEnemy2Done = false;
    public bool isEnemy3Done = false;
    public bool isPlayerReady = false;
    public GameObject player;
    public GameObject[] enemy1;
    public GameObject[] enemy2;
    public GameObject[] enemy3;
    public GameObject form1;
    private GameObject form2;
    private GameObject form3;
    private GameObject form4;
    private GameObject form5;
    public bool secondWaveFinished;
    public bool thirdWaveFinished;
    public bool fourthWaveFinished;

    private float timeBetweenSpawn;
    private GameObject playerText;
    private GameObject roundText;
    private GameObject playerTextHigh;
    private GameObject readyText;
    private PlayerController playerController;
    private ParticleSystem starfield;
    public bool isReadyDone;

    void Awake()
    {
        //GameObject playerSpawn = GameObject.Find("PlayerSpawn");
        //GameObject newPlayer = SimplePool.Spawn(player, playerSpawn.transform.position, playerSpawn.transform.rotation, true) as GameObject;
        //newPlayer.GetComponent<PlayerController>().enabled = true;
        //newPlayer.transform.position = playerSpawn.transform.position;
    }

	void Start () {

        // Formations
        form1 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        form2 = GameObject.FindGameObjectWithTag("phase2").gameObject;
        form3 = GameObject.FindGameObjectWithTag("phase31").gameObject;
        form4 = GameObject.FindGameObjectWithTag("phase41").gameObject;
        form5 = GameObject.FindGameObjectWithTag("phase51").gameObject;
        // Movement
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
        moveFormation = false;

        GalagaHelper.RoundNumber = 1;
        // Starts the Game.
        Invoke("StartRound", 3.0f);
        GalagaHelper.SetPlayerIcons();
        GalagaHelper.PlacePlayerIcons();
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

    public void EndGame()
    {
        Debug.Log("End Game Called".Colored(Colors.red).Bold());
        Application.LoadLevel("Lose Screen");
    }

    public void RestartRound()
    {
        secondWaveFinished = false;
        thirdWaveFinished = false;
        fourthWaveFinished = false;
        //playerText.SetActive(true);
        roundText.GetComponent<Text>().text = "Stage " + GalagaHelper.RoundNumber.ToString();
        roundText.SetActive(false);
        playerTextHigh.SetActive(false);
        readyText.SetActive(false);
        //Invoke("StartRound", 3.0f);
    }

    #region RandomEnemyAttacks
    void PickRandomEnemyOne()
    {
        enemy1 = GameObject.FindGameObjectsWithTag("enemy1");
        if (enemy1.Length > 0 && GalagaHelper.isPlayerCaptured == false)
        {
            int randScorpion = GalagaHelper.RandomNumber(0, 5);
            Debug.Log("Enemy1 RandNumber: ".Colored(Colors.red) + randScorpion.ToString().Colored(Colors.red));
            Enemy1Controller enemyOne = enemy1[GalagaHelper.RandomNumber(0, enemy1.Length)].GetComponent<Enemy1Controller>();
            if (enemyOne)
            {
                if (randScorpion == 3)
                {
                    enemyOne.startScorpionAttack = true;
                }
                else
                {
                    //Debug.Log("Found EnemyOne");
                    enemyOne.CreatePath();
                    //enemyOne.isEnemyFiring = true;
                }
                enemyOne.isRandomPicked = true;
                enemy1Picked = false;
            }
            
        }
    }

    void PickRandomEnemyTwo()
    {
        enemy2 = GameObject.FindGameObjectsWithTag("enemy2");
        if (enemy2.Length > 0 && GalagaHelper.isPlayerCaptured == false)
        {
            //Debug.Log(enemy2[pickedAtRandom].transform.parent.name.Bold() + " Num: " + pickedAtRandom);
            Enemy2Controller enemyTwo = enemy2[GalagaHelper.RandomNumber(0, enemy2.Length)].GetComponent<Enemy2Controller>();
            if (enemyTwo)
            {
                Debug.Log("Found EnemyTwo");
                //enemy1[pickedAtRandom]
                enemyTwo.AttackPlayer = true;
                enemyTwo.isRandomPicked = true;
                enemy2Picked = false;
            }
        }
    }

    void PickRandomEnemyThreeAttack()
    {
        enemy3 = GameObject.FindGameObjectsWithTag("enemy3");
        if (enemy3.Length > 0 && GalagaHelper.isPlayerCaptured == false)
        {
            Enemy3Controller enemyThree = enemy3[GalagaHelper.RandomNumber(0, enemy3.Length)].GetComponent<Enemy3Controller>();
            if (enemyThree)
            {
                Debug.Log("Found Enemy Three");
                int randomTractorBeam = GalagaHelper.RandomNumber(0, 6);
                if (randomTractorBeam == 3 && GalagaHelper.isTractorBeamOn == false && GalagaHelper.isPlayerCaptured == false)
                {
                    enemyThree.isTractorBeamAttack = true;
                }
                else
                {
                    enemyThree.isAttackPlayer = true;
                }
                enemyThree.isRandomPicked = true;
                enemy3Picked = false;
            }
        }
    }

    #endregion

    void StartRound()
    {
        //form1 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        form1.GetComponent<EnemySpawner>().enabled = true;
        Debug.Log("Starting Round 1".Colored(Colors.purple).Bold());
    }

	void Update () {

        #region SetBeginningText
        // Set the player text to show like Galaga
        
        if (GalagaHelper.RoundNumber == 1)
        {
            GalagaHelper.TimeToSpawn = Time.time;
        }
        else
        {
            GalagaHelper.TimeToSpawn = Time.time - GalagaHelper.StartTime;
        }
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
            //timeBetweenSpawn = 0;

        }
        #endregion

        if (GalagaHelper.JustSpawned == 8)
        {
            form1.GetComponent<EnemySpawner>().isFull = true;
            Debug.Log("Form 1 Full".Colored(Colors.green).Bold());
        }
        else if (GalagaHelper.JustSpawned == 16)
        {
            form2.GetComponent<EnemySpawner>().isFull = true;
            this.secondWaveFinished = true;
            Debug.Log("Form 2 Full".Colored(Colors.green).Bold());
        }
        else if (GalagaHelper.JustSpawned == 24)
        {
            form3.GetComponent<EnemySpawner>().isFull = true;
            this.thirdWaveFinished = true;
            Debug.Log("Form 3 Full".Colored(Colors.green).Bold());
        }
        else if (GalagaHelper.JustSpawned == 32)
        {
            form4.GetComponent<EnemySpawner>().isFull = true;
            this.fourthWaveFinished = true;
            Debug.Log("Form 4 Full".Colored(Colors.green).Bold());
        }
        else if (GalagaHelper.JustSpawned == 40)
        {
            form5.GetComponent<EnemySpawner>().isFull = true;
            Debug.Log("Form 5 Full".Colored(Colors.green).Bold());
        }
        //Debug.Log(GalagaHelper.TimeToSpawn.ToString().Italics());
        //Debug.Log(GalagaHelper.CurrentRoundPhase.ToString().Bold());
        //Debug.Log("Player lives: ".Bold() + GalagaHelper.numOfPlayers.ToString().Bold());
        if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase2 && GalagaHelper.TimeToSpawn > 8.0f) // && GalagaHelper.TimeToSpawn > 11.0f
        {
            //GameObject pt2 = GameObject.FindGameObjectWithTag("phase2").gameObject;
            Debug.Log("Round2 enabled".Colored(Colors.purple));
            form2.GetComponent<EnemySpawner>().enabled = true;
            if (form2.GetComponent<EnemySpawner>().isFormationUp)
            {
                GalagaHelper.CurrentRoundPhase += 1;
            }
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase3 && secondWaveFinished && GalagaHelper.TimeToSpawn > 15f) //&& GalagaHelper.TimeToSpawn > 15
        {
            //Debug.Log("Round3 enabled".Colored(Colors.purple));
            form3.GetComponent<EnemySpawner>().enabled = true;
            Debug.Log("Round3 enabled. form3 isFormationUp: " + form3.GetComponent<EnemySpawner>().isFormationUp);
            if (form3.GetComponent<EnemySpawner>().isFormationUp)
            {
                GalagaHelper.CurrentRoundPhase += 1;
                //secondWaveFinished = false;
            }
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase4 && thirdWaveFinished && GalagaHelper.TimeToSpawn > 19f) //&& GalagaHelper.TimeToSpawn > 20
        {
            Debug.Log("Round4 enabled".Colored(Colors.purple));
            form4.GetComponent<EnemySpawner>().enabled = true;
            if (form4.GetComponent<EnemySpawner>().isFormationUp)
            {
                GalagaHelper.CurrentRoundPhase += 1;
                //thirdWaveFinished = false;
            }
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase5 && fourthWaveFinished && GalagaHelper.TimeToSpawn > 24.0f) //&& GalagaHelper.TimeToSpawn > 18
        {
            Debug.Log("Round5 enabled".Colored(Colors.purple));
            form5.GetComponent<EnemySpawner>().enabled = true;
            //
            GalagaHelper.PrintAllGhostObjects();
            Debug.Log("Deleting all ghosts.".Colored(Colors.green));
            Invoke("StartEnemyAttack", 5.0f);
            //moveFormation = true;
            fourthWaveFinished = false;
        }

        // Move formation left and right
        if (moveFormation)
        {
            #region MoveCode
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
            #endregion

            if (!GalagaHelper.isPlayerCaptured)
            {
                GalagaHelper.SetAttackinMotion();
            }
        }

        if (enemyAttacks == 0)
        {
            enemyAttacks = 1;
            isEnemy1Done = true;
            isEnemy2Done = true;
            isEnemy3Done = true;
        }

        if (enemy1Picked && isEnemy1Done == true)
        {
            Debug.Log("Enemy1 called to attack".Bold());
            PickRandomEnemyOne();
            enemy1Picked = false;
            isEnemy1Done = false;
        }

        if (enemy2Picked && isEnemy2Done == true)
        {
            Debug.Log("Enemy2 called to attack".Bold());
            PickRandomEnemyTwo();
            enemy2Picked = false;
            isEnemy2Done = false;
        }

        if (enemy3Picked && isEnemy3Done == true)
        {
            Debug.Log("Enemy3 called to attack".Bold());
            PickRandomEnemyThreeAttack();
            enemy3Picked = false;
            isEnemy3Done = false;
        }

        //Debug.Log("player captured " + GalagaHelper.isPlayerCaptured + " Starfield paused: " + starfield.isPaused);

        // If found a player captured then set ready text.
        if (GalagaHelper.isPlayerCaptured == true)
        {
            if (isReadyDone == false)
            {
                // turn on Ready text
                // Pause starfield
                if (!isPlayerReady)
                {
                    readyText.SetActive(true);
                    starfield.Pause();
                    //Debug.Log("Paused starfield");
                }
                else if (isPlayerReady) // player is ready now get rid of readyText
                {
                    readyText.SetActive(false);
                    //Debug.Log("Starfield Unpaused".Colored(Colors.green));
                    starfield.Play();
                    isPlayerReady = false;
                    isReadyDone = true;
                    GalagaHelper.isPlayerCaptured = false;
                    //GameObject player = GameObject.FindGameObjectWithTag("Player").gameObject;
                    //player.GetComponent<Renderer>().enabled = true;
                    //player.GetComponent<MeshCollider>().enabled = true;
                    isEnemy1Done = true;
                    isEnemy2Done = true;
                    isEnemy3Done = true;
                }
            }
        }
	}

    public void StartEnemyAttack()
    {
        moveFormation = true;
    }

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
