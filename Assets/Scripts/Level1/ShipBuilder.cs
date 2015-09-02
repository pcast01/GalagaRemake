using UnityEngine;
using System.Collections;

public class ShipBuilder : MonoBehaviour {
	
	private Material playerShipMat;
	private GameObject[] playerShips;
	private int curNumPlayerShips;
	public int numMaxPlayerShips = 1;
	
	private Material enemyShipMat;
	private GameObject[] enemyShips;
	private int curNumEnemyShips;
	public int numMaxEnemyShips = 10;
	
	// Use this for initialization
	void Start () {
		curNumPlayerShips = 0;
		playerShipMat = Resources.Load("PlayerMat", typeof(Material)) as Material;
		playerShips = new GameObject[numMaxPlayerShips];
		
		curNumEnemyShips = 0;
		enemyShipMat = Resources.Load("EnemyMat", typeof(Material)) as Material;
		enemyShips = new GameObject[numMaxEnemyShips];
		
	}
	
	public void createPlayerShip(Vector3 in_pos)
	{
		if (curNumPlayerShips < numMaxPlayerShips)
		{
			Debug.Log("spot3");
			//change to spawn ship model instead of cube
			GameObject playerShip = GameObject.CreatePrimitive(PrimitiveType.Cube);
			playerShip.name = "PlayerShip";
			playerShip.tag = "pShip";
			playerShip.transform.position = in_pos;
			playerShip.GetComponent<Renderer>().material = playerShipMat;
			playerShip.AddComponent(System.Type.GetType("PlayerController"));
			
			//playerShips[curNumPlayerShips] = playerShip;
			curNumPlayerShips++;
		}
	}
	
	public void createEnemyShip(Vector3 in_pos, int enemyType)
	{
		if (curNumEnemyShips < numMaxEnemyShips)
		{
			switch (enemyType)
			{
			case 0:
			{
				//change to spawn ship model instead of cube
				GameObject enemyShip = GameObject.CreatePrimitive(PrimitiveType.Cube);
				enemyShip.name = "EnemyShip0";
				enemyShip.tag = "eShip";
				enemyShip.transform.position = in_pos;
				enemyShip.GetComponent<Renderer>().material = enemyShipMat;
				enemyShip.AddComponent(System.Type.GetType("EnemyController"));
				
				enemyShips[curNumEnemyShips] = enemyShip;
				curNumEnemyShips++;
			}break;
				
			default:
				Debug.Log("ShipBuilder::createEnemyShip - unknown ship type spawn requested");
				break;
			}
		}
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
}
