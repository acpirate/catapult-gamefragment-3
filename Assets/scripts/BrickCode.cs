using UnityEngine;
using System.Collections;

public class BrickCode : MonoBehaviour {
	
	int damage=0;
	
	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y<-300) Destroy(gameObject);
		if (damage>1) Destroy(gameObject);
		
		
	
	}
	
	void OnCollisionEnter(Collision col) {
		//Debug.Log("brickcode: collision velocity" + col.relativeVelocity.magnitude);
		if (Mathf.Abs((int) col.relativeVelocity.magnitude)>20) { 
			MakeDust(col);
			damage++;
			ShowDamage();
		}	
	}	
	
	void MakeDust(Collision col) {
		Instantiate(PrefabManager.puckDustPrefab,col.contacts[0].point,Quaternion.Euler(new Vector3(-90,0,0)));
	}	
	
	void ShowDamage() {
		GetComponent<MeshRenderer>().material.SetTexture("_MainTex",PrefabManager.brickCracked);
		GetComponent<MeshRenderer>().material.SetTexture("_BumpMap",PrefabManager.brickCrackedNormal);
	}	
	
}
