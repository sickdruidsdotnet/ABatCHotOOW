using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SceneManager : MonoBehaviour {
	public List<GameObject> events;
	GameObject amelia;
	Player player;
	int index;
	SignPost signPost;
	// Use this for initialization
	void Start () {
		//events = new List<GameObject>();
		amelia = GameObject.Find ("Player");
		player = amelia.GetComponent<Player> ();
		player.SetCurrentSign (this.gameObject);
		signPost = GetComponent<SignPost>();
		index = 0;

		NextEvent ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
	}

	void StartReading(){
		player.SetReadSign (true);
		signPost.startingPassage = events [index].name;
		//signPost.currentPassage = signPost.startingPassage;
		signPost.nextPassage = signPost.startingPassage;
		player.SetCurrentSign (this.gameObject);
		signPost.Read ();
		//signPost.Read ();
		//signPost.Read ();
		//signPost.Read ();
	}

	void NextEvent(){
		//Debug.Log (events [index]);
		if (index < events.Count) {
			switch (events [index].tag) {
			case "Amelia":
				Debug.Log ("Amelia Move");
				index++;
				//NextEvent ();
				break;
		
			case "Ig":
				Debug.Log ("Ig Move");
				index++;
				//NextEvent ();
				break;
		
			case "Text":
				Debug.Log ("Text");
				StartReading();
				index++;
				//NextEvent ();
				break;
		
		
			}
		}
	}

	void PrintThis(){
		Debug.Log ("Got Print");
	}
}
