using UnityEngine;
using System.Collections;

public class PlasmaWall : MonoBehaviour {

	void OnTriggerEnter(Collider other)
    {
        SimplePool.Despawn(gameObject);
        //Destroy(other.gameObject);
    }
}
