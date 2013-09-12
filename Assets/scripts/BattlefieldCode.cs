using UnityEngine;
using System.Collections;

public class BattlefieldCode : MonoBehaviour {
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		
	}
	
	void OnCollisionEnter(Collision col) {
		if (col.collider.gameObject.name=="Puck") MakeDust(col);
	}	
	
	void MakeDust(Collision col) {
		if (Mathf.Abs((int) col.relativeVelocity.y) >10 )
		Instantiate(PrefabManager.puckDustPrefab,col.contacts[0].point,Quaternion.Euler(new Vector3(-90,0,0)));
	}	
	
}
