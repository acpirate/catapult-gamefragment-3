using UnityEngine;
using System.Collections;

public class PrefabManager : MonoBehaviour {
	
	public GameObject puckDustPrefabLoader;
	public Texture brickCrackedLoader;
	public Texture brickCrackedNormalLoader;
	public GameObject puckPrefabLoader;
	public GameObject startZoneLoader;
	public GameObject midZoneLoader;
	public Texture warriorTextureLoader;
	public Texture priestTextureLoader;
	public Texture wizardTextureLoader;
	public Texture rogueTextureLoader;
	public GameObject splashPrefabLoader;
	public GameObject shockFlashPrefabLoader;
	public GameObject holyExplosionPrefabLoader;
	public Shader transparentVectorShaderLoader;
	public Material brickMaterialLoader;
	public GameObject halfBrickPrefabLoader;
	public GameObject brickPrefabLoader;
	
	public static GameObject puckDustPrefab=null;
	public static Texture brickCracked=null;
	public static Texture brickCrackedNormal=null;
	public static GameObject puckPrefab;
	public static GameObject startZone;
	public static GameObject midZone;	
	public static Texture warriorTexture;
	public static Texture priestTexture;
	public static Texture wizardTexture;
	public static Texture rogueTexture;
	public static GameObject splashPrefab=null;
	public static GameObject shockFlashPrefab=null;
	public static GameObject holyExplosionPrefab=null;
	public static Shader transparentVectorShader=null;
	public static Material brickMaterial;
	public static GameObject halfBrickPrefab=null;
	public static GameObject brickPrefab=null;
	
	
	
	// Use this for initialization
	void Awake () {
		puckDustPrefab=puckDustPrefabLoader;
		brickCracked=brickCrackedLoader;
		brickCrackedNormal=brickCrackedNormalLoader;
		puckPrefab=puckPrefabLoader;
		startZone=startZoneLoader;
		midZone=midZoneLoader;
		warriorTexture=warriorTextureLoader;
		priestTexture=priestTextureLoader;
		wizardTexture=wizardTextureLoader;
		rogueTexture=rogueTextureLoader;
		splashPrefab=splashPrefabLoader;
		shockFlashPrefab=shockFlashPrefabLoader;
		holyExplosionPrefab=holyExplosionPrefabLoader;
		transparentVectorShader=transparentVectorShaderLoader;
		brickMaterial=brickMaterialLoader;
		halfBrickPrefab=halfBrickPrefabLoader;
		brickPrefab=brickPrefabLoader;
	}		
	
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
