using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

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
	public GameObject TransistionText;
	GameObject activeTT;

	GameObject amelia;

	bool fading = false;
	bool transitioning;
	int transitionType = 0; //1 for number, 2 for name
	int transitionNum;
	string transitionName;

	bool[] unlockedSeeds; //use once we get seed unlocking in properly. We may not even need this.
	Player.SeedType activeSeed;

	
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


		//Text UI for Act transitions
		activeTT = Instantiate (TransistionText, TransistionText.transform.position, Quaternion.identity) as GameObject;
		activeTT.name = "Transition Text";
		activeTT.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		
		//set as children so they aren't destroyed
		activeIH.transform.SetParent (transform);
		activeTT.transform.SetParent (transform);
		activeFD.transform.SetParent (transform);
		activePC.transform.SetParent (transform);
		activeSS.transform.SetParent (transform);
		activeUI.transform.SetParent (transform);

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
		//make sure all the stuff updates with the new scene's camera/player
		activePC.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeUI.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeFD.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeTT.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
		activeSS.BroadcastMessage("UpdatePlayer");
		amelia = GameObject.FindGameObjectWithTag("Player");
		//apply saved information from previous scene
		amelia.GetComponent<Player> ().SetCurrentSeed (activeSeed);

	}
	
	public IEnumerator FadeOut()
	{
		float startTime = Time.time;
		while (activeFD.GetComponent<Image>().color.a < 1) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/0.5f;//(timepassed/duration)
			activeFD.GetComponent<Image> ().color = new Color (activeFD.GetComponent<Image> ().color.r, activeFD.GetComponent<Image> ().color.g,
			                                                   activeFD.GetComponent<Image> ().color.b, Mathf.SmoothStep(0,1, t));
		}
		fading = false;
		if (transitioning) {
			float delay = 0;
			if(transitionType == 1)
			{
				switch(transitionNum)
				{
				case 1:
					activeTT.GetComponent<Act_Text>().FadeInText("Act II", "It takes a lot to make a tree");
					delay = 2f;
					break;
				default:
					delay = 0;
					break;
				}
				Application.LoadLevel(transitionNum);
			}
			else if (transitionType == 2)
			{
				Application.LoadLevel(transitionName);
				//new act handling
				switch(transitionName)
				{
				case "ActII-1_Encounter":
					activeTT.GetComponent<Act_Text>().FadeInText("Act II", "It takes a lot to make a tree");
					delay = 2f;
					break;
				default:
					break;
				}
			}
			transitioning = false;
			fading = true;
			transitionType = 0;

			StartCoroutine (FadeIn (delay));
		}
	}

	public IEnumerator FadeIn(float delay = 0)
	{
		yield return new WaitForSeconds (delay);
		float startTime = Time.time;
		while (activeFD.GetComponent<Image>().color.a > 0) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/1f; //(timepassed/duration)
			activeFD.GetComponent<Image> ().color = new Color (activeFD.GetComponent<Image> ().color.r, activeFD.GetComponent<Image> ().color.g,
			                                                   activeFD.GetComponent<Image> ().color.b, 1 - Mathf.SmoothStep(0,1, t));
		}
		fading = false;
	}

	public void BeginSceneTransition(int sceneNum)
	{
		amelia.GetComponent<PlayerController> ().canControl = false;
		//here's where we get all relevent info on the player and save it for next scene
		activeSeed = amelia.GetComponent<Player> ().getCurrentSeedType ();
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
		//here's where we get all relevent info on the player and save it for next scene
		activeSeed = amelia.GetComponent<Player> ().getCurrentSeedType ();
		if (!transitioning) {
			fading = true;
			transitioning = true;
			transitionName = sceneName;
			transitionType = 2;
			StartCoroutine (FadeOut ());
		}
	}
}
