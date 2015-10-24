using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy3Controller : EnemyController
{
    [Header("Enemy 3 Settings")]
    public float swoopSpeed;
    public bool isTractorBeamAttack = false;
    public bool isAttackPlayer = false;
    public GameObject tractorBeam;
    private PlayerController playerController;
    private Transform player;
    private bool outOfPlayerRange = false;
    [Header("Path from Top of Screen Settings")]
    public Vector3 _originalPosition;
    private List<Vector3> _waypoints;
    private bool _isOnPath = false;
    private float _pathPercentage = 0f;
    private bool gotOriginalPosition = false;
    private Quaternion origRotation;
    private Transform enemyProjWall;
    [Header("Tractor Beam Settings")]
    public bool sweepTractorBeam = false;
    public GameObject playerNew;
    private bool tractorFoundPlayer = false;
    private int theAngle = 32;
    private int segments = 10;
    private float distance = 20.0f;
    private Animator animator;
    private ParticleSystem tractor;
    private ParticleSystem starfield;
    private bool isSentBack = false;
    private GameObject playerSpawn;
    private Color matColor;
    [Header("Sound Settings")]
    private AudioSource audio;
    private GameObject gameManager;

    void Start()
    {
        base.Start();
        // Player gameobject used for Tractor Beam
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        enemyProjWall = GameObject.Find("EnemyProjectileWall").GetComponent<Transform>();
        starfield = GameObject.FindGameObjectWithTag("Starfield").GetComponent<ParticleSystem>();
        gameManager = GameObject.Find("GameManager");
        playerSpawn = GameObject.Find("PlayerSpawn");
        _waypoints = new List<Vector3>();
        //isAttackPlayer = true;
        origRotation = transform.rotation;
    }

    void Update()
    {
        base.Update();

        // If AttackPlayer is set then Attack with Tractor Beam after getting
        // the orginal position.
        if (isTractorBeamAttack)
        {
            if (gotOriginalPosition == false)
            {
                _originalPosition = this.transform.position;
                gotOriginalPosition = true;
            }
            TractorBeamAttack();
        }

        // Normal attack #1.
        if (isAttackPlayer)
        {
            if (gotOriginalPosition == false)
            {
                _originalPosition = this.transform.position;
                gotOriginalPosition = true;
            }

            Attack();
            // Get Enemy on Path to come from top to original position.
            if (_isOnPath)
            {
                //Debug.Log("Is on path now".Bold());
                iTween.PutOnPath(gameObject, _waypoints.ToArray(), _pathPercentage);
                _pathPercentage += Time.deltaTime * 10f / 10;
                //Debug.Log("path Percent: " + _pathPercentage);
                if (_pathPercentage > 1)
                {
                    _isOnPath = false;
                    _pathPercentage = 0;
                    outOfPlayerRange = false;
                    gotOriginalPosition = false;
                    isAttackPlayer = false;
                    isNotInFormation = false;
                }
            }
        }

        // If Tractor beam is set then do raycast Sweep looking for player.
        if (sweepTractorBeam)
        {
            RaycastSweep();
        }
    }

    /// <summary>
    /// Tractor Beam attack.
    /// Moves toward player and when 45.0f away it stops and sets in motion Particle system.
    /// Sets bool sweepTractorBeam to true if tractor beam is on so to use the Raycast Sweep function.
    /// </summary>
    public void TractorBeamAttack()
    {
        isNotInFormation = true;
        Transform enemyProjWall = GameObject.Find("EnemyProjectileWall").GetComponent<Transform>();
        if (enemyProjWall)
        {
            transform.LookAt(enemyProjWall);
        }
        //transform.LookAt(player);
        Vector3 targetPosition = enemyProjWall.position;
        Vector3 currentPosition = this.transform.position;
        // Set enemy to fire.
        //this.isEnemyFiring = true;

        //first, check to see if we're close enough to the target -- 45.0f for player
        //Debug.Log(Vector3.Distance(currentPosition, targetPosition).ToString().Bold());
        if (Vector3.Distance(currentPosition, targetPosition) > 83.0f && outOfPlayerRange == false)
        {
            Vector3 directionOfTravel = targetPosition - currentPosition;
            //now normalize the direction, since we only want the direction information
            directionOfTravel.Normalize();
            // Play swoop sound
            if (currentPosition == _originalPosition)
            {
                //input sound
                audio = base.addShotSounds(swooshSound, 1.0f);
                audio.Play();
                Debug.Log("Sound swoop played".Colored(Colors.red));
            }
            //scale the movement on each axis by the directionOfTravel vector components
            this.transform.Translate(
                (directionOfTravel.x * swoopSpeed * Time.deltaTime),
                (directionOfTravel.y * swoopSpeed * Time.deltaTime),
                (directionOfTravel.z * swoopSpeed * Time.deltaTime),
                Space.World);
        }
        else
        {
            // After enemy gets close enough then Clone TractorBeam Particle system.
            outOfPlayerRange = true;
            transform.rotation = origRotation;
            //Debug.Log("Reached the player range.");
            // Stop Moving and set tractor beam
            Vector3 offset = new Vector3(0, 0, -3.5f);
            GameObject tractorBeamGO = Instantiate(tractorBeam, gameObject.transform.position + offset, origRotation) as GameObject;
            tractor = tractorBeamGO.GetComponent<ParticleSystem>();
            tractor.enableEmission = true;
            if (!tractor.isPlaying)
            {
                tractor.Play();
                //Enable raycast sweep to be made after 3 seconds
                Invoke("EnableTractorBeamSweep", 2.0f);
                Invoke("SendBackToOriginalPos", 10.5f);
                //sweepTractorBeam = true;
            }
            isTractorBeamAttack = false;
        }

    }

    /// <summary>
    /// Enable raycasts to search for player after 3 seconds
    /// </summary>
    void EnableTractorBeamSweep()
    {
        sweepTractorBeam = true;
    }

    /// <summary>
    /// Use raycast sweep to see if the player touches the Tractor beam.
    /// </summary>
    void RaycastSweep()
    {
        // Set the target as straight down offset
        Vector3 target = new Vector3(transform.position.x, transform.position.y, transform.position.z - 15f);
        Vector3 current = transform.position;
        //Vector3 fwd = transform.TransformDirection(Vector3.forward)
        Vector3 direct = target - current;
        Debug.DrawLine(current, direct, Color.yellow);

        //Debug.Log("Raycast sweep init.");
        // set start point of the raycasts
        Vector3 offset = new Vector3(0, 0, -3f);
        Vector3 startPos = transform.position + offset;
        Vector3 targetPos = Vector3.zero;

        int startAngle = (int)(-theAngle * 0.5);
        int finishAngle = (int)(theAngle * 0.5);

        int inc = (int)(theAngle / segments);
         
        RaycastHit hit;
        //Debug.Log("Start angle: " + startAngle + " Finish Angle: " + finishAngle);

        // Create sweep of raycasts to find player object.
        for (int i = startAngle; i < finishAngle; i+= inc)
        {
            targetPos = (Quaternion.Euler(0, i, 0) * direct) * distance;
            //Debug.Log(transform.forward.ToString().Bold());
            if (Physics.Linecast(startPos, targetPos, out hit))
            {
                if (hit.collider.gameObject.name == "Player")
                {
                    tractorFoundPlayer = true;
                    gameManager.transform.position = transform.position + new Vector3(0, 0, -10.5f);
                    player.gameObject.transform.parent = gameManager.transform;
                }
            }
            Debug.DrawLine(startPos, targetPos, Color.green);
        }

        if (tractorFoundPlayer)
        {
            sweepTractorBeam = false;
            tractorFoundPlayer = false;
            playerController.playerCaptured = true;
            player.gameObject.tag = "CapturedPlayer";

            
            Debug.Log("Player will now be captured.");
            // Get Player animation of getting captured by tractor beam and play it.
            Animation anim = player.GetComponent<Animation>();
            //Debug.Log("wrap mode: " + anim.wrapMode);
            anim.wrapMode = WrapMode.Once;
            anim.Play();
            // Set Parent of Player equal to enemy and turn off tractor beam in SetParentAfterCapture function.
            Invoke("SetParentAfterCapture", (anim["PlayerCapture"].length * anim["PlayerCapture"].speed) + 0.5f);
        }
    }

    /// <summary>
    /// Set Parent for Player because captured then move back up 
    /// to last position.
    /// </summary>
    void SetParentAfterCapture()
    {
        // Set Player's parent to the Enemy.
        player.parent = transform;
        // Stop Tractor beam
        if (tractor)
        {
            Debug.Log("Path set in motion");
            Renderer rend = player.GetComponent<Renderer>();
            matColor = rend.material.color;
            //Debug.Log("Original mat color: " + matColor.ToString().Bold());
            rend.material.SetColor("_Color", Color.red);
            //rend.material.
            tractor.enableEmission = false;
            Debug.Log(_originalPosition.ToString().Colored(Colors.black));
            // Send Enemy and player back to original position after animation of tractor beam stops completely.
            Invoke("SendBackToOriginalPos", 3.2f);
        }
    }

    void SendBackToOriginalPos()
    {
        if (!isSentBack)
        {
            isSentBack = true;
            iTween.MoveTo(gameObject, _originalPosition, 2.3f);
            if (playerController.playerCaptured == true)
            {
                GalagaHelper.isPlayerCaptured = true;
                main.isReadyDone = false;
                GalagaHelper.numOfPlayers += 1;
                GalagaHelper.PlacePlayerIcons();
                player.position = player.position + new Vector3(0, 0, 11.5f);
                //playerController.enabled = false;
                CreateNewPlayer();
            }
            sweepTractorBeam = false; // Turn off raycast sweep
            outOfPlayerRange = false; // this sets the tractor beam in place
            gotOriginalPosition = false; // first position of enemy3
            isTractorBeamAttack = false; // Tractor beam attack setup
            isNotInFormation = false; // set for getting scorevalues and for ??
        }
    }

    void CreateNewPlayer()
    {
        GameObject play =  Instantiate(playerNew, playerSpawn.transform.position, playerSpawn.transform.rotation) as GameObject;
        player = play.gameObject.transform;
        playerController = play.GetComponent<PlayerController>();
        // Stop firing and then resume back on again later??
        Invoke("ResumeGame", 4.0f);
    }

    void ResumeGame()
    {
        if (starfield.isPaused == true && GalagaHelper.isPlayerCaptured == true)
        {
            main.isPlayerReady = true;
        }
    }

    /// <summary>
    /// Creates path for enemy to come back to original position from top of screen.
    /// </summary>
    private void CreateIncomingPath()
    {
        _waypoints.Clear();

        Vector3 topSide = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        _waypoints.Add(new Vector3(-1.289417f, 0, topSide.z));
        _waypoints.Add(_originalPosition);
        _isOnPath = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        Debug.Log("Bullet hit enemy3.");
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);

            if (base.isRandomPicked == true)
            {
                isRandomPicked = false;
                main.isEnemy3Done = true;
            }
            if (health <= 0)
            {
                if (isNotInFormation)
                {
                    scoreKeeper.Score(400);
                }
                else
                {
                    scoreKeeper.Score(150);
                }

                top = base.addShotSounds(base.explosionTop[Random.Range(0, explosionTop.Length)], Random.Range(0.8f, 1.2f));
                bottom = base.addShotSounds(base.explosionBottom, Random.Range(0.8f, 1.2f));
                top.PlayScheduled(0.3);
                bottom.Play();
                rend.enabled = false;
                meshcol.enabled = false;
                GameObject explosionPrefab = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                Destroy(explosionPrefab, 3.0f);
                Debug.Log("Enemy3 killed: " + gameObject.name.Colored(Colors.blue) + " SpawnDisableTime: " + spawnDisableTime);
                GalagaHelper.DisabledEnemies += 1;
                SimplePool.Despawn(gameObject);
                Invoke("DisableEnemy", spawnDisableTime);
                GalagaHelper.EnemiesKilled += 1;
                // Check if there is a captured player.
                if (HaveChild())
                {
                    // Set in motion extra player
                    // Rotate in place then move to center along with currentplayer and then combine both.
                    Transform child = this.transform.GetChild(0);
                    child.parent = null;
                    PlayerController capturedPlayer = GameObject.FindGameObjectWithTag("CapturedPlayer").GetComponent<PlayerController>();
                    // Rotate captured player.
                    capturedPlayer.rotatePlayer = true;
                    // Move back to playerSpawn.
                    iTween.MoveTo(child.gameObject, main.transform.position, 2.0f);
                    iTween.MoveTo(child.gameObject, playerSpawn.transform.position, 2.0f);
                    // Turn off rotation.
                    capturedPlayer.rotatePlayer = false;
                    // Move current player next to captured player
                    // and turn off captured player bool. Set captured
                    // player back to original color.
                    iTween.MoveTo(player.gameObject, playerSpawn.transform.position + new Vector3(-5.3f, 0, 0), 1.5f);
                    capturedPlayer.playerCaptured = false;
                    capturedPlayer.tag = "Player";
                    Renderer newPlayerRend = capturedPlayer.GetComponent<Renderer>();
                    newPlayerRend.material.SetColor("_Color", matColor);
                }
                if (base.isRandomPicked == true)
                {
                    isRandomPicked = false;
                    main.isEnemy3Done = true;
                }
            }
        }
    }

    void DisableEnemy()
    {
        Debug.Log("Disabled Enemy3 called".Colored(Colors.navy) + " SpawnDisableTime: " + spawnDisableTime);
        gameObject.transform.parent = null;
    }

    public bool HaveChild()
    {
        try
        {
            if (transform.GetChild(0).childCount > 0)
            {
                return true;
            }
            else
            {
                return false;
            }

        }
        catch (System.Exception)
        {
            return false;
            //throw;
        }
    }

    /// <summary>
    /// Attack following player and then after getting close just go 
    /// towards the bottom of screen.
    /// </summary>
    public void Attack()
    {
        isNotInFormation = true;
        if (!player)
        {
           player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        }
        transform.LookAt(player);
        Vector3 targetPosition = player.transform.position;
        Vector3 currentPosition = this.transform.position;
        this.isEnemyFiring = true;
        //first, check to see if we're close enough to the target
        if (Vector3.Distance(currentPosition, targetPosition) > 24.0f && outOfPlayerRange == false)
        {
            Vector3 directionOfTravel = targetPosition - currentPosition;
            //now normalize the direction, since we only want the direction information
            directionOfTravel.Normalize();
            //scale the movement on each axis by the directionOfTravel vector components
            // Play swoop sound
            if (currentPosition == _originalPosition)
            {
                //input sound
                audio = base.addShotSounds(swooshSound, 1.0f);
                audio.Play();
                Debug.Log("Sound swoop played".Colored(Colors.red));
            }
            this.transform.Translate(
                (directionOfTravel.x * swoopSpeed * Time.deltaTime),
                (directionOfTravel.y * swoopSpeed * Time.deltaTime),
                (directionOfTravel.z * swoopSpeed * Time.deltaTime),
                Space.World);
        }
        else
        {
            // Is away from player now and set new target as enemyWall
            outOfPlayerRange = true;
            if (outOfPlayerRange)
            {
                this.isEnemyFiring = false;
                //Debug.Log("Not close anymore to player".Colored(Colors.red));
                targetPosition = GalagaHelper.Enemy2PathEnd;
                Vector3 directionAfterPlayer = targetPosition - currentPosition;
                directionAfterPlayer.Normalize();
                this.transform.Translate(
                    (directionAfterPlayer.x * swoopSpeed * Time.deltaTime),
                    (directionAfterPlayer.y * swoopSpeed * Time.deltaTime),
                    (directionAfterPlayer.z * swoopSpeed * Time.deltaTime),
                    Space.World);
                transform.LookAt(GalagaHelper.Enemy2LookAtTransform);
            }
            //Debug.Log(gameObject.transform.position.z.ToString().Bold());
            if (gameObject.transform.position.z < -70f)
            {
                Debug.Log("Enemy made it to wall".Bold());
                CreateIncomingPath();
                //mainForm.isEnemy2Done = true;
                main.isEnemy3Done = true;
            }
        }

        if (_isOnPath)
        {
            //Debug.Log("Is on path now".Bold());
            iTween.PutOnPath(gameObject, _waypoints.ToArray(), _pathPercentage);
            _pathPercentage += Time.deltaTime * 10f / 10;
            //Debug.Log("path Percent: " + _pathPercentage);
            if (_pathPercentage > 1)
            {
                _isOnPath = false;
                //_finishedPath = false;
                _pathPercentage = 0;
                outOfPlayerRange = false;
                gotOriginalPosition = false;
                isAttackPlayer = false;
                //transform.rotation = _originalRotation;
            }
        }
    }
    void OnDisable()
    {
        Debug.Log("Disabled Enemy3 called".Colored(Colors.navy) + " SpawnDisableTime: " + spawnDisableTime);
        Debug.Log("Disabled Enemy: " + gameObject.name.Colored(Colors.red));
    }

    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            iTween.DrawPathGizmos(_waypoints.ToArray());
        }
    }
} 
