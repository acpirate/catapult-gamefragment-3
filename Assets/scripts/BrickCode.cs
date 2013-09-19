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
		string hitName=col.gameObject.name;
		PuckCode tempPuckCode=null;
		if (hitName=="Puck(Clone)") tempPuckCode=col.gameObject.GetComponent<PuckCode>();
		
		//Debug.Log("brickcode: collision velocity" + col.relativeVelocity.magnitude);
		if (invulnerabilityCounter==0) {
			if (Mathf.Abs((int) col.relativeVelocity.magnitude)>20) { 
				
				int tempDamage=1;
				//pucks damage bricks based on their strength
				if (hitName=="Puck(Clone)") 
				tempDamage=col.gameObject.GetComponent<PuckCode>().getStrength();
				//bricks dont damage each other unless they hit really hard
				if ((col.gameObject.name=="Brick(Clone)" || col.gameObject.name=="Brick" || col.gameObject.name=="HalfBrick(Clone)") && col.relativeVelocity.magnitude<40)
				tempDamage=0;	
					
				
				BrickDamage(tempDamage);
	
			}	
		}
		if (hitName=="Puck(Clone)") {
			if (tempPuckCode.getClass()==PUCKCLASS.WIZARD && !tempPuckCode.effectFired) {
				WizardExplosion(tempPuckCode);
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
	
	void WizardExplosion(PuckCode inPuckCode) {
		Instantiate(PrefabManager.shockFlashPrefab,inPuckCode.gameObject.transform.position,Quaternion.Euler(new Vector3(-90,0,0)));
		Collider[] hitColliders = Physics.OverlapSphere(inPuckCode.gameObject.transform.position, inPuckCode.wizardExplosionRadius);
		foreach (Collider col in hitColliders) {
			if (col.gameObject.name=="Brick(Clone)" || col.gameObject.name=="Brick" || col.gameObject.name=="HalfBrick(Clone)") {
				col.gameObject.GetComponent<BrickCode>().BrickDamage(2);	
			}	
		}	
		
		
		inPuckCode.effectFired=true;		
	}	
	
	public void BrickDamage(int damageAmount) {
		damage+=damageAmount;
		if (damage>2) ShowDamage();
		
		if (damage<5) {
			Color tempColor=damageColors[damage];
			GetComponent<MeshRenderer>().material.SetColor("_Color",tempColor);
			GetComponent<MeshRenderer>().material.SetColor("_GridColor",getTransparentDamageColor());
		}	
		if (damageAmount>0) { 
			invulnerabilityCounter=invulnerabilityTime;
			//MakeDust(col);
		}		
	}
	
	public void SetTranparent(bool isTransparent) {
		if (isTransparent) {
			GetComponent<MeshRenderer>().material.shader=PrefabManager.transparentVectorShader;
			GetComponent<MeshRenderer>().material.SetColor("_GridColor",getTransparentDamageColor()); 
			GetComponent<MeshRenderer>().material.SetFloat("_LineWidth",.0f);
		}
		else {
			GetComponent<MeshRenderer>().material=PrefabManager.brickMaterial;
			if (damage>2) ShowDamage();
			if (damage<5)
			GetComponent<MeshRenderer>().material.SetColor("_Color",damageColors[damage]);					
		}	
	}	
	
	public Color getTransparentDamageColor() {
		Color tempColor=Color.white;
		if (damage<=5) tempColor=damageColors[damage];
		tempColor=new Color(tempColor.r, tempColor.g, tempColor.b, .20f);
		
		return tempColor;
	}	
	
	
}
