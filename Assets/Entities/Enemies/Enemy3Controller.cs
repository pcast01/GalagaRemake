using UnityEngine;
using System.Collections;

public class Enemy3Controller : EnemyController 
{
    [Header("Enemy 3 Settings")]
    public float swoopSpeed;
    public bool AttackPlayer = false;
    public GameObject tractorBeam;
    private Transform player;
    private bool outOfPlayerRange = false;
    [Header("Path from Top of Screen Settings")]
    public Vector3 _originalPosition;
    private bool gotOriginalPosition = false;
    [Header("Sound Settings")]
    private AudioSource audio;

	void Start () {
	    base.Start();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        AttackPlayer = true;
	}
	
	void Update () {
        base.Update();
        if (AttackPlayer)
        {
            if (gotOriginalPosition == false)
            {
                _originalPosition = this.transform.position;
                gotOriginalPosition = true;
            }
            TractorBeamAttack();
        }
	}

    public void TractorBeamAttack()
    {
        transform.LookAt(player);
        //move towards the center of the world (or where ever you like)
        //Vector3 targetPosition = new Vector3(0, 0, 0);
        Vector3 targetPosition = player.transform.position;
        Vector3 currentPosition = this.transform.position;
        this.isEnemyFiring = true;
        //first, check to see if we're close enough to the target
        if (Vector3.Distance(currentPosition, targetPosition) > 24.0f && outOfPlayerRange == false)
        {
            Vector3 directionOfTravel = targetPosition - currentPosition;
            //now normalize the direction, since we only want the direction information
            directionOfTravel.Normalize();
            //scale the movement on each axis by the directionOfTravel vector components
            // Play swoop sound
            if (currentPosition == _originalPosition)
            {
                //input sound
                audio = base.addShotSounds(swooshSound, 1.0f);
                audio.Play();
                Debug.Log("Sound swoop played".Colored(Colors.red));
            }
            this.transform.Translate(
                (directionOfTravel.x * swoopSpeed * Time.deltaTime),
                (directionOfTravel.y * swoopSpeed * Time.deltaTime),
                (directionOfTravel.z * swoopSpeed * Time.deltaTime),
                Space.World);
        }
        else
        {
            outOfPlayerRange = true;
            Debug.Log("Reached the player range.");
            // Stop Moving and set tractor beam
            Vector3 offset = new Vector3(0, 0, -3.5f);
            Instantiate(tractorBeam, gameObject.transform.position+ offset, gameObject.transform.rotation);
            // Create Tractor beam for 5 seconds
            AttackPlayer = false;
            // Now move towards enemy wall
        }
    }
}
