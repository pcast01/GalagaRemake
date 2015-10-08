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
    private Quaternion origRotation;
    public bool sweepTractorBeam;
    private int theAngle = 32;
    private int segments = 10;
    private float distance = 2.0f;
    [Header("Sound Settings")]
    private AudioSource audio;

	void Start () {
	    base.Start();
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<Transform>();
        AttackPlayer = true;
        origRotation = transform.rotation;
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

        if (sweepTractorBeam)
        {
            RaycastSweep();
        }
	}

    public void TractorBeamAttack()
    {
        transform.LookAt(player);
        Vector3 targetPosition = player.transform.position;
        Vector3 currentPosition = this.transform.position;
        //this.isEnemyFiring = true;

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
            Vector3 directionOfTravel = targetPosition - currentPosition;
            transform.rotation = origRotation;
            Debug.Log("Reached the player range.");
            // Stop Moving and set tractor beam
            Vector3 offset = new Vector3(0, 0, -3.5f);
            GameObject tractorBeamGO = Instantiate(tractorBeam, gameObject.transform.position + offset, gameObject.transform.rotation) as GameObject;
            ParticleSystem tractor = tractorBeamGO.GetComponent<ParticleSystem>();
            tractor.enableEmission = true;
            if (!tractor.isPlaying)
            {
                tractor.Play();
                //Enable sweep to be made
                sweepTractorBeam = true;
            }
            // Create Tractor beam for 5 seconds
            AttackPlayer = false;
            // Now move towards enemy wall
        }

    }

    // Set lines 
    void RaycastSweep()
    {
        Vector3 targetPosition = player.transform.position;
        Vector3 currentPosition = this.transform.position;
        Vector3 directionOfTravel = targetPosition - currentPosition;
        Debug.Log("Raycast sweep init.");
        Vector3 offset = new Vector3(0, 0, -3.5f);
        Vector3 startPos = transform.position + offset;
        Vector3 targetPos = Vector3.zero;

        int startAngle = (int)(-theAngle * 0.5);
        int finishAngle = (int)(theAngle * 0.5);

        int inc = (int)(theAngle / segments);

        RaycastHit hit;

        for (int i = startAngle; i < finishAngle; i+= inc)
        {
            targetPos = (Quaternion.Euler(0, i, 0) * directionOfTravel) * distance;
            if (Physics.Linecast(startPos, targetPos, out hit))
            {
                if (hit.collider.gameObject.name == "Player")
                {
                    sweepTractorBeam = false;
                    Debug.Log("Player will now be captured.");
                }
            }

            Debug.DrawLine(startPos, targetPos, Color.green);
            Debug.DrawLine(startPos, directionOfTravel, Color.blue);
            //Debug.DrawLine(targetPos, directionOfTravel, Color.magenta);
        }

    }

}
