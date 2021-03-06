﻿using UnityEngine;
using System.Collections;

public class Color_Schemer : MonoBehaviour {

	public int act = 1;

	//Colors go in order from front layer to back.
	//TODO: Read colors from Json file
	public Color[] ActIColors = new Color[7];
	public Color[] ActIIColors = new Color[7];
	public Color[] ActIIIColors = new Color[7];
	public Color[] ActIVColors = new Color[7];
	public Color[] ActVColors = new Color[7];

	Color[] activeColors;

	// Use this for initialization
	void Awake () {
		string actName = extractAct (Application.loadedLevelName);
		switch(actName)
		{
		case "ActI":
			act = 1;
			break;
		case "ActII":
			act = 2;
			break;
		case "ActIII":
			act = 3;
			break;
		case "ActIV":
			act = 4;
			break;
		case "ActV":
			act = 5;
			break;
		case "Cutscene1":
			act = 1;
			break;
		case "Cutscene2":
			act = 1;
			break;
		case "Cutscene3":
			act = 2;
			break;
		case "Cutscene4":
			act = 2;
			break;
		case "Cutscene5":
			act = 3;
			break;
		case "Cutscene6":
			act = 4;
			break;
		case "Cutscene7":
			act = 5;
			break;
		case "Cutscene8":
			act = 5;
			break;
		case "Cutscene9":
			act = 5;
			break;
		case "Cutscene10":
			act = 5;
			break;
		default:
			act = 1;
			break;
		}
		//assign act colors here to prevent nested switch statesments because screw that
		switch (act) {
		case 1:
			activeColors = ActIColors;
			break;
		case 2:
			activeColors = ActIIColors;
			break;
		case 3:
			activeColors = ActIIIColors;
			break;
		case 4:
			activeColors = ActIVColors;
			break;
		case 5:
			activeColors = ActVColors;
			break;
		}
		//need to take care of recoloring background since the Camera does not make this call
		GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ().backgroundColor = activeColors [4];
	}
	
	public Color GetColor(string objectName)
	{
		switch (objectName) {
		case "Background 1":
			return activeColors[0];
		case "Background 2":
			return activeColors[1];
		case "Background 3":
			return activeColors[2];
		case "Backmost":
			return activeColors[3];
		case "grass":
			return activeColors[5];
		case "grass fluff":
			return activeColors[6];
		default:
			return activeColors[0];
		}
	}

	public string extractAct(string levelName){
		
		string actName;
		try{
			actName = levelName.Substring(0, levelName.IndexOf('-'));
		}
		catch{
			actName = levelName;
		}
		
		return actName;
	}
	
}
