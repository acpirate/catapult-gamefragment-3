using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Engine {
	
	readonly int trebuchetElevation=30;
	
	ENGINE type;
	int maxPower;
	List<int> angles=new List<int>();
	int elevation=0;
	
	public Engine(ENGINE inType, int inMaxPower, List<int> inAngles) {
		type=inType;
		maxPower=inMaxPower;
		foreach(int angle in inAngles) {
			angles.Add(angle);	
		}	
		if (type==ENGINE.TREBUCHET) elevation=trebuchetElevation;
	}	
	
	public ENGINE getType() {
		return type;
	}	
	
	public int getMaxPower() {
		return maxPower;	
	}	
	
	public List<int> getAngles() {
		return angles;	
	}	
	
	public int getElevation() {
		return elevation;	
	}	
	
	
}
