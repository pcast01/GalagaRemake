using UnityEngine;
using System.Collections;

public class PlasmaWall : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        Destroy(other.gameObject);
    }
}
