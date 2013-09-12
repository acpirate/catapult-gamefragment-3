using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public enum GAMESTATE { TITLE, PLAY, GAMEOVER, SETTINGS, AIM };
public enum DIRECTION { LEFT, RIGHT, NONE, UP, DOWN};
public enum ENGINE { BALLISTA, CATAPULT, TREBUCHET};

public class MainGameCode : MonoBehaviour {
	
	public static Dictionary<ENGINE,Engine> engines=new Dictionary<ENGINE,Engine>();
	
	public static GameObject king=null;
	public static GameObject puck=null;
	public static GameObject aimCamera=null;
	static Camera mainCamera=null;
	static Vector3 mainCameraStart=new Vector3(0,30,-280);
	
	
	public GameObject brickPrefab;
	static GameObject staticBrickPrefab;
	static int wallWidth=12;
	static int wallHeight=10;
	static float brickWidth=20f;
	static float brickHeight=10f;
	static float brickHeightOffset=5.47986f;
	static float wallXStart=-300;
	static float wallZStart=950;
	
	public static ENGINE selectedEngine = ENGINE.BALLISTA;
	public static int angleSelectNumber=0;
	
	
	static float powerChargeRate=50;
	public static float maxBallistaPower=30;
	public static float maxCatapultPower=60;
	public static float maxTrebuchetPower=100;
	public static float maxPower=100;
	public static float currentPower=0;
	public static float powerMultiplier=150f;
	public static float puckResetLocation=-245;
	
	public static float gameTime;
	public static float finalTime=0;
	public static float bestTime=0;
	
	public static GAMESTATE gamestate=GAMESTATE.TITLE;

	// Use this for initialization
	void Awake() {
		
		if (engines.Count==0) InitializeEngines();
		if (king==null) king=GameObject.Find("King");
		if (puck==null) puck=GameObject.Find("Puck");
		if (aimCamera==null) aimCamera=GameObject.Find("AimCamera");
		if (mainCamera==null) mainCamera=GameObject.Find("Main Camera").GetComponent<Camera>();
		
		staticBrickPrefab=brickPrefab;
		
		BuildWall();
		
	}	
	
	void Start () {
		mainCamera.transform.LookAt(puck.transform.position);
	}
	
	// Update is called once per frame
	void Update () {
		//turn off main camera if the puck is moving
		if (puck.GetComponent<PuckCode>().puckMoving) {mainCamera.enabled=false; } 
		if (!puck.GetComponent<PuckCode>().puckMoving && gamestate!=GAMESTATE.AIM) { 
			mainCamera.enabled=true;
			//mainCamera.transform.position=puck.GetComponent<PuckCode>().moveCamera.transform.position;
			//mainCamera.transform.position=new Vector3(mainCamera.transform.position.x, Mathf.Abs(mainCamera.transform.position.y),mainCamera.transform.position.z);
		}		
		//reset the puck if it falls off
		if (puck.transform.position.y<-100) ResetPuck();
		if (gamestate!=GAMESTATE.GAMEOVER && gamestate!=GAMESTATE.SETTINGS && gamestate!=GAMESTATE.TITLE) gameTime+=Time.deltaTime;
		if (gamestate==GAMESTATE.TITLE) { ResetTimers(); }
		
	}


	public static void ResetTimers() {
		finalTime=0;
		gameTime=0;
	}	
	
	//static methods
	public static void SetEngine(ENGINE inEngine) {
		selectedEngine=inEngine;
	}	
	
	static void InitializeEngines() {
		engines[ENGINE.TREBUCHET]=new Engine(ENGINE.TREBUCHET,100,new List<int>() {40});
		engines[ENGINE.BALLISTA]=new Engine(ENGINE.BALLISTA,50,new List<int>() {1,15,30});
		engines[ENGINE.CATAPULT]=new Engine(ENGINE.CATAPULT,75,new List<int>() {45,60,75});
	}	
	
