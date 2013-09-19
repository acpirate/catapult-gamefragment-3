using UnityEngine;
using System.Collections;
using System.Collections.Generic;
	
public enum GAMESTATE { TITLE, PLAY, GAMEOVER, SETTINGS, AIM };
public enum DIRECTION { LEFT, RIGHT, NONE, UP, DOWN};
public enum ENGINE { BALLISTA, CATAPULT, TREBUCHET};
public enum PUCKCLASS {WARRIOR, WIZARD, ROGUE, PRIEST};

public class MainGameCode : MonoBehaviour {
	
	public static Dictionary<ENGINE,Engine> engines=new Dictionary<ENGINE,Engine>();
	
	public static GameObject king=null;
	//public static GameObject puck=null;
	public static Camera mainCamera=null;

	static int wallWidth=11;
	static int wallHeight=10;
	static float brickWidth=20f;
	static float brickHeight=10f;
	static float brickHeightOffset=3.8f;
	static float wallXStart=-300;
	static float wallZStart=950;
	static float towerHeight=11;
	static float leftTowerXStart=-340;
	static float leftTowerZStart=940;
	static float rightTowerXStart=-80;
	static float rightTowerZStart=940;	
	
	static float throneXStart=-215;
	static float throneZStart=1020;
	
	public static ENGINE selectedEngine = ENGINE.BALLISTA;
	public static int angleSelectNumber=0;
	
	//party of pucks
	public static List<GameObject> party=new List<GameObject>();
	public static GameObject selectedPuck=null;
	public static int partySize=4;
	
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
	
	public static float brickTransparencyRadius=30;
	
	public static GAMESTATE gamestate=GAMESTATE.TITLE;
	
	public static List<GameObject> bricksInTheWay=new List<GameObject>();

	// Use this for initialization
	void Awake() {
		
		if (engines.Count==0) InitializeEngines();
		if (king==null) king=GameObject.Find("King");
		//if (puck==null) puck=GameObject.Find("Puck");
		if (mainCamera==null) mainCamera=GameObject.Find("Main Camera").GetComponent<Camera>();	
	}	
	
	void Start () {
		BuildTower();
		BuildWall();
		BuildThrone();
		ResetKing();
		
		
		InitializeParty();
		setSelectedPuck(party[0]);	
		Debug.Break();
		//mainCamera.transform.LookAt(party[0].transform.position);
	}
	
	// Update is called once per frame
	void Update () {

		
		//turn off main camera if the puck is moving
		if (selectedPuck!=null && selectedPuck.GetComponent<PuckCode>().getPuckMoving()) {mainCamera.enabled=false; } 
		if (selectedPuck!=null && !selectedPuck.GetComponent<PuckCode>().getPuckMoving() && gamestate!=GAMESTATE.AIM) { 
			mainCamera.enabled=true;
			//mainCamera.transform.position=puck.GetComponent<PuckCode>().moveCamera.transform.position;
			//mainCamera.transform.position=new Vector3(mainCamera.transform.position.x, Mathf.Abs(mainCamera.transform.position.y),mainCamera.transform.position.z);
		}		
		//reset the puck if it falls off
		if (gamestate!=GAMESTATE.GAMEOVER && gamestate!=GAMESTATE.SETTINGS && gamestate!=GAMESTATE.TITLE) gameTime+=Time.deltaTime;
		if (gamestate==GAMESTATE.TITLE) { ResetTimers(); }
		

	
		BrickTransparency();
		TurnPhysicsOffAimingPuck();
		
		
	}
	
	public static void TurnPhysicsOffAimingPuck() {
		foreach (GameObject puck in party) {
			puck.rigidbody.isKinematic=false;
		}	
		if (gamestate==GAMESTATE.AIM) selectedPuck.rigidbody.isKinematic=true;
	}	
	
