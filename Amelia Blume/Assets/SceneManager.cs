using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
	public string LevelToLoad;
	public List<GameObject> events;
	ABGameController GameController;
	GameObject amelia;
	GameObject ig;
	GameObject currChar;
	Player player;
	int index;
	SignPost signPost;
	float nextUse;
	float delay = 0.3f;
	bool moving = false;
	public Animator ameliaAnimator;
	public Animator igAnimator;
	//float timer;
	// Use this for initialization
	void Start () {
		GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ABGameController> ();
		//events = new List<GameObject>();
		ig = GameObject.Find ("Ignatius");
		amelia = GameObject.FindGameObjectWithTag ("Player");
		player = amelia.GetComponent<Player> ();
		player.CUTSCENE = true;
		player.SetCurrentSign (this.gameObject);
		signPost = GetComponent<SignPost>();
		if (ig != null){igAnimator = ig.GetComponentInChildren<Animator> ();}
		ameliaAnimator = amelia.GetComponentInChildren<Animator> ();
		index = 0;

		NextEvent ();

		nextUse = Time.time + delay;
		//signPost.inCutscene = true;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//timer++;
		if (moving) {
			MoveCharacter();
		}
	}

	void StartReading(){
		signPost.CutsceneStart (events [index].name);
	}

	void MoveCharacter(){
		Vector3 targetPos;
		if(currChar == ig)
			targetPos = new Vector3(events [index].transform.position.x, currChar.transform.position.y, currChar.transform.position.z);
		else
			targetPos = events [index].transform.position;
		if (Mathf.Abs(targetPos.x - currChar.transform.position.x) > 2) {
			//if(Time.time > nextUse){
//				Debug.Log ("Moving");
				currChar.transform.position = Vector3.MoveTowards (currChar.transform.position, targetPos, 0.1f);
				//MoveCharacter (currChar);
				//nextUse = Time.time+delay;
			//}
		} else {
			index++;
			moving = false;
			NextEvent();
		}
	}

	void NextEvent(){
		//Debug.Log (events [index]);
		if (index < events.Count) {
			switch (events [index].tag) {
			case "Amelia":
				//Debug.Log ("Amelia Move");
				currChar = amelia;
				moving = true;
				player.SetReadSign (false);
				ameliaAnimator.SetBool ("isRunning", true);
				igAnimator.SetBool ("isRunning", false);
				//MoveCharacter();
				//index++;
				//NextEvent ();
				break;
		
			case "Ig":
				//Debug.Log ("Ig Move");
				currChar = ig;
				moving = true;
				player.SetReadSign (false);
				ameliaAnimator.SetBool ("isRunning", false);
				igAnimator.SetBool ("isRunning", true);
				//MoveCharacter();
				//index++;
				//NextEvent ();
				break;
		
			case "Text":
				ameliaAnimator.SetBool ("isRunning", false);
				igAnimator.SetBool ("isRunning", false);
				//Debug.Log ("Text");
				StartReading ();
				index++;
				//NextEvent ();
				break;
		
		
			}
		} else {
			//Fade to Black?
			//Load Next Scene
			ameliaAnimator.SetBool ("isRunning", false);
			igAnimator.SetBool ("isRunning", false);
			GameController.BeginSceneTransition(LevelToLoad);
		}
	}

	void PrintThis(){
		Debug.Log ("Got Print");
	}
}