	static void BuildWall() {
		for (int yCounter=0;yCounter<wallHeight;yCounter++) {
			for (int xCounter=0;xCounter<wallWidth;xCounter++) {
				float brickOffset=0;
				if (yCounter%2==1) brickOffset=brickWidth/2;
				Vector3 brickLocation=new Vector3(wallXStart+xCounter*brickWidth+brickOffset,brickHeight*yCounter+brickHeightOffset,wallZStart);
				Instantiate(staticBrickPrefab,brickLocation,Quaternion.identity);
			}	
		}	
	}	
	
	
	
	
	public static void ShootPuck() {
		Engine tempEngine=engines[selectedEngine];
		
		int tempMaxPower=tempEngine.getMaxPower();
		
		EndAim();
		puck.transform.position+=new Vector3(0,tempEngine.getElevation(),0);
		
		int tempAngle=engines[selectedEngine].getAngles()[angleSelectNumber];
		//puck normalized forward is the starting point
		Vector3 shootVector=puck.transform.forward;
		//rotated vector to shooting angle
		
		
		
		GameObject tempRotatingObject=new GameObject("tempGameObject");
		
		tempRotatingObject.transform.rotation=puck.transform.rotation;
		
		Debug.Log("maingamecode: shootvector " + tempRotatingObject.transform.forward);
		
		tempRotatingObject.transform.eulerAngles+=new Vector3(-1*tempAngle,0,0);
		
		shootVector=tempRotatingObject.transform.forward;
		
		Destroy(tempRotatingObject);
		
		//Debug.Log("maingamecode: tempAngle " + tempAngle);
		
		//shootVector=Quaternion.AngleAxis(-1*tempAngle,puck.transform.right)*shootVector;
		
		Debug.Log("maingamecode: rotated shootvector " + shootVector);
				
		//add power multipliers
		shootVector*=currentPower/100*tempMaxPower*powerMultiplier;
		//puck.transform.eulerAngles+=new Vector3(0,tempAngle*-1,0);
		
		puck.rigidbody.AddForce(shootVector);
		if (tempEngine.getType()!=ENGINE.BALLISTA)
			puck.rigidbody.AddRelativeTorque(new Vector3(currentPower/100*maxPower*powerMultiplier*10,0,0));
		
		currentPower=0;
	}	
		
	
	public static void QuitGame() {
		ClearBricks();
		ResetKingPedestal();
		ResetKing();
		ResetPuck();
		BuildWall();
		gamestate=GAMESTATE.GAMEOVER;
	}
	
	public static void ResetGame() {
		QuitGame();
		gamestate=GAMESTATE.TITLE;	
	}	
	
	public static void ClearBricks() {
		foreach(GameObject brick in GameObject.FindGameObjectsWithTag("Brick")) {
			Destroy(brick);	
		}	
	}	
	
	
	public static void PlayGame() {
		gamestate=GAMESTATE.PLAY;	
	}
	
	public static void GameOver() {
		finalTime=gameTime;
		if (bestTime>finalTime || bestTime==0) bestTime=finalTime;		
		gamestate=GAMESTATE.GAMEOVER;	
	}	
	
	public static void AimMode() {
		mainCamera.enabled=false;
		puck.GetComponent<LineRenderer>().enabled=true;
		gamestate=GAMESTATE.AIM;
		//if (gamestate==GAMESTATE.AIM) GUICode.testPositive=true;
	}
	
	public static void EndAim() {
		mainCamera.enabled=true;
		puck.GetComponent<LineRenderer>().enabled=false;
		gamestate=GAMESTATE.PLAY;
	}	
	
	public static void ResetKingPedestal() {
		
		GameObject tempObject=null;
		tempObject=(GameObject) Instantiate(staticBrickPrefab,new Vector3(-150,6f,1000),Quaternion.identity);	
		tempObject.rigidbody.velocity=new Vector3(0,0,0);
		tempObject=(GameObject) Instantiate(staticBrickPrefab,new Vector3(-150,17f,1000),Quaternion.identity);
		tempObject.rigidbody.velocity=new Vector3(0,0,0);
	}	
	
	public static void ResetKing() {
		king.rigidbody.velocity=new Vector3(0,0,0);
		king.transform.position=new Vector3(-150,24.6f,1000);
		king.transform.eulerAngles=new Vector3(0,0,0);
		king.rigidbody.velocity=new Vector3(0,0,0);
		king.GetComponent<KingCode>().Stabilize();
	}	
	
	public static void ResetPuck() {
		puck.transform.position=new Vector3(0,1.5f,puckResetLocation);
		puck.transform.rotation=Quaternion.Euler(0,0,0);
		puck.rigidbody.velocity=new Vector3(0,0,0);
		MainCameraSetBehindPuck();
	}	
	
	public static void EndZoneResetPuck() {
		puck.transform.position=new Vector3(-200,1.5f,600);
		puck.transform.rotation=Quaternion.Euler(0,0,0);
		puck.rigidbody.velocity=new Vector3(0,0,0);
		MainCameraSetBehindPuck();
	
	}	
	
	public static void MainCameraSetBehindPuck() {
		mainCamera.transform.position=new Vector3(puck.transform.position.x, 50,puck.transform.position.z-100);
		mainCamera.transform.LookAt(puck.transform.position+new Vector3(0,50,0));			
	}	
	
	public static void PowerCharge() {
		currentPower+=powerChargeRate*Time.deltaTime;
		if (currentPower>maxPower) currentPower=0;
		
	}	
}