	public static void BrickTransparency() {
		//make previously transparent bricks opaque
		foreach (GameObject brick in bricksInTheWay) {
			if (brick!=null)
			brick.GetComponent<BrickCode>().SetTranparent(false);	
		}		
		bricksInTheWay.Clear();		
		Vector3 transparentStart=mainCamera.transform.position;
		if (gamestate==GAMESTATE.AIM) transparentStart=selectedPuck.GetComponent<PuckCode>().aimCamera.transform.position;
		if (selectedPuck.GetComponent<PuckCode>().getPuckMoving()) transparentStart=selectedPuck.GetComponent<PuckCode>().moveCamera.transform.position;
		

		RaycastHit[] hits;
		Vector3 heading=selectedPuck.transform.position - transparentStart;
		float distance=heading.magnitude;
        hits = Physics.RaycastAll(transparentStart, heading, distance);
		
		foreach(RaycastHit hit in hits) {
			if (hit.collider.gameObject.name=="Brick(Clone)" || hit.collider.gameObject.name=="Brick") {
				bricksInTheWay.Add(hit.collider.gameObject);
				Collider[] hitColliders = Physics.OverlapSphere(hit.collider.gameObject.transform.position, brickTransparencyRadius);
				foreach(Collider col in hitColliders) {
					if (col.gameObject.name=="Brick(Clone)" || col.gameObject.name=="Brick") {
						if (!bricksInTheWay.Contains(col.gameObject)) bricksInTheWay.Add(col.gameObject);
					}	
				}
			}	
		}	
	
		//set bricks to transparent
		foreach (GameObject brick in bricksInTheWay) {
			brick.GetComponent<BrickCode>().SetTranparent(true);	
		}		
	}	
	
	public static void setSelectedPuck(GameObject inSelectedPuck) {
		if (gamestate==GAMESTATE.AIM) selectedPuck.GetComponentInChildren<LineRenderer>().enabled=false;
		selectedPuck=inSelectedPuck;
		if (gamestate==GAMESTATE.AIM) selectedPuck.GetComponentInChildren<LineRenderer>().enabled=true;
		selectedPuck.GetComponent<PuckCode>().MainCameraSetBehindPuck();
	}	
	
	public static void InitializeParty() {
		int partyCount=party.Count;
		for (int counter=0;counter<partyCount;counter++){
			Destroy(party[counter]);
		}	
			party.Clear();
		
			party.Add((GameObject) Instantiate(PrefabManager.puckPrefab,getPuckStartLocation(0),Quaternion.identity));
			party[party.Count-1].GetComponent<PuckCode>().setClass(PUCKCLASS.WARRIOR);
			party.Add((GameObject) Instantiate(PrefabManager.puckPrefab,getPuckStartLocation(1),Quaternion.identity));
			party[party.Count-1].GetComponent<PuckCode>().setClass(PUCKCLASS.WIZARD);
			party.Add((GameObject) Instantiate(PrefabManager.puckPrefab,getPuckStartLocation(2),Quaternion.identity));
			party[party.Count-1].GetComponent<PuckCode>().setClass(PUCKCLASS.PRIEST);	
			party.Add((GameObject) Instantiate(PrefabManager.puckPrefab,getPuckStartLocation(3),Quaternion.identity));
			party[party.Count-1].GetComponent<PuckCode>().setClass(PUCKCLASS.ROGUE);			
		
		
	}
	
	public static Vector3 getPuckStartLocation(int puckIndexNumber) {
		float yPosition=PrefabManager.startZone.transform.position.y;
		float zPosition=PrefabManager.startZone.transform.position.z;
		float tempX=PrefabManager.startZone.transform.position.x - PrefabManager.startZone.transform.localScale.x/2;
		
		float xPosition=tempX+PrefabManager.startZone.transform.localScale.x/(partySize+1)*(puckIndexNumber+1);
		
		return new Vector3(xPosition,yPosition,zPosition);
		
	}
	
