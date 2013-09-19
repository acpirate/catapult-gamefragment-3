using UnityEngine;
using System.Collections;

public class AimCameraCode : MonoBehaviour {

	float aimCameraRotationSpeed = 30;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		DIRECTION direction=DIRECTION.NONE;
		
		if (Input.GetKey(KeyCode.UpArrow) || Input.GetKey(KeyCode.W)) direction=DIRECTION.UP;
		if (Input.GetKey(KeyCode.DownArrow) || Input.GetKey(KeyCode.S)) direction=DIRECTION.DOWN;
				
		
		if (MainGameCode.gamestate==GAMESTATE.AIM && direction!=DIRECTION.NONE) {
			int rotationSwitch=1;
			if (direction==DIRECTION.UP) rotationSwitch=-1;
			
		 transform.Rotate(Vector3.right*aimCameraRotationSpeed*Time.deltaTime*rotationSwitch);
		}			
		
			
		
		
	}
}
