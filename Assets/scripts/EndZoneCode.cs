using UnityEngine;
using System.Collections;

public class EndZoneCode : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
	
	void OnTriggerEnter(Collider col) {
		//Debug.Log("EndZoneCode: collider name " + col.gameObject.name);
		if (col.gameObject.name=="Puck(Clone)") col.gameObject.GetComponent<PuckCode>().CastleReset();
		
		GameObject tempSplash=(GameObject) Instantiate(PrefabManager.splashPrefab, col.gameObject.transform.position,Quaternion.identity);
		tempSplash.transform.eulerAngles=new Vector3(-90,0,0);
	}	
}
