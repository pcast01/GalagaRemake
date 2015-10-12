using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    [Header("Captured Player Settings")]
    public bool playerCaptured = false;
    public bool rotatePlayer = false;
    [Header("Weapon Settings")]
    public GameObject bullet;
    public GameObject explosion;
    public float projectileSpeed;
    public float speed = 15f;
    public float firingRate;
    private float padding = 2f;
    private float xMin;
    private float xMax;
    private float playerWidth;
    private bool allowFire = true;
    private float newX;
    [Header("Sound Settings")]
    public AudioClip[] shotTop;
    public AudioClip[] shotBottom;
    public Vector3[] circlePath;
    public AudioClip explosionTop;
    public AudioClip explosionBottom;
    private AudioSource top;
    private AudioSource bottom;
    public Vector3 myOffset;

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
        Mesh mesh = GetComponent<MeshFilter>().mesh;
        playerWidth = mesh.bounds.size.x;
        Debug.Log("PlayerWidth: " + playerWidth);
        GalagaHelper.numOfPlayers += 1;
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
        bottom = addShotSounds(shotBottom[Random.Range(0, shotBottom.Length)], Random.Range(0.8f, 1.2f));
        top.volume = 0.5f;
        bottom.volume = 0.5f;
        top.Play();
        bottom.Play();
        yield return new WaitForSeconds(firingRate);
        allowFire = true;
        //Debug.Log("Fire at speed: " + projectileSpeed);
    }

	// Update is called once per frame
	void Update () {
        // If there is no captured player then proceed.
        if (!this.playerCaptured)
        {
            if (Input.GetButtonDown("Fire1") && allowFire)
            {
                StartCoroutine("Fire");
                //Debug.Log("Firing");
            }

            if (Input.GetKey(KeyCode.A))
            {
                transform.position += new Vector3(-speed * Time.deltaTime, 0, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, -7, 25), 50 * Time.deltaTime);
            }
            else if (Input.GetKey(KeyCode.D))
            {
                transform.position += new Vector3(speed * Time.deltaTime, 0, 0);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 7, 25), 50 * Time.deltaTime);
            }

            if (!Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
            {
                transform.rotation = Quaternion.RotateTowards(transform.rotation, Quaternion.Euler(0, 0, 0), 50 * Time.deltaTime);
            }

            // IF there are 2 players then change min(for left player) and max(for right player)
            if (this.tag == "CapturedPlayer")
            {
                newX = Mathf.Clamp(transform.position.x, xMin + playerWidth, xMax);
            }
            else if (this.tag == "Player")
            {
                newX = Mathf.Clamp(transform.position.x, xMin, xMax - playerWidth);
            }

            // restrict the player to the gamespaces
            // float newX = Mathf.Clamp(transform.position.x, xMin, xMax);
            transform.position = new Vector3(newX, transform.position.y, transform.position.z);
            
        }

        if (rotatePlayer)
        {
              transform.Rotate(0, 360 * Time.deltaTime, 0, Space.World);
        }
	}

    void OnTriggerEnter(Collider other)
    {
        Projectile enemyProjectile = other.gameObject.GetComponent<Projectile>();
        Enemy1Controller enemy1 = other.gameObject.GetComponent<Enemy1Controller>();
        if (enemyProjectile)
        {
            Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
            //DestroyImmediate(explosion);
            Debug.Log("Enemy proj hit Player.");
            top = addShotSounds(explosionTop, Random.Range(0.8f, 1.2f));
            bottom = addShotSounds(explosionBottom, Random.Range(0.8f, 1.2f));
            top.Play();
            bottom.Play();
            Destroy(gameObject);
            GalagaHelper.numOfPlayers -= 1;
            if (GalagaHelper.numOfPlayers == 1)
            {
                Invoke("EndGame", 3.0f);
            }
        }
        if (enemy1)
        {
            Destroy(gameObject);
            GalagaHelper.numOfPlayers -= 1;
            if (GalagaHelper.numOfPlayers == 1)
            {
                Invoke("EndGame", 3.0f);
            }
            Debug.Log("Enemy ran into Player".Colored(Colors.cyan));
        }
        Debug.Log("Something hit the player.".Colored(Colors.darkblue));
    }
    
    void EndGame()
    {
        Application.LoadLevel("Lose Screen");
    }

    void OnCollisionEnter(Collision other)
    {
        Debug.Log("Something hit the player.".Colored(Colors.red));
    }
}
