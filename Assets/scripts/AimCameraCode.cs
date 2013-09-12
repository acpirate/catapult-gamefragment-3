using UnityEngine;
using System.Collections;

public class AimCameraCode : MonoBehaviour {

	public GameObject puck;

	float aimCameraRotationSpeed = 30;
	
	float aimCameraFollowDistance= 30;
	float aimCameraHeightOffset=20;
	
	bool puckMoving=false;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DIRECTION direction=DIRECTION.NONE;
		
		if (Input.GetKey(KeyCode.UpArrow)) direction=DIRECTION.UP;
		if (Input.GetKey(KeyCode.DownArrow)) direction=DIRECTION.DOWN;
				
		
		if (MainGameCode.gamestate==GAMESTATE.AIM && direction!=DIRECTION.NONE) {
			int rotationSwitch=1;
			if (direction==DIRECTION.UP) rotationSwitch=-1;
			
		 transform.Rotate(Vector3.right*aimCameraRotationSpeed*Time.deltaTime*rotationSwitch);
		}			
		
			
		
		
	}
}
