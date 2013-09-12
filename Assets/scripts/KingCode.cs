using UnityEngine;
using System.Collections;

public class KingCode : MonoBehaviour {
	
	float stabilizeTimer=0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		//if (transform.position.y<-300) Destroy(gameObject);
		
		//if the king gets knocked off pedestal then its gameover
		if (transform.position.y<20) MainGameCode.GameOver();
		
		if (stabilizeTimer>0) stabilizeTimer-=Time.deltaTime;
		if (stabilizeTimer<0)  {
			stabilizeTimer=0;
			rigidbody.WakeUp(); }
		
	}
	
	public void Stabilize() {
		rigidbody.Sleep();
		stabilizeTimer=2;
		
	}	
}
