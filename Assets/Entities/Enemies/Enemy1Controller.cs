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
    private List<Vector3> _waypoints;
    private float _pathPercentage = 0f;
    private bool _isOnPath = false;
    private bool _finishedPath = false;
    private Hashtable tweenPath = new Hashtable();
    private int choosePath;
    private Quaternion _originalRotation;
    private AudioSource audio;
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
        //Debug.Log("Original Pos in START: " + transform.position.ToString());
        _isOnPath = true;
        //CreatePath();
    }

    private void Update()
    {
        base.Update();

        //if (Input.GetKeyDown(KeyCode.RightControl))
        //{
        //    // attack player either come right back up or go below screen and come back on top.
        //    CreatePath();
        //}

        // if enemy has made it to last point then reappear from top
        // of screen.
        if (_finishedPath)
        {
            this.isEnemyFiring = false;
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
            player.GetCirclePath();
            Vector3[] pathToPlayer = new Vector3[9];
            pathToPlayer = player.circlePath;

            // chose random swoops
            choosePath = Random.Range(0, 2);
            //Debug.Log("Path = " + choosePath);
            // Path0 = circle then swoop back up
            // Path1 = circle then reappear on top
            if (choosePath==0)
	        {
                for (int i = 0; i < 8; i++)
                {
                    _waypoints.Add(pathToPlayer[i]);
                }
                _waypoints.Add(pathToPlayer[0]);
                Vector3[] newVect30 = new Vector3[_waypoints.Count];
                for (int i = 0; i < _waypoints.Count; i++)
                {
                    newVect30[i] = _waypoints[i];
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
                this.isEnemyFiring = true;
	        }
            else
            {
                for (int i = 0; i < 9; i++)
                {
                    _waypoints.Add(pathToPlayer[i]);
                }
                //Debug.Log("Waypoints Count: " + _waypoints.Count);
                Vector3[] newVect3 = new Vector3[_waypoints.Count];
                //Debug.Log(_waypoints.Count.ToString().Bold().Italics());
                for (int i = 0; i < _waypoints.Count; i++)
                {
                    newVect3[i] = _waypoints[i];
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
                this.isEnemyFiring = true;
            }
        }

    }

    public void CreatePathForEnemy4Trio()
    {
        //isNotInFormation = true;
        PlayerController player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
        tweenPath.Clear();
        if (player)
        {
            _waypoints.Clear();
            _waypoints.Add(transform.position);
            _originalPosition = transform.position;
            //Debug.Log(_waypoints[0].ToString().Bold());
            player.GetCirclePath();
            Vector3[] pathToPlayer = new Vector3[9];
            pathToPlayer = player.circlePath;
            
            for (int i = 0; i < 9; i++)
            {
                _waypoints.Add(pathToPlayer[i]);
            }
            //Debug.Log("Waypoints Count: " + _waypoints.Count);
            Vector3[] newVect3 = new Vector3[_waypoints.Count];
            //Debug.Log(_waypoints.Count.ToString().Bold().Italics());
            for (int i = 0; i < _waypoints.Count; i++)
            {
                newVect3[i] = _waypoints[i];
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
    }

    public void CircleComplete()
    {
        _finishedPath = true;
        transform.rotation = _originalRotation;
        //Debug.Log("Enemy completed circle".Bold().Italics());
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Something hit an enemy");
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Debug.Log("Enemy hit!".Bold().Colored(Colors.red));

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
                top = base.addShotSounds(explosionTop[Random.Range(0, explosionTop.Length)], Random.Range(0.8f, 1.2f));
                bottom = base.addShotSounds(explosionBottom, Random.Range(0.8f, 1.2f));
                top.PlayScheduled(0.3);
                bottom.Play();
                rend.enabled = false;
                GameObject explosionPrefab = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                Destroy(explosionPrefab, 3.0f);
                Invoke("DisableEnemy", top.clip.length);
                GalagaHelper.EnemiesKilled += 1;
                if (base.isRandomPicked == true)
                {
                    isRandomPicked = false;
                    main.isEnemy1Done = true;
                }
            }
        }
    }

    void DisableEnemy()
    {
        SimplePool.Despawn(gameObject);
        gameObject.transform.parent = null;
    }

    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            iTween.DrawPathGizmos(_waypoints.ToArray());
        }
    }
}
