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

public class ABGameController : MonoBehaviour {
	
	public GameObject pauseCanvas;
	GameObject activePC;
	public GameObject inputHandler;
	GameObject activeIH;
	public GameObject seedSelector;
	GameObject activeSS;
	public GameObject ui;
	GameObject activeUI;
	public GameObject Fader;
	GameObject activeFD;

	GameObject amelia;

	bool fading = false;
	bool transitioning;
	int transitionType = 0; //1 for number, 2 for name
	int transitionNum;
	string transitionName;
	
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
		
		//textbox UI
		activeUI = Instantiate (ui, ui.transform.position, Quaternion.identity) as GameObject;
		activeUI.name = "UI";
		activeUI.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		
		//blackscreen for fading
		activeFD = Instantiate (Fader, Fader.transform.position, Quaternion.identity) as GameObject;
		activeFD.name = "Fader";
		activeFD.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeFD.GetComponent<Image> ().color = new Color (activeFD.GetComponent<Image> ().color.r, activeFD.GetComponent<Image> ().color.g,
		                                                 activeFD.GetComponent<Image> ().color.b, 0);
		
		//set as children so they aren't destroyed
		activeIH.transform.SetParent (transform);
		activePC.transform.SetParent (transform);
		activeSS.transform.SetParent (transform);
		activeUI.transform.SetParent (transform);
		activeFD.transform.SetParent (transform);

		//load amelia for some direct influence
		amelia = GameObject.FindGameObjectWithTag("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.F) && !fading) {
			fading = true;
			StartCoroutine (FadeOut ());
		}
		if (Input.GetKeyDown (KeyCode.I) && !fading) {
			fading = true;
			StartCoroutine (FadeIn());
		}
	}
	
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
		amelia = GameObject.FindGameObjectWithTag("Player");
	}
	
	public IEnumerator FadeOut()
	{
		float startTime = Time.time;
		while (activeFD.GetComponent<Image>().color.a < 1) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/1.5f;
			activeFD.GetComponent<Image> ().color = new Color (activeFD.GetComponent<Image> ().color.r, activeFD.GetComponent<Image> ().color.g,
			                                                   activeFD.GetComponent<Image> ().color.b, Mathf.SmoothStep(0,1, t));
		}
		fading = false;
		if (transitioning) {
			if(transitionType == 1)
			{
				Application.LoadLevel(transitionNum);
			}
			else if (transitionType == 2)
			{
				Application.LoadLevel(transitionName);
			}
			transitioning = false;
			fading = true;
			transitionType = 0;
			StartCoroutine (FadeIn ());
		}
	}

	public IEnumerator FadeIn()
	{
		float startTime = Time.time;
		while (activeFD.GetComponent<Image>().color.a > 0) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/1.5f;
			activeFD.GetComponent<Image> ().color = new Color (activeFD.GetComponent<Image> ().color.r, activeFD.GetComponent<Image> ().color.g,
			                                                   activeFD.GetComponent<Image> ().color.b, 1 - Mathf.SmoothStep(0,1, t));
		}
		fading = false;
	}

	public void BeginSceneTransition(int sceneNum)
	{
		amelia.GetComponent<PlayerController> ().canControl = false;
		if (!transitioning) {
			fading = true;
			transitioning = true;
			transitionNum = sceneNum;
			transitionType = 1;
			StartCoroutine (FadeOut ());
		}
	}

	public void BeginSceneTransition(string sceneName)
	{
		amelia.GetComponent<PlayerController> ().canControl = false;
		if (!transitioning) {
			fading = true;
			transitioning = true;
			transitionName = sceneName;
			transitionType = 2;
			StartCoroutine (FadeOut ());
		}
	}
}
