using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject bullet;
    public float projectileSpeed;
    public float speed = 15f;
    public float firingRate = 1f;
    private float padding = 2f;
    private float xMin;
    private float xMax;

	// Use this for initialization
	void Start () {
	    float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
	}
	
    void Fire()
    {
        Vector3 offset = new Vector3(0, 0, 4);
        GameObject laserBeam = Instantiate(bullet, transform.position + offset, Quaternion.identity) as GameObject;
        laserBeam.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, projectileSpeed);
        //Debug.Log("Fire at speed: " + projectileSpeed);
    }


	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Fire1"))
        {
            InvokeRepeating("Fire", 0.000001f, firingRate);
            //Debug.Log("Firing");
        }

        if (Input.GetButtonUp("Fire1"))
        {
            CancelInvoke("Fire");
        }

        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
        }

        // restrict the player to the gamespaces
        float newX = Mathf.Clamp(transform.position.x, xMin, xMax);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
	}

    void OnTriggerEnter(Collider other)
    {
        Projectile enemyProjectile = other.gameObject.GetComponent<Projectile>();
        if (enemyProjectile)
        {
            Destroy(gameObject);
            Debug.Log("Enemy hit Player.");
            Application.LoadLevel("Lose Screen");
        }
    }
}