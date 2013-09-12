using UnityEngine;
using System.Collections;



public class PuckCode : MonoBehaviour {
	
	float RotationSpeed = 100;
	
	public bool puckMoving=false;
	
	bool cameraResetFlag=false;
	
	public GameObject moveCamera;
	
	// Use this for initialization
	
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
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
			MainGameCode.MainCameraSetBehindPuck();
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
	}
	
}
