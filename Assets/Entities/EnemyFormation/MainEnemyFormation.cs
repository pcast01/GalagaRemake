using UnityEngine;
using System.Collections;

public class MainEnemyFormation : MonoBehaviour {

    public bool isMovingRight;
    //public bool isStartFormation;
    public bool moveFormation;
    public float padding = 1f;
    public float speed = 5.0f;
    public float width = 10f;
    public float height = 5f;
    private float xMin;
    private float xMax;

	// Use this for initialization
	void Start () {
        float distance = transform.position.z - Camera.main.transform.position.z;
        Vector3 leftMost = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, distance));
        Vector3 rightMost = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, distance));
        xMin = leftMost.x + padding;
        xMax = rightMost.x - padding;
        moveFormation = false;
        GalagaHelper.RoundNumber = 1;
        Invoke("StartRound", 3.0f);
	}
	
    void StartRound()
    {
        GameObject form1 = GameObject.FindGameObjectWithTag("phase1").gameObject;
        form1.GetComponent<EnemySpawner>().enabled = true;
        Debug.Log("Starting Round 1".Colored(Colors.purple).Bold());
    }
	// Update is called once per frame
	void Update () {
        if (moveFormation)
        {
            if (isMovingRight)
            {
                transform.position += Vector3.right * speed * Time.deltaTime;
            }
            else
            {
                transform.position += Vector3.left * speed * Time.deltaTime;
            }

            float rightEdgeOfFormation = transform.position.x + (0.5f * width);
            float leftEdgeOfFormation = transform.position.x - (0.5f * width);
            if (leftEdgeOfFormation < xMin)
            {
                isMovingRight = true;
            }
            else if (rightEdgeOfFormation > xMax)
            {
                isMovingRight = false;
            }
        }
        // Check to see if enemies have all been killed.
        //if (GalagaHelper.EnemiesSpawned == 0)
        //{
        //    Debug.Log("Round 2 about to begin".Colored(Colors.green));
        //    // Reset Formations
        //    GalagaHelper.ResetFormations();
        //    GalagaHelper.RoundNumber += 1;
        //}
        //Debug.Log("Enemies Currently Spawned: " + GalagaHelper.EnemiesSpawned + " Wave#: " + GalagaHelper.CurrentRoundPhase + " Round #: " + GalagaHelper.RoundNumber);
	}

    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube(transform.position, new Vector3(width, 2, height));
    }
}
