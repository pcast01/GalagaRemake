using UnityEngine;
using System.Collections;

public class CameraController : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public Vector3 getLeftSide()
	{
		return Camera.main.ViewportToWorldPoint(new Vector3(0, 0, 0));
	}

	public Vector3 getRightSide()
	{
		return Camera.main.ViewportToWorldPoint(new Vector3(1, 0, 0));
	}
}
