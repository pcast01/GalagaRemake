using System.Collections.Generic;
using System.Collections;
using UnityEngine;

public class Enemy1Controller : EnemyController
{
    [Header("Enemy 1 Settings")]
    public float returnPathSpeed = 10f;
    public Transform PlayerTransform;
    public GameObject enemy4Prefab;
    public Vector3 _originalPosition;
    public bool startScorpionAttack;
    private List<Vector3> _waypoints;
    private float _pathPercentage = 0f;
    private bool _isOnPath = false;
    private bool _finishedPath = false;
    private Hashtable tweenPath = new Hashtable();
    private int choosePath;
    private Quaternion _originalRotation;
    private AudioSource audio;
    private GameObject form1;
    [SerializeField]
    private float swoopDownSpeed;

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
        form1 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        //Debug.Log("Original Pos in START: " + transform.position.ToString());
        _isOnPath = true;
        //CreatePath();
    }

    private void Update()
    {
        base.Update();

        if (startScorpionAttack)
        {
            GalagaHelper.isScorpionAttackOn = true;
            form1.GetComponent<EnemySpawner>().CreateEnemy4Trio(this.transform, this.transform.parent.transform);
            GetComponent<Renderer>().enabled = false;
            gameObject.transform.parent = null;
            SimplePool.Despawn(gameObject);
            GalagaHelper.StartScorpionPaths();
            startScorpionAttack = false;
        }

        // if enemy has made it to last point then reappear from top
        // of screen.
        if (_finishedPath)
        {
            //this.isEnemyFiring = false;
            CreateIncomingPath();

            if (_isOnPath)
            {
                iTween.PutOnPath(gameObject, _waypoints.ToArray(), _pathPercentage);
                _pathPercentage += Time.deltaTime * returnPathSpeed / 10;
                //Debug.Log("path Percent: " + _pathPercentage);
                if (_pathPercentage > 1)
                {
                    _isOnPath = false;
                    _finishedPath = false;
                    _pathPercentage = 0;
                    transform.rotation = _originalRotation;
                    main.isEnemy1Done = true;
                    isNotInFormation = false;
                }
            }
        }
    }

    // Test path for ondrawgizmos
    private void path()
    {
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        _waypoints.Add(_originalPosition);
        player.GetCirclePath();
        Vector3[] pathToPlayer = new Vector3[9];
        pathToPlayer = player.circlePath;
        for (int i = 0; i < 8; i++)
        {
            _waypoints.Add(pathToPlayer[i]);
        }
        _waypoints.Add(pathToPlayer[0]);
    }

    /// <summary>
    /// Create path based on points around player.
    /// </summary>
    public void CreatePath()
    {
        isNotInFormation = true;
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        tweenPath.Clear();
        if (player)
        {
            _waypoints.Clear();
            _waypoints.Add(transform.position);
            _originalPosition = transform.position;
            //Debug.Log(_waypoints[0].ToString().Bold());
            player.GetCirclePathScorpions();
            Vector3[] pathToPlayer = new Vector3[5];
            pathToPlayer = player.scorpionCirclePath;

            // chose random swoops
            choosePath = GalagaHelper.RandomNumber(0, 2);
            switch (choosePath)
            {
                case 0:
                    Debug.Log("Path: Circle Player then swoop back up.".Colored(Colors.purple));
                    break;
                case 1:
                    Debug.Log("Path: Circle Player then re-appear on top.".Colored(Colors.red));
                    break;
                default:
                    Debug.Log("Path: other than 0 or 1 picked.".Colored(Colors.purple));
                    break;
            }
            // Path0 = circle then swoop back up
            // Path1 = circle then reappear on top
            if (choosePath==0)
	        {
                //_waypoints.Add(GameObject.FindGameObjectWithTag("Point2").GetComponent<Transform>().position);
                //for (int i = 0; i < 8; i++)
                for (int i = 0; i < 5; i++)
                {
                    _waypoints.Add(pathToPlayer[i]);
                }
                _waypoints.Add(pathToPlayer[0]);
                //_waypoints.Add(GameObject.FindGameObjectWithTag("Point2").GetComponent<Transform>().position);
                Vector3[] newVect30 = new Vector3[_waypoints.Count];
                for (int i = 0; i < _waypoints.Count; i++)
                {
                    newVect30[i] = _waypoints[i];
                    Debug.Log(gameObject.name.ToString().Bold() + " Path Swoop " + i + ": " + newVect30[i].ToString().Bold());
                }

                // _waypoints.Add(_originalPosition);
                tweenPath.Add("path", newVect30);
                tweenPath.Add("time", swoopDownSpeed);
                tweenPath.Add("easetype", "linear");
                tweenPath.Add("orienttopath", true);
                tweenPath.Add("onComplete", "Path1Complete");
                tweenPath.Add("onCompleteTarget", gameObject);
                iTween.MoveFrom(gameObject, tweenPath);
                audio = base.addShotSounds(swooshSound, 1.0f);
                audio.Play();
                //this.isEnemyFiring = true;
	        }
            else
            {
                this.isEnemyFiring = true;
                _waypoints.Add(GameObject.FindGameObjectWithTag("Point2").GetComponent<Transform>().position);
                //for (int i = 0; i < 9; i++)
                for (int i = 0; i < 5; i++)
                {
                    _waypoints.Add(pathToPlayer[i]);
                }
                //_waypoints.Add(GameObject.FindGameObjectWithTag("Point2").GetComponent<Transform>().position);
                //Debug.Log("Waypoints Count: " + _waypoints.Count);
                Vector3[] newVect3 = new Vector3[_waypoints.Count];
                //Debug.Log(_waypoints.Count.ToString().Bold().Italics());
                for (int i = 0; i < _waypoints.Count; i++)
                {
                    newVect3[i] = _waypoints[i];
                    Debug.Log(gameObject.name.ToString().Bold() + " Path reappear on top " + i + ": " + newVect3[i].ToString().Bold());
                }
                tweenPath.Add("path", newVect3);
                tweenPath.Add("time", swoopDownSpeed);
                tweenPath.Add("easetype", "linear");
                tweenPath.Add("orienttopath", true);
                tweenPath.Add("onComplete", "CircleComplete");
                tweenPath.Add("onCompleteTarget", gameObject);
                iTween.MoveTo(gameObject, tweenPath);
                audio = base.addShotSounds(swooshSound, 1.0f);
                audio.Play();
                //this.isEnemyFiring = true;
            }
        }

    }

    private void CreateIncomingPath()
    {
        Vector3 lastPoint = _waypoints[_waypoints.Count - 1];
        _waypoints.Clear();
        Vector3 topSide = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        _waypoints.Add(new Vector3(lastPoint.x, 0, topSide.z));
        _waypoints.Add(_originalPosition);
        _isOnPath = true;
    }

    public void Path1Complete()
    {
        transform.rotation = _originalRotation;
        Debug.Log("Path 1 Enemy completed,".Bold().Italics());
    }

    public void CircleComplete()
    {
        this.isEnemyFiring = false;
        _finishedPath = true;
        transform.rotation = _originalRotation;
        //Debug.Log("Enemy completed circle".Bold().Italics());
    }

    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something hit an enemy1".Colored(Colors.navy));
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Debug.Log(gameObject.name.Colored(Colors.red).Bold() + " Enemy hit!".Bold().Colored(Colors.red));
            // BEE: if formation = 50 points, diving == 100
            if (isNotInFormation)
            {
                scoreKeeper.Score(100);
            }
            else
            {
                scoreKeeper.Score(50);
            }

            if (health <= 0)
            {
                 
                top = base.addShotSounds(explosionTop[GalagaHelper.RandomNumber(0, explosionTop.Length)], Random.Range(0.8f, 1.2f));
                bottom = base.addShotSounds(explosionBottom, Random.Range(0.8f, 1.2f));
                top.PlayScheduled(0.3);
                bottom.Play();
                rend.enabled = false;
                meshcol.enabled = false;
                GameObject explosionPrefab = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                Destroy(explosionPrefab, 3.0f);
                //Debug.Log("Enemy1 killed: " + gameObject.name.Colored(Colors.blue) + " Parent: " + gameObject.transform.parent.parent.name.Colored(Colors.blue) + " Position: " + gameObject.transform.parent.name.Colored(Colors.blue));
                this.isEnemyFiring = false;
                DisableEnemy();
                //Invoke("DisableEnemy", spawnDisableTime);
                GalagaHelper.EnemiesKilled += 1;
                if (base.isRandomPicked == true)
                {
                    isRandomPicked = false;
                    main.isEnemy1Done = true;
                }
                iTween onTween = gameObject.GetComponent<iTween>();
                if (onTween)
                {
                    if (onTween.isRunning)
                    {
                        Debug.Log("Enemy1 Killed during Itween".Colored(Colors.red).Bold());
                        //onTween.isRunning = false;
                        GalagaHelper.EnemiesSpawned += 1;
                    }
                }
                if (startScorpionAttack)
                {
                    startScorpionAttack = false;
                }
                SimplePool.Despawn(gameObject);
            }
        }
    }

    void DisableEnemy()
    {
        Debug.Log("Runaway from parent Enemy1 called".Colored(Colors.navy) + " SpawnDisableTime: " + spawnDisableTime);
        gameObject.transform.parent = null;
    }

    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            iTween.DrawPathGizmos(_waypoints.ToArray());
        }
    }

    void OnDisable()
    {
        Debug.Log("Disabled Enemy1: " + gameObject.name.Colored(Colors.red));
        GalagaHelper.DisabledEnemies += 1;
        //if (gameObject.transform.parent != null)
        //{
        //    gameObject.transform.parent = null;
        //}
    }
}
