using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 30.0f;
    public float health = 150f;
    public int scoreValue = 150;
    public GameObject explosion;
    private GameObject hero;
    private Renderer rend;

    [Header("Weapon Settings")]
    public GameObject enemyLaser;
    public bool isEnemyFiring;
    public float shotsPerSecond = 0.5f;
    public float projectileSpeed = 16f;

    [Header("Flight Pattern Settings")]
    public Hashtable myTween = new Hashtable();
    private ScoreKeeper scoreKeeper;
    private EnemySpawner round1Phase1spawner;
    private const float fDelay = 0.06f;

    [Header("Sound Settings")]
    public AudioClip[] explosionTop;
    public AudioClip explosionBottom;
    public AudioClip swooshSound;
    private AudioSource top;
    private AudioSource bottom;

    [SerializeField]
    private float movePathTime;

    public AudioSource addShotSounds(AudioClip clip, float pitch)
    {
        AudioSource audio = gameObject.AddComponent<AudioSource>();
        audio.clip = clip;
        audio.pitch = pitch;
        return audio;
    }

    public void Start()
    {
        hero = GameObject.FindGameObjectWithTag("Player");
        rend = GetComponent<Renderer>();
        GalagaHelper.EnemiesSpawned += 1;
        round1Phase1spawner = GameObject.Find("Round1Phase1EnemyFormation").GetComponent<EnemySpawner>();
        scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
        //Debug.Log("enemies spawned: " + GalagaHelper.EnemiesSpawned);

        // Wave 1 path creation.
        if (GalagaHelper.EnemiesSpawned <= 8)
        {
            // Spawn 1 of 5 phases
            if (round1Phase1spawner.spawnEntranceRight)
            {
                myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownRight));
                myTween.Add("delay", GalagaHelper.Wave1Delay);
                GalagaHelper.Wave1Delay += fDelay;
                //Debug.Log("1 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            }
            else
            {
                myTween.Add("path", GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownLeft));
                myTween.Add("delay", GalagaHelper.Wave1Delay);
                GalagaHelper.Wave1Delay += fDelay;
                //Debug.Log("1 WAVE DELAY; " + GalagaHelper.Wave1Delay);
            }
        }

        // Collect all 1st wave enemies and move them all at once with a delay
        if (GalagaHelper.EnemiesSpawned < 9)
        {
            myTween.Add("time", movePathTime);
            myTween.Add("easetype", "linear");
            GalagaHelper.CollectEnemyPaths(gameObject, myTween);
        }
        else if (GalagaHelper.EnemiesSpawned > 8 && GalagaHelper.EnemiesSpawned < 17)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase2, GalagaHelper.RoundNumber);
        }
        else if (GalagaHelper.EnemiesSpawned > 16 && GalagaHelper.EnemiesSpawned < 25)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase3, GalagaHelper.RoundNumber);
        }
        else if (GalagaHelper.EnemiesSpawned > 24 && GalagaHelper.EnemiesSpawned < 33)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase4, GalagaHelper.RoundNumber);
        }
        else if (GalagaHelper.EnemiesSpawned > 32 && GalagaHelper.EnemiesSpawned < 41)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase5, GalagaHelper.RoundNumber);
        }
        else
        {
            myTween.Add("time", movePathTime);
            myTween.Add("easetype", "linear");
            iTween.MoveTo(gameObject, myTween);
        }
    }

    /// <summary>
    /// Create path based on wave number.
    /// </summary>
    /// <param name="wave"></param>
    public void CreatePathAndMove(GalagaHelper.Formations form, int RoundNumber)
    {
        GalagaHelper.ClearWavePath();
        GalagaHelper.GetWavePaths(form, RoundNumber);
        GalagaHelper.Wave1Delay += 0.06f;
        if ((int)form == 2 || (int)form == 3)
        {
            myTween.Add("path", GalagaHelper.SecondWavePath);
        }
        else
        {
            myTween.Add("path", GalagaHelper.FourthWavePath);
        }
        myTween.Add("time", movePathTime);
        myTween.Add("delay", GalagaHelper.Wave1Delay);
        myTween.Add("easetype", "linear");
        iTween.MoveTo(gameObject, myTween);
        if ((int)form % 2 != 0)
        {
            GalagaHelper.PrintAllGhostObjects();
        }
    }

    public void OnDrawGizmos()
    {
        if (GalagaHelper.SecondWavePath.Length != 0)
        {
            iTween.DrawPath(GalagaHelper.SecondWavePath);
        }
        if (GalagaHelper.FourthWavePath.Length != 0)
        {
            iTween.DrawPath(GalagaHelper.FourthWavePath);
        }
        //iTween.DrawPath(GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownLeft));
       // iTween.DrawPath(GalagaHelper.EntrancePatterns(GalagaHelper.EntranceFlightPatterns.round1_DownRight));
    }

    public void Update()
    {
        // Fire random
        if (isEnemyFiring)
        {
            float probability = Time.deltaTime * shotsPerSecond;
            if (Random.value < probability)
            {
                Debug.Log("Enemy firing.");
                Fire();
            }  
        }
        
        //EnemySpawner formSpawn = GalagaHelper.GetFormationScript(GalagaHelper.CurrentRoundPhase);

        //if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase1 && GalagaHelper.EnemiesSpawned > 7)
        //{
        //    if (gameObject.transform.position == formSpawn.transform.position)
        //    {
        //        iTween.MoveUpdate(gameObject, formSpawn.currentSpawnPos.position, 0.5f);
        //        Debug.Log("Move update called.".Bold());
        //    }
        //}
        // Until the enemy is in the position keep updating moveTo
    }

    private void Fire()
    {
        // If enemy is north of player then fire
        if (gameObject.transform.position.x > hero.transform.position.x)
        {
            Vector3 startPos = transform.position + new Vector3(0, 0, -4);
            //GameObject enemyBullet = Instantiate(enemyLaser, startPos, Quaternion.identity) as GameObject;
            GameObject enemyBullet = SimplePool.Spawn(enemyLaser, startPos, Quaternion.identity, true) as GameObject;
            enemyBullet.transform.position = startPos;

            // get player target
            Vector3 targetPosition = hero.transform.position;
            Vector3 currentPosition = enemyBullet.transform.position;

            Vector3 directionOfTravel = targetPosition - currentPosition;
            Debug.Log("firing".Colored(Colors.red));
            enemyBullet.GetComponent<Rigidbody>().velocity = directionOfTravel.normalized * projectileSpeed;
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Something hit an enemy");
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Debug.Log("Enemy hit!".Bold().Colored(Colors.red));
            scoreKeeper.Score(scoreValue);
            if (health <= 0)
            {
                //gameObject.isDead = true;
                //Debug.Log("parent ".Bold()+ gameObject.transform.parent);
                //Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation);
                top = addShotSounds(explosionTop[Random.Range(0, explosionTop.Length)], Random.Range(0.8f, 1.2f));
                bottom = addShotSounds(explosionBottom, Random.Range(0.8f, 1.2f));
                top.PlayScheduled(0.3);
                bottom.Play();
                rend.enabled = false;
                GameObject explosionPrefab = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                Destroy(explosionPrefab, 3.0f);
                Invoke("DisableEnemy", top.clip.length);
                //SimplePool.Despawn(gameObject);
                //gameObject.transform.parent = null;
                GalagaHelper.EnemiesKilled += 1;
                //gameObject.SetActive(false);
                //Debug.Log("Enemy is dead".Bold() + " Pos: " + gameObject.transform.parent.transform.parent.name);
                scoreKeeper.Score(200);
                //Application.LoadLevel("Win Screen");
                //Die();
                //Destroy(explosion);
            }
        }
    }

    void DisableEnemy()
    {
        SimplePool.Despawn(gameObject);
        gameObject.transform.parent = null;
    }
}
