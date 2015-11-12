using UnityEngine;
using System.Collections;

public class PlasmaWall : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        SimplePool.Despawn(other.gameObject);
    }
}
