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
    //public Vector3[] SecondWavePath;
    public float pathSpeed;
    public Hashtable myTween = new Hashtable();
    private ScoreKeeper scoreKeeper;
    private EnemySpawner round1Phase1spawner;
    private const float fDelay = 0.06f;

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
                myTween.Add("delay", GalagaHelper.Wave1Delay);
                GalagaHelper.Wave1Delay += fDelay;
                Debug.Log("1 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            }
            else
            {
                myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownLeft));
                myTween.Add("delay", GalagaHelper.Wave1Delay);
                GalagaHelper.Wave1Delay += fDelay;
                Debug.Log("1 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            }
        }
        else if (GalagaHelper.EnemiesSpawned > 8 && GalagaHelper.EnemiesSpawned < 17)  // If Round1 Phase2 is not full then set path
        {
            //reset wave1delay
            //GalagaHelper.Wave1Delay = 0.0f;
            //myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_Left));
        }
        else if (GalagaHelper.EnemiesSpawned > 16 && GalagaHelper.EnemiesSpawned < 25) // 4 only
        {
            //myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_Right)); 
        }
        else if (GalagaHelper.EnemiesSpawned > 24 && GalagaHelper.EnemiesSpawned < 33)
        {
            //myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_TopLeft));
        }
        else if (GalagaHelper.EnemiesSpawned > 32 && GalagaHelper.EnemiesSpawned < 41)
        {
            //myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_TopRight));
        }

        scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
        if (GalagaHelper.EnemiesSpawned < 9)
        {
            myTween.Add("time", 3.0f);
            myTween.Add("easetype", "linear");
            GalagaHelper.CollectEnemyPaths(gameObject, myTween);
        }
        else if (GalagaHelper.EnemiesSpawned > 8 && GalagaHelper.EnemiesSpawned < 17)
        {
            GalagaHelper.Get2ndwave();
            GalagaHelper.Wave1Delay += 0.06f;
            myTween.Add("path", GalagaHelper.SecondWavePath);
            Debug.Log("<color=green>Second Wave Last pos: </color>" + GalagaHelper.SecondWavePath[10]);
            myTween.Add("time", 2.0f);
            myTween.Add("delay", GalagaHelper.Wave1Delay);
            myTween.Add("easetype", "linear");
            Debug.Log("2 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            //GalagaHelper.CollectEnemyPaths(gameObject, myTween);
            iTween.MoveTo(gameObject, myTween);
        }
        else if (GalagaHelper.EnemiesSpawned > 16 && GalagaHelper.EnemiesSpawned < 25)
        {
            GalagaHelper.ClearWavePath();
            GalagaHelper.Get3rdwave();
            GalagaHelper.Wave1Delay += 0.06f;
            myTween.Add("path", GalagaHelper.SecondWavePath);
            Debug.Log("<color=green>Third Wave Last pos: </color>" + GalagaHelper.SecondWavePath[10]);
            myTween.Add("time", 2.0f);
            myTween.Add("delay", GalagaHelper.Wave1Delay);
            myTween.Add("easetype", "linear");
            Debug.Log("2 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            //GalagaHelper.CollectEnemyPaths(gameObject, myTween);
            iTween.MoveTo(gameObject, myTween);
        }
        else if (GalagaHelper.EnemiesSpawned > 24 && GalagaHelper.EnemiesSpawned < 33)
        {
            GalagaHelper.ClearWavePath();
            GalagaHelper.Get4thwave();
            GalagaHelper.Wave1Delay += 0.06f;
            try
            {
                myTween.Add("path", GalagaHelper.FourthWavePath);

            }
            catch (System.Exception ex)
            {
                Debug.Log("error: " + ex.Message + ex.Source + ex.InnerException + ex.StackTrace);
                throw;
            }
            //Debug.Log("<color=green>Third Wave Last pos: </color>" + GalagaHelper.SecondWavePath[10]);
            myTween.Add("time", 2.0f);
            myTween.Add("delay", GalagaHelper.Wave1Delay);
            myTween.Add("easetype", "linear");
            Debug.Log("2 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            //GalagaHelper.CollectEnemyPaths(gameObject, myTween);
            iTween.MoveTo(gameObject, myTween);
        }
        else if (GalagaHelper.EnemiesSpawned > 32 && GalagaHelper.EnemiesSpawned < 41)
        {
            GalagaHelper.ClearWavePath();
            GalagaHelper.Get5thwave();
            GalagaHelper.Wave1Delay += 0.06f;
            try
            {
                myTween.Add("path", GalagaHelper.FourthWavePath);

            }
            catch (System.Exception ex)
            {
                Debug.Log("error: " + ex.Message + ex.Source + ex.InnerException + ex.StackTrace);
                throw;
            }
            //Debug.Log("<color=green>Third Wave Last pos: </color>" + GalagaHelper.SecondWavePath[10]);
            myTween.Add("time", 2.0f);
            myTween.Add("delay", GalagaHelper.Wave1Delay);
            myTween.Add("easetype", "linear");
            Debug.Log("2 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            //GalagaHelper.CollectEnemyPaths(gameObject, myTween);
            iTween.MoveTo(gameObject, myTween);
        }
        else
	    {
            myTween.Add("time", 2.0f);
            myTween.Add("easetype", "linear");
            iTween.MoveTo(gameObject, myTween);
	    }

    }

    public void OnDrawGizmos()
    {
        if (GalagaHelper.SecondWavePath.Length != 0)
        {
            iTween.DrawPath(GalagaHelper.SecondWavePath);
        }
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
