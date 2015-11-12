using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Enemy2Controller : EnemyController
{
    [Header("Enemy 2 Settings")]
    public float swoopSpeed;
    public GameObject leftSwoop;
    public GameObject rightSwoop;
    public bool AttackPlayer = false;
    private Hashtable tweenPath = new Hashtable();
    private Vector3 pos;
    private Vector3 axis;
    private Transform enemyProjWall;
    private Transform player;
    private bool outOfPlayerRange = false;
    [Header("Path from Top of Screen Settings")]
    public Vector3 _originalPosition;
    private Quaternion _originalRotation;
    private bool gotOriginalPosition = false;
    private List<Vector3> _waypoints;
    private AudioSource audio;
    public AudioClip attackSound;
    private bool _isOnPath = false;
    private float _pathPercentage = 0f;

    private void Awake()
    {
        // Save the first transform so we can use that to create our path.
        _originalRotation = transform.rotation;
        _waypoints = new List<Vector3>();
        //Debug.Log("Original Pos: " + transform.position.ToString());
    }

    private void Start() 
    {
        base.Start();
        if (meshcol.enabled == false)
        {
            meshcol.enabled = true;
        }
        if (rend.enabled == false)
        {
            rend.enabled = true;
        }
        leftSwoop = GameObject.FindGameObjectWithTag("enemy2Left");
        rightSwoop = GameObject.FindGameObjectWithTag("enemy2Right");
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        enemyProjWall = GameObject.Find("EnemyProjectileWall").GetComponent<Transform>();
        _waypoints = new List<Vector3>();
        pos = transform.position;
        axis = transform.right;
	}

    private void Update()
    {
        base.Update();
        if (AttackPlayer)
        {
            if (gotOriginalPosition == false)
            {
                _originalPosition = this.transform.position;
                gotOriginalPosition = true;
            }
            Attack();
        }
    }

    private void CreateIncomingPath()
    {
        _waypoints.Clear();

        Vector3 topSide = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        _waypoints.Add(new Vector3(-1.289417f, 0, topSide.z));
        _waypoints.Add(_originalPosition);
        _isOnPath = true;
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
            try
            {
                player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>(); ;

            }
            catch (System.Exception)
            {
                Debug.Log("Player not present");
            }
        }
        transform.LookAt(player);
        Vector3 targetPosition = player.transform.position;
        Vector3 currentPosition = this.transform.position;
        this.isEnemyFiring = true;
        //first, check to see if we're close enough to the target
        if (Vector3.Distance(currentPosition, targetPosition) > 15.0f && outOfPlayerRange == false)
        {
            Vector3 directionOfTravel = targetPosition - currentPosition;
            //now normalize the direction, since we only want the direction information
            directionOfTravel.Normalize();
            //scale the movement on each axis by the directionOfTravel vector components
            // Play swoop sound
            if (currentPosition == _originalPosition)
            {
                //input sound
                audio = base.addShotSounds(attackSound, 1.0f);
                audio.Play();
                //Debug.Log("Sound swoop played".Colored(Colors.red));
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
                //Debug.Log("Enemy made it to wall".Bold());
                CreateIncomingPath();
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
                _pathPercentage = 0;
                outOfPlayerRange = false;
                gotOriginalPosition = false;
                AttackPlayer = false;
                isNotInFormation = false;
                main.isEnemy2Done = true;
                transform.rotation = _originalRotation;
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Something hit an enemy");
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Debug.Log(gameObject.name.Colored(Colors.red).Bold() + " Enemy hit!".Bold().Colored(Colors.red));

            // Butterfly: if formation = 80 points, diving == 160
            if (isNotInFormation)
            {
                scoreKeeper.Score(160);
            }
            else
            {
                scoreKeeper.Score(80);
            }

            if (health <= 0)
            {
                top = base.addShotSounds(explosionTop[Random.Range(0, explosionTop.Length)], Random.Range(0.8f, 1.2f));
                bottom = base.addShotSounds(explosionBottom, Random.Range(0.8f, 1.2f));
                top.PlayScheduled(0.3);
                bottom.Play();
                rend.enabled = false;
                meshcol.enabled = false;
                GameObject explosionPrefab = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                Destroy(explosionPrefab, 3.0f);
                //Debug.Log("Enemy2 killed: " + gameObject.name.Colored(Colors.blue) + " Parent: " + gameObject.transform.parent.parent.name.Colored(Colors.blue)+ " Position: " + gameObject.transform.parent.name.Colored(Colors.blue));
                this.isEnemyFiring = false;
                RunawayFromParent();
                GalagaHelper.EnemiesKilled += 1;
                if (base.isRandomPicked == true)
                {
                    isRandomPicked = false;
                    main.isEnemy2Done = true;
                }
                iTween onTween = gameObject.GetComponent<iTween>();
                if (onTween)
                {
                    if (onTween.isRunning)
                    {
                        Debug.Log("Enemy2 Killed during Itween".Colored(Colors.red).Bold());
                        GalagaHelper.EnemiesSpawned += 1;
                    }
                }
                SimplePool.Despawn(gameObject);
            }
        }
    }

    void RunawayFromParent()
    {
        Debug.Log("Runaway from parent Enemy2 called".Colored(Colors.navy) + " SpawnDisableTime: " + spawnDisableTime);
        gameObject.transform.parent = null;
    }

    void OnDisable()
    {
        Debug.Log("Disabled Enemy2: " + gameObject.name.Colored(Colors.red));
        GalagaHelper.DisabledEnemies += 1;
    }
}
