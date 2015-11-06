using UnityEngine;
using System.Collections;

public class CirclePathController : MonoBehaviour {

    void Update()
    {
        Quaternion noRotation = Quaternion.LookRotation(Vector3.forward, Vector3.up);
        transform.rotation = noRotation;
    }
}
