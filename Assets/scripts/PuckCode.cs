using UnityEngine;
using System.Collections;



public class PuckCode : MonoBehaviour {
	
	float RotationSpeed = 100;
	
	bool puckMoving=false;
	
	bool cameraResetFlag=false;
	
	[HideInInspector]
	public bool castleResetFlag=false;
	
	float castleResetTime=3f;
	float castleResetCountdown=0;
	
	[HideInInspector]
	public GameObject moveCamera;
	
	PUCKCLASS puckClass;
	
	[HideInInspector]
	public GameObject aimCamera;
	
	[HideInInspector]
	public float maxCooldown=30;
	
	[HideInInspector]
	public float currentCooldown=0;
	
	int strength=0;
	
	[HideInInspector]
	public float maxCooldownWarningTime=3;
	
	[HideInInspector]
	public float currentCooldownWarningTime=0;

	[HideInInspector]
	public bool effectFired=false;
	
	[HideInInspector]
	public float wizardExplosionRadius=30;
	
	[HideInInspector]
	public float priestExplosionRadius=300;
	
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
		
		
		if (rigidbody.velocity.magnitude>3) puckMoving=true;
		if (puckMoving) {
			cameraResetFlag=true;
			moveCamera.GetComponent<Camera>().enabled=true;
			moveCamera.transform.position=transform.position + rigidbody.velocity.normalized*-100;
			if (moveCamera.transform.position.y<20) moveCamera.transform.position=new Vector3(moveCamera.transform.position.x, 20, moveCamera.transform.position.z);
			moveCamera.transform.LookAt(transform.position);
		}	
		
		if (cameraResetFlag && !puckMoving) {
			//Debug.Log("puckcode: in camera reset flag");
			cameraResetFlag=false;
			if (MainGameCode.selectedPuck==gameObject) MainCameraSetBehindPuck();
		}	
		
		if (puckMoving && rigidbody.velocity.magnitude<1) {
			puckMoving=false;
			transform.rotation=Quaternion.Euler(0,0,0);
			moveCamera.GetComponent<Camera>().enabled=false;
			if (puckClass==PUCKCLASS.PRIEST) {
				PriestExplosion();
			}	
			
		}	
		
		if (Input.GetKey(KeyCode.LeftArrow) || Input.GetKey(KeyCode.Q) || Input.GetKey(KeyCode.A)) direction=DIRECTION.LEFT;
		if (Input.GetKey(KeyCode.RightArrow) || Input.GetKey(KeyCode.E) || Input.GetKey(KeyCode.D)) direction=DIRECTION.RIGHT;
		
		if (MainGameCode.gamestate==GAMESTATE.AIM && direction!=DIRECTION.NONE && MainGameCode.selectedPuck==gameObject) {
			int rotationSwitch=1;
			if (direction==DIRECTION.LEFT) rotationSwitch=-1;
			
		 transform.Rotate(0, (RotationSpeed*Time.deltaTime*rotationSwitch), 0, Space.World); 
		}	
		//reset the puck if it falls off
		if (transform.position.y<-100) ResetPuck();
		//run the cooldown timer
		if (currentCooldown>0 && MainGameCode.gamestate!=GAMESTATE.SETTINGS) currentCooldown-=Time.deltaTime;
		//run cooldown warning timer
		if (currentCooldownWarningTime>0 && MainGameCode.gamestate!=GAMESTATE.SETTINGS) currentCooldownWarningTime-=Time.deltaTime;
		
		if (castleResetFlag) {
			castleResetCountdown-=Time.deltaTime;
			if (castleResetCountdown<0) {
				castleResetFlag=false;
				ResetPuckMid();
			}	
		}
		if (MainGameCode.selectedPuck!=gameObject) moveCamera.GetComponent<Camera>().enabled=false;
		
		if (currentCooldown<=0 && rigidbody.velocity.magnitude<3) effectFired=false;
		
	}
	
	void PriestExplosion() {
		Instantiate(PrefabManager.holyExplosionPrefab,transform.position,Quaternion.identity);	
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, priestExplosionRadius);
		foreach (Collider hit in hitColliders) {
			if (hit.gameObject.name=="Puck(Clone)") hit.gameObject.GetComponent<PuckCode>().currentCooldown*=.5f; 	
		}	
	}	
	
	public void DoCooldown() {
		currentCooldown=maxCooldown;	
	}	
	
	
	public bool getPuckMoving() {
		return puckMoving;
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
			//rigidbody.mass=.5f;
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.priestTexture);
		break;
		case PUCKCLASS.ROGUE:
			strength=1;
			//rigidbody.mass=.2f;
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.rogueTexture);
		break;
		case PUCKCLASS.WIZARD:
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.wizardTexture);
			strength=0;
			//rigidbody.mass=.1f;
		break;
		case PUCKCLASS.WARRIOR:
			transform.GetComponentInChildren<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.warriorTexture);
			strength=3;
		break;
		}	
		
	}	
	
	public PUCKCLASS getClass() {
		return puckClass;	
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
