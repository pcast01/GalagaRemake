using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Main : MonoBehaviour {

	private PlayerController m_playerController;
	private EnemyController m_enemyController;
	private CameraController m_cameraController;
	private ShipBuilder m_shipBuilder;

	// Use this for initialization
	void Start () {
		//Get GameObjects
		//m_playerController = (PlayerController)(GameObject.Find("o_PlayerController")).GetComponent(typeof(PlayerController));
		//m_enemyController = (EnemyController)(GameObject.Find("o_EnemyController")).GetComponent(typeof(EnemyController));
		m_cameraController = (CameraController)(GameObject.Find("Main Camera")).GetComponent(typeof(CameraController));
		m_shipBuilder = (ShipBuilder)(GameObject.Find("o_ShipBuilder")).GetComponent(typeof(ShipBuilder));

		initLevel();
	}
	
	// Update is called once per frame
	void Update () {

	

	}

	private void initLevel()
	{
		//m_playerController
		//m_enemyController
		//m_cameraController

		Debug.Log("spot1");

		//Create player ships and enemy ships. 
		//Ideally call a spawnLevelLayout() or something that calls createEnemyShip a bunch of times to create the enemy layout.
		//m_shipBuilder.createPlayerShip((m_cameraController.getLeftSide() + m_cameraController.getRightSide() / 2));

		Vector3 tmpVector = new Vector3(0,0,0);
		m_shipBuilder.createPlayerShip(tmpVector);

		Vector3 tmpVector2 = new Vector3(0,0,4);
		m_shipBuilder.createEnemyShip(tmpVector2,0);
		Debug.Log("spot2");
	}
}
