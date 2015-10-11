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
    private bool enemy3hit = false;
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
    private bool tractorFoundPlayer = false;
    private int theAngle = 32;
    private int segments = 10;
    private float distance = 20.0f;
    private Animator animator;
    private ParticleSystem tractor;
    private ParticleSystem starfield;
    [Header("Sound Settings")]
    private AudioSource audio;

    void Start()
    {
        base.Start();
        // Player gameobject used for Tractor Beam
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        playerController = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        enemyProjWall = GameObject.Find("EnemyProjectileWall").GetComponent<Transform>();
        starfield = GameObject.FindGameObjectWithTag("Starfield").GetComponent<ParticleSystem>();
        _waypoints = new List<Vector3>();
        //isAttackPlayer = true;
        origRotation = transform.rotation;
    }

    void Update()
    {
        base.Update();

        // If AttackPlayer is set then Attack with Tractor Beam
        if (isTractorBeamAttack)
        {
            if (gotOriginalPosition == false)
            {
                _originalPosition = this.transform.position;
                gotOriginalPosition = true;
            }
            TractorBeamAttack();


            // Get Enemy on Path to come from top to original position.
            //if (_isOnPath)
            //{
            //    Debug.Log("Is on path now".Bold());
            //    iTween.PutOnPath(gameObject, _waypoints.ToArray(), _pathPercentage);
            //    _pathPercentage += Time.deltaTime * 10f / 10;
            //    //Debug.Log("path Percent: " + _pathPercentage);
            //    if (_pathPercentage > 1)
            //    {
            //        _isOnPath = false;
            //        _pathPercentage = 0;
            //        outOfPlayerRange = false;
            //        gotOriginalPosition = false;
            //        isTractorBeamAttack = false;
            //    }
            //}
        }

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
                Debug.Log("Is on path now".Bold());
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
        Debug.Log(Vector3.Distance(currentPosition, targetPosition).ToString().Bold());
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
            GalagaHelper.isPlayerCaptured = true;
            Debug.Log("Player will now be captured.");
            // Get Player animation of getting captured by tractor beam and play it.
            Animation anim = player.GetComponent<Animation>();
            //Debug.Log("wrap mode: " + anim.wrapMode);
            anim.wrapMode = WrapMode.Once;
            anim.Play();
            //Debug.Log((anim["PlayerCapture"].length * anim["PlayerCapture"].speed).ToString().Colored(Colors.aqua));
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
            tractor.enableEmission = false;
            Debug.Log(_originalPosition.ToString().Colored(Colors.black));
            // Send Enemy and player back to original position after animation of tractor beam stops completely.
            Invoke("SendBackToOriginalPos", 3.2f);
        }
    }

    void SendBackToOriginalPos()
    {
        iTween.MoveTo(gameObject, _originalPosition, 2.3f);
        if (playerController.playerCaptured == true)
        {
            player.position = player.position + new Vector3(0, 0, 13);
            Renderer rend = player.GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
        }
        sweepTractorBeam = false;
        outOfPlayerRange = false;
        gotOriginalPosition = false;
        isTractorBeamAttack = false;
        // turn starfield back on
        if (starfield.isPaused == true)
        {
            starfield.Play();
            Debug.Log("Starfield should play.".Colored(Colors.red));
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
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            Renderer rend = GetComponent<Renderer>();
            rend.material.SetColor("_Color", Color.red);
            if (enemy3hit)
            {
                
            }
        }
    }

    /// <summary>
    /// Attack following player and then after getting close just go 
    /// towards the bottom of screen.
    /// </summary>
    public void Attack()
    {
        transform.LookAt(player);
        Vector3 targetPosition = player.transform.position;
        Vector3 currentPosition = this.transform.position;
        //this.isEnemyFiring = true;
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
                targetPosition = enemyProjWall.transform.position;
                Vector3 directionAfterPlayer = targetPosition - currentPosition;
                directionAfterPlayer.Normalize();
                this.transform.Translate(
                    (directionAfterPlayer.x * swoopSpeed * Time.deltaTime),
                    (directionAfterPlayer.y * swoopSpeed * Time.deltaTime),
                    (directionAfterPlayer.z * swoopSpeed * Time.deltaTime),
                    Space.World);
                transform.LookAt(enemyProjWall);
            }
            //Debug.Log(gameObject.transform.position.z.ToString().Bold());
            if (gameObject.transform.position.z < -70f)
            {
                Debug.Log("Enemy made it to wall".Bold());
                CreateIncomingPath();
                //mainForm.isEnemy2Done = true;
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

    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            iTween.DrawPathGizmos(_waypoints.ToArray());
        }
    }
} 