	public static Vector3 getPuckMidfieldLocation(int puckIndexNumber) {
		float yPosition=PrefabManager.midZone.transform.position.y;
		float zPosition=PrefabManager.midZone.transform.position.z;
		float tempX=PrefabManager.midZone.transform.position.x - PrefabManager.midZone.transform.localScale.x/2;
		
		float xPosition=tempX+PrefabManager.midZone.transform.localScale.x/(partySize+1)*(puckIndexNumber+1);
		
		return new Vector3(xPosition,yPosition,zPosition);		
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
	
	static void BuildThrone() {
		float halfBrickWidth=brickWidth/2;
		//level 1
		Vector3 level1LeftBrickPosition=new Vector3(throneXStart,brickHeightOffset,throneZStart);
		Vector3 level1MiddleLeftBrickPosition=level1LeftBrickPosition+new Vector3(0+halfBrickWidth,0,0);
		Vector3 level1MiddleRightBrickPosition=level1LeftBrickPosition+new Vector3(0+brickWidth,0,0);
		Vector3 level1RightBrickPosition=level1LeftBrickPosition+new Vector3(0+brickWidth*1.5f,0,0);
	
		Instantiate(PrefabManager.brickPrefab,level1LeftBrickPosition,Quaternion.Euler(0,90,0));
		Instantiate(PrefabManager.brickPrefab,level1MiddleLeftBrickPosition,Quaternion.Euler(0,90,0));					
		Instantiate(PrefabManager.brickPrefab,level1MiddleRightBrickPosition,Quaternion.Euler(0,90,0));
		Instantiate(PrefabManager.brickPrefab,level1RightBrickPosition,Quaternion.Euler(0,90,0));		
		
		//level 2
		Vector3 level2LeftBrickPosition=level1LeftBrickPosition+new Vector3(0,halfBrickWidth,0);
		Vector3 level2UpperMiddleBrickPosition=level2LeftBrickPosition+new Vector3(brickWidth*.75f,0,halfBrickWidth*.5f);
		Vector3 level2RightBrickPosition=level2LeftBrickPosition+new Vector3(halfBrickWidth*3,0,0);
		
		Instantiate(PrefabManager.brickPrefab,level2LeftBrickPosition,Quaternion.Euler(0,90,0));
		Instantiate(PrefabManager.brickPrefab,level2UpperMiddleBrickPosition,Quaternion.Euler(0,0,0));
		Instantiate(PrefabManager.brickPrefab,level2RightBrickPosition,Quaternion.Euler(0,90,0));
				
	}	
	
	static void BuildWall() {
		float brickHalfWidth=brickWidth/2;
		for (int yCounter=0;yCounter<wallHeight;yCounter++) {
			for (int xCounter=0;xCounter<wallWidth;xCounter++) {
				float brickOffset=0;
				if (yCounter%2==1) brickOffset=brickHalfWidth;
				Vector3 brickLocation=new Vector3(wallXStart+xCounter*brickWidth+brickOffset,brickHeight*yCounter+brickHeightOffset,wallZStart);
				bool destroyWallBrick=true;
				if (xCounter!=wallWidth-1) destroyWallBrick=false;
				if (yCounter%2!=1) destroyWallBrick=false;
				if (!destroyWallBrick) Instantiate(PrefabManager.brickPrefab,brickLocation,Quaternion.identity);
			}	
		}	
	}	
	
	static void BuildTower() {
		float brickHalfWidth=brickWidth/2;
		
		for (int levelCounter=0;levelCounter<towerHeight;levelCounter++) {
		//int levelCounter=0;	
			//left tower
			float brickOffset=0;
			if (levelCounter%2==1) brickOffset=brickHalfWidth;
			//front bricks
			Vector3 frontBrick1Location=new Vector3(leftTowerXStart+brickOffset,brickHeight*levelCounter+brickHeightOffset,leftTowerZStart);
			Vector3 frontBrick2Location=frontBrick1Location+new Vector3(brickWidth,0,0);
			Instantiate(PrefabManager.brickPrefab,frontBrick1Location,Quaternion.identity);
			Instantiate(PrefabManager.brickPrefab,frontBrick2Location,Quaternion.identity);
			//rear bricks
			Vector3 rearBrick1Location=new Vector3(leftTowerXStart-brickOffset+brickHalfWidth,brickHeight*levelCounter+brickHeightOffset,leftTowerZStart+4*brickHalfWidth);
			Vector3 rearBrick2Location=rearBrick1Location+new Vector3(brickWidth,0,0);
			Instantiate(PrefabManager.brickPrefab,rearBrick1Location,Quaternion.identity);
			Instantiate(PrefabManager.brickPrefab,rearBrick2Location,Quaternion.identity);
			//left bricks
			Vector3 leftBrick1Location=new Vector3(leftTowerXStart-brickHalfWidth/2,brickHeight*levelCounter+brickHeightOffset,leftTowerZStart+brickWidth*.75f-brickOffset);
			Vector3 leftBrick2Location=leftBrick1Location+new Vector3(0,0,brickWidth);
			Instantiate(PrefabManager.brickPrefab,leftBrick1Location,Quaternion.Euler(new Vector3(0,90,0)));
			Instantiate(PrefabManager.brickPrefab,leftBrick2Location,Quaternion.Euler(new Vector3(0,90,0)));
			//right bricks
			Vector3 rightBrick1Location=new Vector3(leftTowerXStart-brickHalfWidth/2+4*brickHalfWidth,brickHeight*levelCounter+brickHeightOffset,leftTowerZStart+brickWidth*.75f+brickOffset-brickHalfWidth);
			Vector3 rightBrick2Location=rightBrick1Location+new Vector3(0,0,brickWidth);
			
			if (levelCounter%2==1 || levelCounter==towerHeight-1) {
				Instantiate(PrefabManager.brickPrefab,rightBrick1Location,Quaternion.Euler(new Vector3(0,90,0)));
			}
			else {
				if (levelCounter<towerHeight-1)
				Instantiate(PrefabManager.halfBrickPrefab,rightBrick1Location+new Vector3(0,0,brickHalfWidth/2-brickHalfWidth),Quaternion.identity);				 
			}	
			Instantiate(PrefabManager.brickPrefab,rightBrick2Location,Quaternion.Euler(new Vector3(0,90,0))); 
			
			//right tower
			//right front bricks
			Vector3 rightFrontBrick1Location=new Vector3(rightTowerXStart-brickOffset,brickHeight*levelCounter+brickHeightOffset,rightTowerZStart);
			Vector3 rightFrontBrick2Location=rightFrontBrick1Location+new Vector3(brickWidth,0,0);
			Instantiate(PrefabManager.brickPrefab,rightFrontBrick1Location,Quaternion.identity);
			Instantiate(PrefabManager.brickPrefab,rightFrontBrick2Location,Quaternion.identity);
			//right rear bricks
			Vector3 rightRearBrick1Location=new Vector3(rightTowerXStart+brickOffset-brickHalfWidth,brickHeight*levelCounter+brickHeightOffset,rightTowerZStart+4*brickHalfWidth);
			Vector3 rightRearBrick2Location=rightRearBrick1Location+new Vector3(brickWidth,0,0);
			Instantiate(PrefabManager.brickPrefab,rightRearBrick1Location,Quaternion.identity);
			Instantiate(PrefabManager.brickPrefab,rightRearBrick2Location,Quaternion.identity);			
			//right leftside bricks
			Vector3 rightTowerLeftBrick1Location=new Vector3(rightTowerXStart-brickHalfWidth*1.5f,brickHeight*levelCounter+brickHeightOffset,rightTowerZStart+brickWidth*.25f+brickOffset);
			Vector3 rightTowerLeftBrick2Location=rightTowerLeftBrick1Location+new Vector3(0,0,brickWidth);
			if (levelCounter%2==1 || levelCounter==towerHeight-1) {
				Instantiate(PrefabManager.brickPrefab,rightTowerLeftBrick1Location,Quaternion.Euler(new Vector3(0,90,0))); }
			else  {
				if (levelCounter<towerHeight-1)
				Instantiate(PrefabManager.halfBrickPrefab,rightTowerLeftBrick1Location+new Vector3(0,0,brickHalfWidth/2-brickHalfWidth),Quaternion.Euler(new Vector3(0,90,0)));					
			}	
			Instantiate(PrefabManager.brickPrefab,rightTowerLeftBrick2Location,Quaternion.Euler(new Vector3(0,90,0)));			
			//right rightside bricks
			Vector3 rightTowerRightBrick1Location=new Vector3(rightTowerXStart-brickHalfWidth*1.5f+4*brickHalfWidth,brickHeight*levelCounter+brickHeightOffset,rightTowerZStart+brickWidth*.25f-brickOffset+brickHalfWidth);
			Vector3 rightTowerRightBrick2Location=rightTowerRightBrick1Location+new Vector3(0,0,brickWidth);
			Instantiate(PrefabManager.brickPrefab,rightTowerRightBrick1Location,Quaternion.Euler(new Vector3(0,90,0)));
			Instantiate(PrefabManager.brickPrefab,rightTowerRightBrick2Location,Quaternion.Euler(new Vector3(0,90,0)));	
		
		
		}
	}	
	
	
	
	public static void ShootPuck() {
		//Debug.Log("maingamecode: shoot code selectedpuck cooldown" + selectedPuck.GetComponent<PuckCode>().currentCooldown);
		if (selectedPuck.GetComponent<PuckCode>().currentCooldown<=0) {
			//turn on puck physics after aiming
			selectedPuck.rigidbody.isKinematic=false;
			Engine tempEngine=engines[selectedEngine];
			
			int tempMaxPower=tempEngine.getMaxPower();
			
			EndAim();
			selectedPuck.transform.position+=new Vector3(0,tempEngine.getElevation(),0);
			
			int tempAngle=engines[selectedEngine].getAngles()[angleSelectNumber];
			//puck normalized forward is the starting point
			Vector3 shootVector=selectedPuck.transform.forward;
			//rotated vector to shooting angle
			
			
			
			GameObject tempRotatingObject=new GameObject("tempGameObject");
			
			tempRotatingObject.transform.rotation=selectedPuck.transform.rotation;
			
			//Debug.Log("maingamecode: shootvector " + tempRotatingObject.transform.forward);
			
			tempRotatingObject.transform.eulerAngles+=new Vector3(-1*tempAngle,0,0);
			
			shootVector=tempRotatingObject.transform.forward;
			
			Destroy(tempRotatingObject);
			
			//Debug.Log("maingamecode: tempAngle " + tempAngle);
			
			//shootVector=Quaternion.AngleAxis(-1*tempAngle,puck.transform.right)*shootVector;
			
			//Debug.Log("maingamecode: rotated shootvector " + shootVector);
					
			//add power multipliers
			shootVector*=currentPower/100*tempMaxPower*powerMultiplier;
			//puck.transform.eulerAngles+=new Vector3(0,tempAngle*-1,0);
			
			selectedPuck.rigidbody.AddForce(shootVector);
			if (tempEngine.getType()!=ENGINE.BALLISTA)
				selectedPuck.rigidbody.AddRelativeTorque(new Vector3(currentPower/100*maxPower*powerMultiplier*10,0,0));
			
			
			selectedPuck.GetComponent<PuckCode>().DoCooldown();
		} else selectedPuck.GetComponent<PuckCode>().currentCooldownWarningTime=selectedPuck.GetComponent<PuckCode>().maxCooldownWarningTime;	
		currentPower=0;
	}	
		
	
	public static void QuitGame() {
		ClearBricks();
		
		InitializeParty();
		setSelectedPuck(party[0]);
		BuildWall();
		BuildTower();
		ResetKing();		
		BuildThrone();
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
		selectedPuck.GetComponent<LineRenderer>().enabled=true;
		gamestate=GAMESTATE.AIM;
		//if (gamestate==GAMESTATE.AIM) GUICode.testPositive=true;
	}
	
	public static void EndAim() {
		mainCamera.enabled=true;
		foreach (GameObject puck in party) puck.GetComponent<LineRenderer>().enabled=false;
		gamestate=GAMESTATE.PLAY;
	}		
	
	public static void ResetKing() {
		king.rigidbody.velocity=new Vector3(0,0,0);
		king.transform.position=new Vector3(-200,11.2f,1015);
		king.transform.eulerAngles=new Vector3(0,0,0);
		king.rigidbody.velocity=new Vector3(0,0,0);
		king.GetComponent<KingCode>().Stabilize();
	}		
	
	public static void PowerCharge() {
		currentPower+=powerChargeRate*Time.deltaTime;
		if (currentPower>maxPower) currentPower=0;
		
	}	
}
