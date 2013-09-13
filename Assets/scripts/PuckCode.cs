using UnityEngine;
using System.Collections;



public class PuckCode : MonoBehaviour {
	
	float RotationSpeed = 100;
	
	public bool puckMoving=false;
	
	public bool cameraResetFlag=false;
	
	bool castleResetFlag=false;
	float castleResetTime=3f;
	float castleResetCountdown=0;
	
	public GameObject moveCamera;
	
	PUCKCLASS puckClass;
	
	public GameObject aimCamera;
	
	float maxCooldown=120;
	float currentCooldown=0;
	int strength=0;
	
	
	// Use this for initialization
	
	void Start () {
		//hardcode ftw
		aimCamera=transform.GetChild(0).gameObject;
		moveCamera=transform.GetChild(1).gameObject;
		
	}
	
	// Update is called once per frame
	void Update () {
		//turn on and off aim cameras
		aimCamera.GetComponent<Camera>().enabled=false;
		if (MainGameCode.selectedPuck==gameObject) {
			aimCamera.GetComponent<Camera>().enabled=true;			
		}	
		
		
		DIRECTION direction=DIRECTION.NONE;
		//Debug.Log(rigidbody.velocity.magnitude);
		
		
		if (rigidbody.velocity.magnitude>1) puckMoving=true;
		if (puckMoving) {
			cameraResetFlag=true;
			moveCamera.GetComponent<Camera>().enabled=true;
			moveCamera.transform.position=transform.position + rigidbody.velocity.normalized*-100;
			if (moveCamera.transform.position.y<20) moveCamera.transform.position=new Vector3(moveCamera.transform.position.x, 20, moveCamera.transform.position.z);
			moveCamera.transform.LookAt(transform.position);
		}	
		
		if (cameraResetFlag && !puckMoving) {
			cameraResetFlag=false;
			MainCameraSetBehindPuck();
		}	
		
		if (puckMoving && rigidbody.velocity.magnitude<1) {
			puckMoving=false;
			transform.rotation=Quaternion.Euler(0,0,0);
			moveCamera.GetComponent<Camera>().enabled=false;
		}	
		
		if (Input.GetKey(KeyCode.LeftArrow)) direction=DIRECTION.LEFT;
		if (Input.GetKey(KeyCode.RightArrow)) direction=DIRECTION.RIGHT;
		
		if (MainGameCode.gamestate==GAMESTATE.AIM && direction!=DIRECTION.NONE) {
			int rotationSwitch=1;
			if (direction==DIRECTION.LEFT) rotationSwitch=-1;
			
		 transform.Rotate(0, (RotationSpeed*Time.deltaTime*rotationSwitch), 0, Space.World); 
		}	
		//reset the puck if it falls off
		if (transform.position.y<-100) ResetPuck();
		//run the cooldown timer
		if (currentCooldown>0 && MainGameCode.gamestate!=GAMESTATE.SETTINGS) currentCooldown-=Time.deltaTime;
		
		if (castleResetFlag) {
			castleResetCountdown-=Time.deltaTime;
			if (castleResetCountdown<0) {
				castleResetFlag=false;
				ResetPuckMid();
			}	
		}
	}
			
	public void CastleReset() {
			castleResetFlag=true;
			castleResetCountdown=castleResetTime;
		}	
	
	public void setClass(PUCKCLASS inPuckClass) {
		puckClass=inPuckClass;
		switch (puckClass) {
		case PUCKCLASS.PRIEST:
			strength=2;
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.priestTexture);
		break;
		case PUCKCLASS.ROGUE:
			strength=1;
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.rogueTexture);
		break;
		case PUCKCLASS.WIZARD:
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.wizardTexture);
			strength=0;
		break;
		case PUCKCLASS.WARRIOR:
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.warriorTexture);
			strength=3;
		break;
		}	
		
	}	
	
	public int getStrength() {
		return strength;	
	}				
			
	public void ResetPuck() {
		currentCooldown=maxCooldown;
		transform.position=MainGameCode.getPuckStartLocation(MainGameCode.party.IndexOf(gameObject));
		transform.rotation=Quaternion.Euler(0,0,0);
		rigidbody.velocity=new Vector3(0,0,0);	
	}	
	
	public void ResetPuckMid() {
		currentCooldown=maxCooldown;
		transform.position=MainGameCode.getPuckMidfieldLocation(MainGameCode.party.IndexOf(gameObject));
		transform.rotation=Quaternion.Euler(0,0,0);
		rigidbody.velocity=new Vector3(0,0,0);	
	}
	
	
	public void MainCameraSetBehindPuck() {
		//Debug.Log("puckcode: setting main camera");
		MainGameCode.mainCamera.transform.position=new Vector3(transform.position.x, 50,transform.position.z-100);
		MainGameCode.mainCamera.transform.LookAt(transform.position+new Vector3(0,50,0));			
	}	
	
}
