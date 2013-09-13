using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class BrickCode : MonoBehaviour {
	
	int damage=0;
	float invulnerabilityTime=.3f;
	float invulnerabilityCounter=0;
	
	Dictionary<int, Color> damageColors=new Dictionary<int, Color>();
	
	// Use this for initialization
	void Start () {
		damageColors[0]=Color.white;
		damageColors[1]=Color.green;
		damageColors[2]=Color.yellow;
		damageColors[3]=new Color(1,.3f,0,1); //orange
		damageColors[4]=Color.red;
		damageColors[5]=Color.black;
	
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.position.y<-300) Destroy(gameObject);
		if (damage>4) Destroy(gameObject);
		
		if (invulnerabilityCounter>0) invulnerabilityCounter-=Time.deltaTime;
		if (invulnerabilityCounter<0) invulnerabilityCounter=0;
	}
	
	void OnCollisionEnter(Collision col) {
		//Debug.Log("brickcode: collision velocity" + col.relativeVelocity.magnitude);
		if (invulnerabilityCounter==0) {
			if (Mathf.Abs((int) col.relativeVelocity.magnitude)>20) { 
				MakeDust(col);
				int tempDamage=1;
				//pucks damage bricks based on their strength
				if (col.gameObject.name=="Puck(Clone)") 
				tempDamage=col.gameObject.GetComponent<PuckCode>().getStrength();
				//bricks dont damage each other unless they hit really hard
				if (col.gameObject.name=="Brick(Clone)" && col.relativeVelocity.magnitude<40)
				tempDamage=0;	
					
				
				damage+=tempDamage;
				if (damage>2) ShowDamage();
				
				if (damage<5) GetComponent<MeshRenderer>().material.SetColor("_Color",damageColors[damage]);
				invulnerabilityCounter=invulnerabilityTime;
			}	
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
