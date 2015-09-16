using UnityEngine;
using System.Collections;

public class EnemyController : MonoBehaviour {

    public float speed = 30.0f;
    public float health = 150f;
    public int scoreValue = 150;
    public bool isMovingRight;
    [Header("Weapon Settings")]
    public GameObject enemyLaser;
    public float shotsPerSecond = 0.5f;
    public float projectileSpeed = 16f;
    [Header("Flight Pattern Settings")]
    public float pathSpeed = 50.0F;
    public Hashtable myTween = new Hashtable();
    private ScoreKeeper scoreKeeper;
    private EnemySpawner round1Phase1spawner;

	void Start () {
        GalagaHelper.EnemiesSpawned += 1;
        round1Phase1spawner = GameObject.Find("Round1Phase1EnemyFormation").GetComponent<EnemySpawner>();
        //Debug.Log("enemies spawned: " + GalagaHelper.EnemiesSpawned);
        if (GalagaHelper.EnemiesSpawned <= 8)
        {
            // Spawn 1 of 5 phases
            if (round1Phase1spawner.spawnEntranceRight)
            {
                myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownRight));
            }
            else
            {
                myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownLeft));
            }
        }
        else if (GalagaHelper.EnemiesSpawned>8 && GalagaHelper.EnemiesSpawned<17)  // If Round1 Phase2 is not full then set path
        {
            myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.rd1_Left));
        }
        else if (GalagaHelper.EnemiesSpawned>16 && GalagaHelper.EnemiesSpawned<25) // 4 only
        {
            myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.rd1_Right)); 
        }
        else if (GalagaHelper.EnemiesSpawned > 24 && GalagaHelper.EnemiesSpawned < 33)
        {
            myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.rd1_TopLeft));
        }
        else if (GalagaHelper.EnemiesSpawned > 32 && GalagaHelper.EnemiesSpawned < 41)
        {
            myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.rd1_TopRight));
        }

        myTween.Add("speed", pathSpeed);
        scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
        iTween.MoveTo(gameObject, myTween);
    }

	void Update () {
        // Fire random
        float probability = Time.deltaTime * shotsPerSecond;
        if (Random.value < probability)
        {
            //Debug.Log("Enemy firing.");
            //Fire();
        }
	}

    void Fire()
    {
        Vector3 startPos = transform.position + new Vector3(0, 0, -4);
        GameObject enemyBullet = Instantiate(enemyLaser, startPos, Quaternion.identity) as GameObject;
        enemyBullet.GetComponent<Rigidbody>().velocity = new Vector3(0, 0, -projectileSpeed);
    }

    void OnTriggerEnter(Collider other)
    {
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            //Debug.Log("Enemy hit!");
            scoreKeeper.Score(scoreValue);
            if (health <= 0)
            {
                Destroy(gameObject);
                //end of game
                scoreKeeper.Score(200);
                Application.LoadLevel("Win Screen");
                //Die();
            }
        }
    }
}
