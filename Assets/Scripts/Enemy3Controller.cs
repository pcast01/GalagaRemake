using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy3Controller : EnemyController {


    private enum State
    {
        Idle,
        NormalFlying,
        StealFlying,
        Returning,
        Stealing
    }

    public GameObject playerObject;
    public GameObject playerShipPrefab;
    public float pathSpeed = 10f;
    public float heightOffset = 25f;

    private List<Vector3> _wayPoints = new List<Vector3>();

    private Vector3 _originalPosition;
    private State _state = State.Idle;
    private Hashtable _myTween = new Hashtable();
    private bool _hasPlayerShip = false;
    private float _xMin;
    private float _xMax;
    void Awake() 
    {
        _xMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        _xMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
       
    }
	void Start () 
    {
        base.Start();
        _originalPosition = transform.position;
       
	}
	
	
	void Update () 
    {
        if (_state == State.Idle)
        {
            base.Update();

            // Handling the states ---- These keys are just for debugging purposes
            if (Input.GetKeyDown(KeyCode.K))
            {
                _state = State.NormalFlying;
                CreatePath(_state);
                iTween.MoveTo(gameObject, _myTween);
            }
            if (Input.GetKeyDown(KeyCode.L) && !_hasPlayerShip)
            {
                _state = State.StealFlying;
                CreatePath(_state);
                iTween.MoveTo(gameObject, _myTween);
            }
        }
        else if (_state == State.Stealing)
        {
            Vector3 midpointRay = new Vector3(transform.position.x,0,playerObject.transform.position.z);
            float xOffset = 15f;

            Ray ray = new Ray(midpointRay - new Vector3(xOffset,0,0),new Vector3(1,0,0));
            RaycastHit info;
            if (Physics.Raycast(ray, out info, 2 * xOffset))
            {
                Vector3 desiredPosition = transform.position + new Vector3(0, 0, 5);
                Quaternion desiredRotation = Quaternion.Euler(0,180,0);
                GameObject clone = Instantiate(playerShipPrefab, playerObject.transform.position,playerObject.transform.rotation) as GameObject;
                Destroy(playerObject);

                clone.transform.parent = transform;
                _state = State.Returning;
                CreatePath(_state);
                _hasPlayerShip = true;

                Hashtable playerTween = new Hashtable();
                Vector3[] waypoints = new Vector3[2];
                waypoints[0] = clone.transform.position;
                waypoints[1] = desiredPosition;
                playerTween.Add("easetype", "linear");
                playerTween.Add("speed", 120);
                playerTween.Add("path", waypoints);
                iTween.MoveTo(clone, playerTween);
                iTween.MoveTo(gameObject, _myTween);
            }

        }
        else
        {
            base.FireRandom();
        }
        
        
        
	}

    // Creates the desired path according to the state
    private void CreatePath(State state)
    {
        _myTween.Clear();

        
        switch (state)
        {
            case State.Idle:
                break;
            case State.NormalFlying:
                _originalPosition = transform.position;
                CreateNormalFlyingPath();
                break;
            case State.StealFlying:
                _originalPosition = transform.position;
                CreateStealFlyingPath();
                break;
            case State.Returning:
                CreateReturnFlyingPath();
                _myTween.Add("delay", 1);
                break;
            case State.Stealing:
                break;
            default:
                break;
        }
        _myTween.Add("path", _wayPoints.ToArray());
        _myTween.Add("speed", pathSpeed);
        _myTween.Add("oncomplete", "OnCompleteTween");
        _myTween.Add("easetype", "linear");
    }
    // Method that calculates the trajectory for the finding the player
    private void CreateStealFlyingPath()
    {
        
        _wayPoints.Add(transform.position);
        Vector3 endPoint = playerObject.transform.position + new Vector3(0, 0, heightOffset);
        _wayPoints.Add(endPoint);
    }
    // Method that calculates the normal flying path based on offset
    private void CreateNormalFlyingPath()
    {
        _wayPoints.Add(transform.position);

        Vector3 point = new Vector3(_xMin + Random.Range(10, 60),transform.position.y,Random.Range(9f,15f));
        _wayPoints.Add(point);
        point = new Vector3(_xMax - Random.Range(10, 60), transform.position.y, Random.Range(-7f, 6f));
        _wayPoints.Add(point);
        point = new Vector3(_xMin + Random.Range(10, 60), transform.position.y, -35);
        _wayPoints.Add(point);

    }
    // Method that calculates the return path
    private void CreateReturnFlyingPath()
    {
        _wayPoints.Add(transform.position);
        _wayPoints.Add(_originalPosition);

    }
    
    // oncompletepath callback for iTween.
    private void OnCompleteTween()
    {
        _wayPoints.Clear();
        switch (_state)
        {
            case State.Idle:
                break;
            case State.NormalFlying:
                _state = State.Idle;
                transform.position = _originalPosition;
                break;
            case State.StealFlying:
                _state = State.Stealing;
                break;
            case State.Returning:
                _state = State.Idle;
                break;
            case State.Stealing:
                break;
            default:
                break;
        }
       
    }
    

    protected void OnTriggerEnter(Collider other)
    {
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Renderer render = GetComponent<MeshRenderer>();
            render.material.color = Color.blue;
            Debug.Log("Enemy hit!");
            scoreKeeper.Score(scoreValue);
            
            if (health <= 0)
            {
                Destroy(gameObject);
                //end of game
                scoreKeeper.Score(200);
               
                //Die();
            }

            
        }
       
    }

}
