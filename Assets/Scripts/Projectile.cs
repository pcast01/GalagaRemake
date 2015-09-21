using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float damage = 200f;

    void Awake()
    {
        //SimplePool.Preload(20);
    }

    public float GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
        SimplePool.Despawn(gameObject);
        //Destroy(gameObject);
    }
}
