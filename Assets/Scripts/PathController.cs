using UnityEngine;
using System.Collections;

public class PathController : MonoBehaviour {

    public Transform playerTransform;

    public bool isFixed = false;
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
        if (!isFixed)
        {
            transform.position = new Vector3(playerTransform.position.x,0,transform.position.z);
        }
	}
}
