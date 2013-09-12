using UnityEngine;
using System.Collections;

public class PrefabManager : MonoBehaviour {
	
	public GameObject puckDustPrefabLoader;
	public Texture brickCrackedLoader;
	public Texture brickCrackedNormalLoader;
	
	public static GameObject puckDustPrefab=null;
	public static Texture brickCracked=null;
	public static Texture brickCrackedNormal=null;
	
	
	
	// Use this for initialization
	void Awake () {
		puckDustPrefab=puckDustPrefabLoader;
		brickCracked=brickCrackedLoader;
		brickCrackedNormal=brickCrackedNormalLoader;
		
	}		
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
