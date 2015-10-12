using UnityEngine;
using System.Collections;

public class PlayerIconController : MonoBehaviour {

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(3, 2, 3));
    }
}
