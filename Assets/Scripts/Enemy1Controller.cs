using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy1Controller : EnemyController {


    public float pathSpeed = 10f;


    public Transform PlayerTransform;

    public List<Transform> playerOffsetPoints;
    


    private List<Vector3> _waypoints;
    private Vector3 _originalPosition;
    private float _remainingFallDelay = 5f;

    private bool _isOnPath = false;
    private bool _finishedPath = false;

    private Hashtable _myTween = new Hashtable();
    void Awake() {
        // Save the first transform so we can use that to create our path.
        _originalPosition = transform.position;
        _waypoints = new List<Vector3>();
       
    }
	void Start () {
        base.Start();

        
	}
	
	
	void Update () {
        base.Update();

        _remainingFallDelay -= Time.deltaTime;
        if (!_isOnPath && (_remainingFallDelay <= 0 || Input.GetKeyDown(KeyCode.K)))
        {
            _remainingFallDelay = Random.Range(2f, 20f);

            _originalPosition = transform.position;
            CreatePath();
            _isOnPath = !_isOnPath;
            iTween.MoveTo(gameObject,_myTween);
            
        }   
	}
    private void CreatePath()
    {
        
        //_originalTransform.position = transform.position;
        _waypoints.Clear();
        _waypoints.Add(transform.position);

        _waypoints.Add(Vector3.Lerp(transform.position, playerOffsetPoints[0].position, 0.8f));
        foreach (var thisTransform in playerOffsetPoints)
        {
            _waypoints.Add(thisTransform.position);
        }

        _myTween.Clear();
        

        _myTween.Add("easetype", "linear");
        _myTween.Add("oncomplete", "OnComplete");
        _myTween.Add("path", _waypoints.ToArray());
        _myTween.Add("speed", pathSpeed);
        
    }
    private void CreateIncomingPath()
    {

        Vector3 lastPoint = _waypoints[_waypoints.Count - 1];
        _waypoints.Clear();
        
        Vector3 topSide = Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
        _waypoints.Add(new Vector3(lastPoint.x, 0, 43));
        _waypoints.Add(_originalPosition);

        _myTween.Clear();

        _myTween.Add("easetype", "linear");
        _myTween.Add("path", _waypoints.ToArray());
        _myTween.Add("oncomplete", "OnCompleteIncoming");
        _myTween.Add("speed", pathSpeed);
    }
    private void OnComplete()
    {
        

        Vector3 lastPoint = _waypoints[_waypoints.Count - 1];
        Vector3 topSide = Camera.main.ViewportToWorldPoint(new Vector3(0, 1, 0));
        transform.position = new Vector3(lastPoint.x,0,topSide.z);
        CreateIncomingPath();
        iTween.MoveTo(gameObject, _myTween);
    }
    private void OnCompleteIncoming()
    {
        _finishedPath = true;
        _isOnPath = false;
    }
    public void OnDrawGizmos()
    {
        if (_waypoints != null)
        {
            iTween.DrawPathGizmos(_waypoints.ToArray());
        }
        if (playerOffsetPoints != null)
        {
            iTween.DrawPathGizmos(playerOffsetPoints.ToArray());
        }
       
    }

}
