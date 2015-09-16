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
    public float pathSpeed = 10f;
    public float heightOffset = 25f;

    private List<Vector3> _wayPoints = new List<Vector3>();

    private float _pathPercentage = 0f;
    private Vector3 _originalPosition;
    private Vector3 _endPoint;
    private State _state = State.Idle;
    private Hashtable myTween = new Hashtable();
    private float xMin;
    private float xMax;
    void Awake() 
    {
        xMin = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0)).x;
        xMax = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0)).x;
       
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
                iTween.MoveTo(gameObject, myTween);
            }
            if (Input.GetKeyDown(KeyCode.L))
            {
                _state = State.StealFlying;
                CreatePath(_state);
                iTween.MoveTo(gameObject, myTween);
            }
        }
        
        
        
	}
    // Method that calculates the trajectory for the finding the player
    private void CreateStealFlyingPath()
    {
        
        _wayPoints.Add(transform.position);
        Vector3 endPoint = playerObject.transform.position + new Vector3(0, 0, heightOffset);
        _wayPoints.Add(endPoint);
    }

    // Function that calculates the normal flying path based on offset
    private void CreateNormalFlyingPath()
    {
        _wayPoints.Add(transform.position);

        Vector3 point = new Vector3(xMin + Random.Range(10, 60),transform.position.y,Random.Range(9f,15f));
        _wayPoints.Add(point);
        point = new Vector3(xMax - Random.Range(10, 60), transform.position.y, Random.Range(-7f, 6f));
        _wayPoints.Add(point);
        point = new Vector3(xMin + Random.Range(10, 60), transform.position.y, -35);
        _wayPoints.Add(point);

    }

    private void CreatePath(State state)
    {
        myTween.Clear();
        _originalPosition = transform.position;
        switch (state)
        {
            case State.Idle:
                break;
            case State.NormalFlying:
                CreateNormalFlyingPath();
                myTween.Add("path", _wayPoints.ToArray());
                break;
            case State.StealFlying:
                CreateStealFlyingPath();
                myTween.Add("path",_wayPoints.ToArray());
                break;
            case State.Returning:
                break;
            case State.Stealing:
                break;
            default:
                break;
        }
        myTween.Add("speed", pathSpeed);
        myTween.Add("oncomplete", "onCompleteTween");
        myTween.Add("easetype", "linear");
    }
    private void onCompleteTween()
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
                break;
            case State.Stealing:
                break;
            default:
                break;
        }
       
    }
    

}
