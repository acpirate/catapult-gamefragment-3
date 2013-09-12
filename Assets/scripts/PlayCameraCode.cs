using UnityEngine;
using System.Collections;

public class PlayCameraCode : MonoBehaviour {
	
	public int camSpeed;
	public int runningCamSpeed;
	
	public int rotationSpeed;
	public int runningRotationSpeed;
	
	float yPlane;
	
	
	// Use this for initialization
	void Start () {
		transform.LookAt(MainGameCode.puck.transform.position);
		yPlane=transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
		if (MainGameCode.gamestate==GAMESTATE.PLAY)
			MoveCamera();	
	}
	
	void MoveCamera() {
		Vector3 newPosition=transform.position;
		
		if (Input.GetKey(KeyCode.W)) {
			int camMoveSpeed=camSpeed;
			if (Input.GetKey(KeyCode.LeftShift)) camMoveSpeed=runningCamSpeed;
			//Debug.Log("playcamera: movinc camera");
			newPosition=transform.position+transform.forward.normalized*camMoveSpeed*Time.deltaTime;		
		}
		if (Input.GetKey(KeyCode.S)) {
			int camMoveSpeed=camSpeed;
			if (Input.GetKey(KeyCode.LeftShift)) camMoveSpeed=runningCamSpeed;
			//Debug.Log("playcamera: movinc camera");
			newPosition=transform.position-transform.forward.normalized*camMoveSpeed*Time.deltaTime;		
		}		
		if (Input.GetKey(KeyCode.A)) {
			int camMoveSpeed=camSpeed;
			if (Input.GetKey(KeyCode.LeftShift)) camMoveSpeed=runningCamSpeed;
			//Debug.Log("playcamera: movinc camera");
			newPosition=transform.position-transform.right.normalized*camMoveSpeed*Time.deltaTime;		
			
		}
		if (Input.GetKey(KeyCode.D)) {
			int camMoveSpeed=camSpeed;
			if (Input.GetKey(KeyCode.LeftShift)) camMoveSpeed=runningCamSpeed;
			//Debug.Log("playcamera: movinc camera");
			newPosition=transform.position+transform.right.normalized*camMoveSpeed*Time.deltaTime;		
			
		}
		if (Input.GetKey(KeyCode.E)) {
			int camRotate=rotationSpeed;
			if (Input.GetKey(KeyCode.LeftShift)) camRotate=runningRotationSpeed;
			transform.eulerAngles+=new Vector3(0,camRotate*Time.deltaTime,0);
		}	
		if (Input.GetKey(KeyCode.Q)) {
			int camRotate=rotationSpeed;
			if (Input.GetKey(KeyCode.LeftShift)) camRotate=runningRotationSpeed;			
			transform.eulerAngles-=new Vector3(0,camRotate*Time.deltaTime,0);
		}		
		transform.position=newPosition;
		transform.position=new Vector3(transform.position.x, yPlane,transform.position.z);		
		
	}	
}
