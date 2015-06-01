using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
	public string LevelToLoad;
	public List<GameObject> events;
	ABGameController GameController;
	SideScrollerCameraController CameraController;
	GameObject amelia;
	GameObject ig;
	GameObject currChar;
	Player player;
	int index;
	SignPost signPost;
	float nextUse;
	float delay = 0.3f;
	bool moving = false;
	bool waitingOnCamera = false;
	public Animator ameliaAnimator;
	public Animator igAnimator;
	//float timer;
	// Use this for initialization
	void Start () {
		GameController = GameObject.FindGameObjectWithTag("GameController").GetComponent<ABGameController> ();
		CameraController = Camera.main.GetComponent<SideScrollerCameraController>();
		//events = new List<GameObject>();
		ig = GameObject.Find ("Ignatius");
		amelia = GameObject.FindGameObjectWithTag ("Player");
		player = amelia.GetComponent<Player> ();
		player.CUTSCENE = true;
		player.SetCurrentSign (this.gameObject);
		signPost = GetComponent<SignPost>();
		if (ig != null){igAnimator = ig.GetComponentInChildren<Animator> ();}
		ameliaAnimator = amelia.GetComponentInChildren<Animator> ();
		index = -1;

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
		if (waitingOnCamera)
		{
			CheckCamera();
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
			moving = false;
			NextEvent();
		}
	}

	void CheckCamera()
	{
		if(waitingOnCamera)
		{
			if (Camera.main.transform.position == CameraController.panTo)
			{
				Debug.Log("Camera has reached its location, start next event.");
				waitingOnCamera = false;
				NextEvent();
			}
			else
			{
				Debug.Log("Camera still in motion, don't start next event yet.");
			}
		}
		else
		{
			Debug.Log("Why check on the camera if it's not being waitied on?");
		}
	}

	void NextEvent(){
		index++;
		Debug.Log (index);
		if (index < events.Count) {
			switch (events [index].tag) {
			case "Amelia":
				//Debug.Log ("Amelia Move");
				currChar = amelia;
				moving = true;
				player.SetReadSign (false);
				if (ameliaAnimator != null){ameliaAnimator.SetBool ("isRunning", true);}
				if (igAnimator != null){igAnimator.SetBool ("isRunning", false);}
				//MoveCharacter();
				//NextEvent ();
				break;
		
			case "Ig":
				//Debug.Log ("Ig Move");
				currChar = ig;
				moving = true;
				player.SetReadSign (false);
				if (ameliaAnimator != null){ameliaAnimator.SetBool ("isRunning", false);}
				if (igAnimator != null){igAnimator.SetBool ("isRunning", true);}
				//MoveCharacter();
				//NextEvent ();
				break;
		
			case "Text":
				if (ameliaAnimator != null){ameliaAnimator.SetBool ("isRunning", false);}
				if (igAnimator != null){igAnimator.SetBool ("isRunning", false);}
				//Debug.Log ("Text");
				StartReading ();
				//NextEvent ();
				break;

			case "Camera":
				Debug.Log("Camera Event");
				CameraEvent eventScript = events[index].GetComponent<CameraEvent>();
				CameraController.MoveToPosition(events[index].transform.position, eventScript.speed, eventScript.cameraSize);
				waitingOnCamera = eventScript.waitForDestination;
				if (!waitingOnCamera)
				{
					Debug.Log("Starting next event while Camera is still in motion");
					NextEvent();
				}
				break;

		
		
			}
		} else {
			//Fade to Black?
			//Load Next Scene
			if (ameliaAnimator != null){ameliaAnimator.SetBool ("isRunning", false);}
			if (igAnimator != null){igAnimator.SetBool ("isRunning", false);}
			GameController.BeginSceneTransition(LevelToLoad);
		};
	}

	void PrintThis(){
		Debug.Log ("Got Print");
	}
}
