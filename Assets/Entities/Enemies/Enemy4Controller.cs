using UnityEngine;
using System.Collections;

public class Enemy4Controller : EnemyController
{
	void Start () {
        scoreKeeper = GameObject.Find("Score").GetComponent<ScoreKeeper>();
	}
	
    void OnTriggerEnter(Collider other)
    {
        Debug.Log("Something hit an enemy4".Colored(Colors.navy));
        Projectile playerBullet = other.gameObject.GetComponent<Projectile>();
        if (playerBullet)
        {
            health -= playerBullet.GetDamage();
            playerBullet.Hit();
            Debug.Log(gameObject.name.Colored(Colors.red).Bold() + " Enemy hit!".Bold().Colored(Colors.red));
            // Scorpion: if formation = 50 points, diving == 100
            if (isNotInFormation)
            {
                scoreKeeper.Score(100);
            }
            else
            {
                scoreKeeper.Score(50);
            }

            if (health <= 0)
            {

                top = base.addShotSounds(explosionTop[GalagaHelper.RandomNumber(0, explosionTop.Length)], Random.Range(0.8f, 1.2f));
                bottom = base.addShotSounds(explosionBottom, Random.Range(0.8f, 1.2f));
                top.PlayScheduled(0.3);
                bottom.Play();
                //rend.enabled = false;
                //meshcol.enabled = false;
                GameObject explosionPrefab = Instantiate(explosion, gameObject.transform.position, gameObject.transform.rotation) as GameObject;
                Destroy(explosionPrefab, 3.0f);
                //Debug.Log("Enemy1 killed: " + gameObject.name.Colored(Colors.blue) + " Parent: " + gameObject.transform.parent.parent.name.Colored(Colors.blue) + " Position: " + gameObject.transform.parent.name.Colored(Colors.blue));
                this.isEnemyFiring = false;
                DisableEnemy();
                //Invoke("DisableEnemy", spawnDisableTime);
                GalagaHelper.EnemiesKilled += 1;
                if (base.isRandomPicked == true)
                {
                    isRandomPicked = false;
                    main.isEnemy1Done = true;
                }
                iTween onTween = gameObject.GetComponent<iTween>();
                if (onTween)
                {
                    if (onTween.isRunning)
                    {
                        Debug.Log("Enemy4 Killed during Itween".Colored(Colors.red).Bold());
                        GalagaHelper.isScorpionAttackOn = false;
                    }
                }

                SimplePool.Despawn(gameObject);
            }
        }
    }

    void DisableEnemy()
    {
        Debug.Log("Runaway from parent Enemy1 called".Colored(Colors.navy) + " SpawnDisableTime: " + spawnDisableTime);
        gameObject.transform.parent = null;
    }

    void OnDisable()
    {
        Debug.Log("Disabled Enemy4: " + gameObject.name.Colored(Colors.red));
        GalagaHelper.DisabledEnemies += 1;
    }
}
