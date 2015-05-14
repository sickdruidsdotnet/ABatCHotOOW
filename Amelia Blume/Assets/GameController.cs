using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

//TODO:
 /* Carry the player's state between scenes
 * 		-Saving the entire player gameobject, then overwrite the player in the next scene with the one in the previous\
 * 		- or save aspects of character (unlocked seeds, current then 
 * 	Transitioning between scenes
 * 		- Delete any GameController in the next scene and replace it with the older one on transitions
 * 		- fade in/out
 * */

public class GameController : MonoBehaviour {
	
	public GameObject pauseCanvas;
	GameObject activePC;
	public GameObject inputHandler;
	GameObject activeIH;
	public GameObject seedSelector;
	GameObject activeSS;
	public GameObject ui;
	GameObject activeUI;


	// Use this for initialization
	void Start () {
		//first check to make sure another gamecontroller does not already exist
		GameObject[] gcs = GameObject.FindGameObjectsWithTag ("GameController");
		if (gcs.Count () > 1) {
			Destroy(gameObject);
			return;
		}

		gameObject.name = "Game Controller " + Application.loadedLevelName;
		//let's ensure this stays alive and constant
		DontDestroyOnLoad(gameObject);

		//start with pause canvas
		activePC = Instantiate (pauseCanvas, pauseCanvas.transform.position, Quaternion.identity) as GameObject;
		activePC.name = "Pause Canvas";
		activePC.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		//input handler
		activeIH  = Instantiate (inputHandler,  pauseCanvas.transform.position, Quaternion.identity) as GameObject;
		activeIH.name = "Input Handler";
		activeIH.GetComponent<InputHandler>().pauseCanvas = activePC;

		//seed selector
		activeSS = Instantiate (seedSelector, seedSelector.transform.position, Quaternion.identity) as GameObject;
		activeSS.name = "Seed Selector";

		activeUI = Instantiate (ui, ui.transform.position, Quaternion.identity) as GameObject;
		activeUI.name = "UI";
		activeUI.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		//set as children so they aren't destroyed
		activeIH.transform.SetParent (transform);
		activePC.transform.SetParent (transform);
		activeSS.transform.SetParent (transform);
		activeUI.transform.SetParent (transform);
	}
	
	// Update is called once per frame
	/*void Update () {
	
	}*/

	void OnLevelWasLoaded(int level)
	{
		Debug.Log ("Level " + level + " loaded.");
		//set properties that need to be reset
		if (activePC == null) {
			return;
		}
		activePC.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeUI.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeSS.BroadcastMessage("UpdatePlayer");
	}
	
}
