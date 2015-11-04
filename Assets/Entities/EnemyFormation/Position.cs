using UnityEngine;
using System.Collections;

public class Position : MonoBehaviour {

    public bool isOccupied = false;

    void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, 3.5f);
    }
}
