using UnityEngine;
using System.Collections;

public class Projectile : MonoBehaviour {

    public float damage = 100f;

	public float timer;

	// Use this for initialization
	void Start () 
	{
		timer = 3f;
	}

    public float GetDamage()
    {
        return damage;
    }

    public void Hit()
    {
		SimplePool.Despawn(this.gameObject);
	}

	// Update is called once per frame
	void Update () 
	{
		timer -=Time.deltaTime;
		if (timer <= 0)
		{
			SimplePool.Despawn(this.gameObject);
		}
	}
	
	void OnEnable()
	{
		timer = 5f;
	}
}
