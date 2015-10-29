using System.Collections;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    public float speed = 30.0f;
    public float health = 150f;
    public GameObject explosion;
    public bool isNotInFormation = false;
    private GameObject hero;
    public Renderer rend;
    public MeshCollider meshcol;
    public MainEnemyFormation main;
    public float spawnDisableTime = 5.1f;
    public bool isRandomPicked;

    [Header("Weapon Settings")]
    public GameObject enemyLaser;
    public bool isEnemyFiring;
    public float shotsPerSecond = 0.5f;
    public float projectileSpeed = 16f;

    [Header("Flight Pattern Settings")]
    public Hashtable myTween = new Hashtable();
    public ScoreKeeper scoreKeeper;
    private EnemySpawner round1Phase1spawner;
    private const float fDelay = 0.06f;

    [Header("Sound Settings")]
    public AudioClip[] explosionTop;
    public AudioClip explosionBottom;
    public AudioClip swooshSound;
    public AudioSource top;
    public AudioSource bottom;

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
        meshcol = GetComponent<MeshCollider>();
        main = GameObject.FindGameObjectWithTag("MainFormation").GetComponent<MainEnemyFormation>();
        spawnDisableTime = 10.0f;
        // get end direction random
        GalagaHelper.SetEnemy2Random();
        GalagaHelper.Enemy2PathEnd = GalagaHelper.Enemy2PathDirection();
        // get Tranform.Lookat() for Enemy2
        GalagaHelper.Enemy2LookAt();
        Debug.Log("Enemy2Random: ".Colored(Colors.red) + GalagaHelper.Enemy2Random);
        round1Phase1spawner = GameObject.Find("Round1Phase1EnemyFormation").GetComponent<EnemySpawner>();
        scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
        isEnemyFiring = false;
        //Debug.Log("enemies spawned: " + GalagaHelper.EnemiesSpawned);

        // Wave 1 path creation.
        if (GalagaHelper.JustSpawned <= 8)
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
        if (GalagaHelper.JustSpawned < 9)
        {
            myTween.Add("time", movePathTime);
            myTween.Add("easetype", "linear");
            myTween.Add("onComplete", "EnemyCompletePath");
            myTween.Add("onCompleteTarget", gameObject);
            GalagaHelper.CollectEnemyPaths(gameObject, myTween);
            //GalagaHelper.CurrentRoundPhase += 1;
        }

        if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase2)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase2, GalagaHelper.RoundNumber);
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase3)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase3, GalagaHelper.RoundNumber);
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase4)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase4, GalagaHelper.RoundNumber);
        }
        else if (GalagaHelper.CurrentRoundPhase == GalagaHelper.Formations.Round1Phase5)
        {
            CreatePathAndMove(GalagaHelper.Formations.Round1Phase5, GalagaHelper.RoundNumber);
            spawnDisableTime = 2.0f;
        }
    }

    public void EnemyCompletePath()
    {
        GalagaHelper.EnemiesSpawned += 1;
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
            if (gameObject.name == "EnemyTwo (16)" || gameObject.name == "EnemyOne (12)")
            {
                Debug.Log(GalagaHelper.SecondWavePath[10].ToString().Bold().Colored(Colors.darkblue));
            }
        }
        else
        {
            myTween.Add("path", GalagaHelper.FourthWavePath);
            if (gameObject.name == "EnemyTwo (16)" || gameObject.name == "EnemyOne (12)")
            {
                Debug.Log(GalagaHelper.FourthWavePath[7].ToString().Bold().Colored(Colors.darkblue));
            }
        }
        myTween.Add("time", movePathTime);
        myTween.Add("delay", GalagaHelper.Wave1Delay);
        myTween.Add("easetype", "linear");
        myTween.Add("onComplete", "EnemyCompletePath");
        myTween.Add("onCompleteTarget", gameObject);
        iTween.MoveTo(gameObject, myTween);
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
    }

    public void Update()
    {
        // Fire random
        if (isEnemyFiring)
        {
            float probability = Time.deltaTime * shotsPerSecond;
            float x = Random.value;
            //Debug.Log(gameObject.name + " (if x < probability )- Random: "+ x.ToString() + " Probability: " + probability.ToString());
            if (x < probability)
            {
                Fire();
            }  
        }
    }

    private void Fire()
    {
        // If enemy is north of player then fire
        if (!hero)
        {
            hero = GameObject.FindGameObjectWithTag("Player");
        }
        if (gameObject.transform.position.z > 10f)
        {
            Vector3 startPos = transform.position + new Vector3(0, 0, -4);
            //GameObject enemyBullet = Instantiate(enemyLaser, startPos, Quaternion.identity) as GameObject;
            GameObject enemyBullet = SimplePool.Spawn(enemyLaser, startPos, Quaternion.identity, true) as GameObject;
            enemyBullet.transform.position = startPos;

            // get player target
            Vector3 targetPosition = hero.transform.position;
            Vector3 currentPosition = enemyBullet.transform.position;

            Vector3 directionOfTravel = targetPosition - currentPosition;
            //Debug.Log("enemy firing ".Colored(Colors.red));
            enemyBullet.GetComponent<Rigidbody>().velocity = directionOfTravel.normalized * projectileSpeed;
        }
        else
        {
            //Debug.Log("Out of firing range.".Bold().Colored(Colors.red) + " Z position of Enemy: " + gameObject.transform.position.z);
        }
    }

    void DisableEnemy()
    {
        SimplePool.Despawn(gameObject);
        gameObject.transform.parent = null;
    }
}
