using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	private CameraController m_cameraController;
	private ShipBuilder m_shipBuilder;

	public float respawnRate = 10f;
	private float timer;

	// Use this for initialization
	void Start () {
		//Get GameObjects
		m_cameraController = (CameraController)(GameObject.Find("Main Camera")).GetComponent(typeof(CameraController));
		m_shipBuilder = (ShipBuilder)(GameObject.Find("o_ShipBuilder")).GetComponent(typeof(ShipBuilder));

		timer = respawnRate;

		initLevel();
	}
	
	// Update is called once per frame
	void Update () {
		timer -=Time.deltaTime;
		if (timer <= 0)
		{
			Vector3 tmpVector3 = new Vector3(0,0,4);
			m_shipBuilder.createEnemyShip(tmpVector3,0);
			timer = respawnRate;
		}
	

	}

	private void initLevel()
	{
		//Create player ships and enemy ships. 
		//Ideally call a spawnLevelLayout() or something that calls createEnemyShip a bunch of times to create the enemy layout.
		//m_shipBuilder.createPlayerShip((m_cameraController.getLeftSide() + m_cameraController.getRightSide() / 2));

		Vector3 tmpVector = new Vector3(0,0,-4);
		m_shipBuilder.createPlayerShip(tmpVector);

		Vector3 tmpVector2 = new Vector3(0,0,4);
		m_shipBuilder.createEnemyShip(tmpVector2,0);
	}
}
