using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Enemy1Controller : EnemyController {


    public float pathSpeed = 10f;


    public Transform PlayerTransform;

    public List<Transform> playerOffsetPoints;
    


    private List<Vector3> _waypoints;
    private Vector3 _originalPosition;
    private float _pathPercentage = 0f;
    private float _remainingFallDelay = 5f;

    private bool _isOnPath = false;
    private bool _finishedPath = false;
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
            
        }
        if (_isOnPath)
        {
            _pathPercentage += Time.deltaTime * pathSpeed / 10;
            if (_pathPercentage > 1)
            {
                _pathPercentage = 0;
                if (_finishedPath)
                {
                    _isOnPath = false;
                    _waypoints.Clear();
                }
                _finishedPath = true;
                
                CreateIncomingPath();
                transform.position = _originalPosition;
                
            }
            if (gameObject)
            {
                if (_finishedPath)
                {
                    iTween.PutOnPath(gameObject, _waypoints.ToArray(), _pathPercentage);
                }
                else
                {
                    iTween.PutOnPath(gameObject, _waypoints.ToArray(), _pathPercentage);  
                }
                
            }
            
        }
        

        
	}
    private void CreatePath()
    {
        //_originalTransform.position = transform.position;
        _waypoints.Clear();
        _waypoints.Add(transform.position);
        for (float i = 0; i < 1; i+= 0.2f)
        {
            Vector3 lerpPoint = Vector3.Lerp(transform.position, playerOffsetPoints[0].position, i);
            _waypoints.Add(lerpPoint);
        }
        
       
        foreach (var thisTransform in playerOffsetPoints)
        {
            _waypoints.Add(thisTransform.position);
        }
    }
    private void CreateIncomingPath()
    {
        Vector3 lastPoint = _waypoints[_waypoints.Count - 1];
        _waypoints.Clear();

        Vector3 topSide = Camera.main.ViewportToWorldPoint(new Vector3(0,1,0));
        _waypoints.Add(new Vector3(lastPoint.x, 0, topSide.z));
        _waypoints.Add(_originalPosition);

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
