using UnityEngine;
using System.Collections;
using System;

public class GUICode : MonoBehaviour {
	
	
	public GUIStyle titleStyle;
	public GUIStyle instructionStyle;
	public GUIStyle powerMeterTextStyle;
	public GUIStyle powerMeterInteriorStyle;
	public GUIStyle timerStyle;
	public GUIStyle angleLableStyle;
	
	//buttons
	int buttonSize=60;
	int buttonOffset=20;
	//settings window
	int settingsWindowWidth=20; //percent
	int settingsWindowHeight=50; //percent
	//target indicator
	int targetIndicatorSize=20;
	//power meter
	int powerMeterMax=100;
	int powerMeterLength=100;
	int powerMeterWidth=20;
	//engine selection
	int engineSelectNumber=0;
			
	public Texture2D settingsTexture;
	public Texture2D resetTexture;
	public Texture2D targetTexture;
	//test code
	//public static bool testPositive=false;
	
	void Awake() {
	}	
	
	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		switch (MainGameCode.gamestate) {
			case GAMESTATE.TITLE:
				if (Input.GetMouseButtonDown(0))  {
					MainGameCode.PlayGame();
				}			
			break;
			case GAMESTATE.GAMEOVER:
				if (Input.GetMouseButtonDown(0))  {
					MainGameCode.ResetGame();
				}			
			break;
			case GAMESTATE.PLAY:
				if (Input.GetKey(KeyCode.Return)) {
					MainGameCode.AimMode(); 
				}
			break;
			case GAMESTATE.AIM:
				if (Input.GetKey(KeyCode.Escape)) {
					MainGameCode.EndAim();
				}
				if (Input.GetKey(KeyCode.Space)) {
						MainGameCode.PowerCharge();
					}			
				if (Input.GetKeyUp(KeyCode.Space) && MainGameCode.currentPower>0) {
					MainGameCode.ShootPuck();
				}
			break;
			
		}	
		
	}		
	
	void OnGUI() {
		
		//if (testPositive) GUI.Box(new Rect(10,10,100,100),"test positive");
		
		switch (MainGameCode.gamestate) {
			case GAMESTATE.TITLE:	
				DrawTitle();
				DrawBestTime();
			break;
			case GAMESTATE.PLAY:
				PlayUIButtons();
				PlayInstructions();
				DrawTime();
			break;
			case GAMESTATE.SETTINGS:
				DrawSettingsWindow();
			break;
			case GAMESTATE.GAMEOVER:
				DrawFinalTime();
				DrawGameOver();
			break;
			case GAMESTATE.AIM:
				DrawTime();
				AimInstructions();
				DisplayTargetIndicator();
				DrawPowerMeter();
				DrawEngineSelect();
				DrawAngleSelect();
			break;			
		}	
	}	
	
	void DrawBestTime() {
		if (MainGameCode.bestTime>0) {
			System.TimeSpan t = System.TimeSpan.FromSeconds(MainGameCode.bestTime);
 
			string timerFormatted = "Best Time: "+string.Format("{0:D2}m:{1:D2}s",t.Minutes, t.Seconds);
		
			Rect timerPosition=new Rect(Screen.width*.8f, 10,1000,1000);
		
			ShadowAndOutline.DrawOutline(timerPosition,timerFormatted,timerStyle,Color.black,Color.white,2);	
		}	
	}	
	
	void DrawFinalTime() {
		if (MainGameCode.finalTime>0) {
			System.TimeSpan t = System.TimeSpan.FromSeconds(MainGameCode.finalTime);
 
			string timerFormatted = "Final Time: "+string.Format("{0:D2}m:{1:D2}s",t.Minutes, t.Seconds);
		
			Rect timerPosition=new Rect(Screen.width*.8f, 10,1000,1000);
		
			ShadowAndOutline.DrawOutline(timerPosition,timerFormatted,timerStyle,Color.black,Color.white,2);	
		}			
	}	
	
	void DrawTime() {
		
		System.TimeSpan t = System.TimeSpan.FromSeconds(MainGameCode.gameTime);
 
		string timerFormatted = "Time: "+string.Format("{0:D2}m:{1:D2}s",t.Minutes, t.Seconds);
		
		Rect timerPosition=new Rect(Screen.width*.8f, 10,1000,1000);
		ShadowAndOutline.DrawOutline(timerPosition,timerFormatted,timerStyle,Color.black,Color.white,2);
		
	}	
	
	void DrawAngleSelect() {
		
		Rect angleLablePosition=new Rect(15,118,50,100);
		ShadowAndOutline.DrawOutline(angleLablePosition,"Angle",instructionStyle,Color.black,Color.white,2);
		
		Rect angleSelectPosition=new Rect(10,140,100,100);	
		
		
		
		GUIContent[] selectionContent=new GUIContent[MainGameCode.engines[MainGameCode.selectedEngine].getAngles().Count];
		
		for (int counter=0;counter<MainGameCode.engines[MainGameCode.selectedEngine].getAngles().Count;counter++) {
			selectionContent[counter]=new GUIContent(MainGameCode.engines[MainGameCode.selectedEngine].getAngles()[counter].ToString());
		}	
		if (MainGameCode.selectedEngine==ENGINE.TREBUCHET) MainGameCode.angleSelectNumber=0;
		
		MainGameCode.angleSelectNumber=GUI.SelectionGrid(angleSelectPosition,MainGameCode.angleSelectNumber,selectionContent,1);
		
		
	}	
	
	
	void DrawEngineSelect() {
		GUIContent[] selectionContent=new GUIContent[3];
		selectionContent[0]=new GUIContent("ballista");
		selectionContent[1]=new GUIContent("catapult");
		selectionContent[2]=new GUIContent("trebuchet");
		
		
		Rect engineSelectPosition=new Rect(10,20,100,100);
		
		
		engineSelectNumber=GUI.SelectionGrid(engineSelectPosition,engineSelectNumber,selectionContent,1);
		switch (engineSelectNumber) {
			case 0:
				MainGameCode.SetEngine(ENGINE.BALLISTA);
			break;
			case 1:
				MainGameCode.SetEngine(ENGINE.CATAPULT);
			break;
			case 2:
				MainGameCode.SetEngine(ENGINE.TREBUCHET);
			break;
		}	
		
		
	}	
	
	void DrawPowerMeter() {
		Rect powerTextPosition=new Rect(10,Screen.height*.9f,100,20);
		ShadowAndOutline.DrawOutline(powerTextPosition,"Power",powerMeterTextStyle,Color.black,Color.white,2);
		
		Rect powerContainerPosition= new Rect(50,Screen.height*.9f-powerMeterLength-10,powerMeterWidth,powerMeterLength);
		GUI.Box(powerContainerPosition,"");
		
		float powerMeterInteriorLength=powerMeterLength*MainGameCode.currentPower/powerMeterMax;
		float powerMeterInteriorVeritcalPositionAdjustment=powerMeterLength-powerMeterLength*MainGameCode.currentPower/powerMeterMax;
		
		Rect powerDisplay=new Rect(50,Screen.height*.9f-powerMeterLength+powerMeterInteriorVeritcalPositionAdjustment-10,powerMeterWidth, powerMeterInteriorLength);
		GUI.Box(powerDisplay,"",powerMeterInteriorStyle);
		
			
	}	
	
	void DisplayTargetIndicator() {
		GUIStyle tempStyle=new GUIStyle();
		Vector3 kingPosition=MainGameCode.king.transform.position;
		Vector3 kingScreenPosition = MainGameCode.aimCamera.GetComponentInChildren<Camera>().WorldToViewportPoint(kingPosition);
		
		float kingScreenPositionHorizontal=kingScreenPosition.x;
		if (kingScreenPositionHorizontal>1) kingScreenPositionHorizontal=1;
		if (kingScreenPositionHorizontal<0) kingScreenPositionHorizontal=0;
		
		float kingScreenPostionHorizontalTranslation=Screen.width*kingScreenPositionHorizontal;
		
		Rect targetIndicatorLocation=new Rect(kingScreenPostionHorizontalTranslation-targetIndicatorSize*.5f,10,targetIndicatorSize,targetIndicatorSize);		
		
		//Debug.Log("guicode: king x viewport position " +  viewPos.x);
		
		
		
		GUI.Box(targetIndicatorLocation,targetTexture,tempStyle);
		
	}	
	
	void DrawGameOver() {
		ShadowAndOutline.DrawOutline(new Rect(0,Screen.height*.25f,Screen.width,Screen.height*.5f),"Game Over",titleStyle,Color.black,Color.white,2f);
		if ((float.Parse(Time.time.ToString("0.0"))) % 3<2.5f)
		ShadowAndOutline.DrawOutline(new Rect(0,Screen.height*.75f,Screen.width,Screen.height*.25f),"Click anywhere to go back to title",instructionStyle,Color.black,Color.white,2f);	
	}	
	
	void DrawTitle() {
		ShadowAndOutline.DrawOutline(new Rect(0,Screen.height*.25f,Screen.width,Screen.height*.5f),"Catapult!",titleStyle,Color.black,Color.white,2f);
		if ((float.Parse(Time.time.ToString("0.0"))) % 3<2.5f)
		ShadowAndOutline.DrawOutline(new Rect(0,Screen.height*.75f,Screen.width,Screen.height*.25f),"Click anywhere to play",instructionStyle,Color.black,Color.white,2f);
	}	
	
	void PlayUIButtons() {
		DrawSettingsButton();
		DrawResetPuckButton();
	}	
	
	void PlayInstructions() {
		Rect instructionPosition=new Rect(0,Screen.height*.9f,Screen.width,Screen.height*.10f);
		ShadowAndOutline.DrawOutline(instructionPosition,"enter to aim puck\n wasd moves view, q and e rotate",instructionStyle,Color.black,Color.white,2f);
	}	
	
	void AimInstructions() {
		string aimInstructionString="";
		aimInstructionString+="ballista fires flat shots at low angles, ";
		aimInstructionString+="catapult fires spinning shots at high angles,\n";
		aimInstructionString+="trebuchet first high power spinning shots at a fixed angle\n and starts in an elevated position\n";
		aimInstructionString+="arrow keys to turn puck and tilt view, esc to cancel\nhold space to power up, release to fire";
		Rect instructionPosition=new Rect(0,Screen.height*.75f,Screen.width,Screen.height*.10f);
		ShadowAndOutline.DrawOutline(instructionPosition,aimInstructionString,instructionStyle,Color.black,Color.white,2f);
	}	
	
	void DrawSettingsButton() {	
		if (GUI.Button(new Rect(buttonOffset,Screen.height-buttonOffset-buttonSize,buttonSize,buttonSize),
			settingsTexture)) 
			MainGameCode.gamestate=GAMESTATE.SETTINGS;
		
	}	
	
	void DrawResetPuckButton() {	
		if (GUI.Button(new Rect(Screen.width-buttonSize-buttonOffset,Screen.height-buttonOffset-buttonSize,buttonSize,buttonSize),
			resetTexture)) 
			MainGameCode.ResetPuck();
		
	}	
	
	void DrawSettingsWindow() {

		GUILayout.BeginArea(new Rect(Screen.width*.5f-Screen.width*.5f*.01f*settingsWindowWidth,
						 Screen.height*.5f-Screen.height*.5f*.01f*settingsWindowHeight,
						 Screen.width*.01f*settingsWindowWidth,
						 Screen.height*.01f*settingsWindowHeight));
		if (GUILayout.Button("Back To Game")) 
			MainGameCode.gamestate=GAMESTATE.PLAY;
		if (GUILayout.Button("Quit Game")) 
			MainGameCode.QuitGame();
		
		
		GUILayout.EndArea();
	}	
	
}
