using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public GameObject bullet;
    public float projectileSpeed;
    public float speed = 15f;
    public float firingRate;
    private float padding = 2f;
    private float xMin;
    private float xMax;
    private bool allowFire = true;
    public AudioClip[] shotTop;
    public AudioClip[] shotBottom;
    public Vector3[] circlePath;
    private AudioSource top;
    private AudioSource bottom;

    void Awake()
    {
        circlePath = new Vector3[9];
        //SimplePool.Preload(bullet, 20);
    }
	// Use this for initialization
	void Start () {
	    float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
        //audio = GetComponents<AudioSource>();
        // Get circle path
        
        //Debug.Log(circlePath[0]);
        //circlePath[0]
	}

    public void GetCirclePath()
    {
        // Get all grandchildren
        //Debug.Log(transform.GetChild(0).childCount.ToString().Bold());
        for (int i = 0; i < transform.GetChild(0).childCount; i++)
		{
            //Debug.Log(transform.GetChild(0).GetChild(i).name.Bold());
            if (transform.GetChild(0).GetChild(i))
            {
                circlePath[i] = transform.GetChild(0).GetChild(i).position;
            }
		}
    }

    public AudioSource addShotSounds(AudioClip clip, float pitch)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.pitch = pitch;
        return audio;
    }
	
    IEnumerator Fire()
    {
        allowFire = false;
        Vector3 offset = new Vector3(0, 0, 4);
        //GameObject laserBeam = Instantiate(bullet, transform.position + offset, Quaternion.identity) as GameObject
        GameObject laserBeam = SimplePool.Spawn(bullet, transform.position + offset, Quaternion.identity, true) as GameObject;
        laserBeam.transform.position = transform.position + offset;
        laserBeam.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, projectileSpeed);
        // pick one of 3 random top shots
        top = addShotSounds(shotTop[Random.Range(0, shotTop.Length)], Random.Range(0.8f, 1.2f));
        bottom = addShotSounds(shotBottom[Random.Range(0, shotTop.Length)], Random.Range(0.8f, 1.2f));
        top.Play();
        bottom.Play();
        yield return new WaitForSeconds(firingRate);
        allowFire = true;
        //Debug.Log("Fire at speed: " + projectileSpeed);
    }


	// Update is called once per frame
	void Update () {

        if (Input.GetButtonDown("Fire1") && allowFire)
        {
            //InvokeRepeating("Fire", 0.000001f, firingRate);
            StartCoroutine("Fire");
            //Debug.Log("Firing");
        }

        //if (Input.GetButtonUp("Fire1"))
        //{
        //    CancelInvoke("Fire");
        //}

        
	}

    void FixedUpdate()
    {
        //float z = 0;
        //Vector3 euler = transform.localEulerAngles;
        //euler.z = Mathf.Lerp(euler.z, z, 2.0f * Time.deltaTime);
        //transform.localEulerAngles = euler;
        if (Input.GetKey(KeyCode.A))
        {
            transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
            //z = Input.GetAxis("Horizontal") * -35.0f;
            //Debug.Log("z: " + z + " euler.z: " + euler.z);
            //euler.z = Mathf.Lerp(euler.z, z, Time.fixedDeltaTime);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
            //z = Input.GetAxis("Horizontal") * 35.0f;
            //Debug.Log(" Right z: " + z + " euler.z: " + euler.z);
            //euler.z = Mathf.Lerp(euler.z, z, Time.fixedDeltaTime);
        }
        //else
        //{
        //    z = Input.GetAxis("Horizontal");
        //    euler.z = Mathf.Lerp(euler.z, 0f, 0f);
        //}

        // restrict the player to the gamespaces
        float newX = Mathf.Clamp(transform.position.x, xMin, xMax);
        transform.position = new Vector3(newX, transform.position.y, transform.position.z);
        //transform.localEulerAngles = euler;
        //transform.rotation = Quaternion.Euler(0.0f, 0.0f, rot);
    }

    void OnTriggerEnter(Collider other)
    {
        Projectile enemyProjectile = other.gameObject.GetComponent<Projectile>();
        Enemy1Controller enemy1 = other.gameObject.GetComponent<Enemy1Controller>();
        if (enemyProjectile)
        {
            Destroy(gameObject);
            Debug.Log("Enemy hit Player.");
            Application.LoadLevel("Lose Screen");
        }
        if (enemy1)
        {
            Destroy(gameObject);
            Debug.Log("Enemy ran into Player".Colored(Colors.cyan));
            Application.LoadLevel("Lose Screen");
        }
        Debug.Log("Something hit the player.".Colored(Colors.darkblue));
    }
}
