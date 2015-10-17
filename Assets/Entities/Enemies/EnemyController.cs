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
    public MainEnemyFormation main;
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
        main = GameObject.FindGameObjectWithTag("MainFormation").GetComponent<MainEnemyFormation>();
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
        }

        //else if (GalagaHelper.EnemiesSpawned > 8 && GalagaHelper.EnemiesSpawned < 17)
        //{
        //    CreatePathAndMove(GalagaHelper.Formations.Round1Phase2, GalagaHelper.RoundNumber);
        //}
        //else if (GalagaHelper.EnemiesSpawned > 16 && GalagaHelper.EnemiesSpawned < 25)
        //{
        //    Debug.Log(GalagaHelper.CurrentRoundPhase.ToString().Colored(Colors.lightblue));
        //    CreatePathAndMove(GalagaHelper.Formations.Round1Phase3, GalagaHelper.RoundNumber);
        //}
        //else if (GalagaHelper.EnemiesSpawned > 24 && GalagaHelper.EnemiesSpawned < 33)
        //{
        //    Debug.Log(GalagaHelper.CurrentRoundPhase.ToString().Colored(Colors.lightblue));
        //    CreatePathAndMove(GalagaHelper.Formations.Round1Phase4, GalagaHelper.RoundNumber);
        //}
        //else if (GalagaHelper.EnemiesSpawned > 32 && GalagaHelper.EnemiesSpawned < 41)
        //{
        //    Debug.Log(GalagaHelper.CurrentRoundPhase.ToString().Colored(Colors.lightblue));
        //    CreatePathAndMove(GalagaHelper.Formations.Round1Phase5, GalagaHelper.RoundNumber);
        //}
        //else
        //{
        //    //Debug.Log("Else fired".Bold());
        //    //myTween.Add("time", movePathTime);
        //    //myTween.Add("easetype", "linear");
        //    //iTween.MoveTo(gameObject, myTween);
        //}
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
        Debug.Log("Wave1delay: " + GalagaHelper.Wave1Delay);
        myTween.Add("easetype", "linear");
        iTween.MoveTo(gameObject, myTween);
        //if (myTween.Contains("path"))
        //{
        //    Debug.Log(myTween["path"])
        //}
        if (GalagaHelper.EnemiesSpawned == 16)
        {
            // Last enemy done for 2nd wave
            main.secondWaveFinished = true;
        }
        else if (GalagaHelper.EnemiesSpawned == 24)
        {
            main.thirdWaveFinished = true;
            Debug.Log("third wave finished.");
        }
        else if (GalagaHelper.EnemiesSpawned == 32)
        {
            main.fourthWaveFinished = true;
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

    void DisableEnemy()
    {
        SimplePool.Despawn(gameObject);
        gameObject.transform.parent = null;
    }
}
