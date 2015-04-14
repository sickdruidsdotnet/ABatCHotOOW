using UnityEngine;
using System.Collections;

public class SignPost : MonoBehaviour {
	int wordsIndex = 0;
	//public int INDEX = 100;
	TextMesh myTextMesh;
	GameObject player;
	Player amelia;
	string[] words;
	bool beingRead = false;
	TextAsset file;
	string[] lines;
	int sentenceIndex = 0;
	string textDisplay = "";
	string sentence = "";
	float delay = 0.05f;
	float nextUse;
	// Use this for initialization
	void Start () {
		nextUse = Time.time + delay;
		file = (TextAsset)Resources.Load ("SignPosts_Notes/test");
		words = file.text.Split ('\n');
		myTextMesh = GetComponentInChildren<TextMesh> ();
		myTextMesh.text = words[wordsIndex];
		myTextMesh.renderer.enabled = false;
		player = GameObject.FindGameObjectWithTag ("Player");
		amelia = player.GetComponent<Player> ();

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time > nextUse) {
			DisplayWords ();
			nextUse = Time.time + delay;
		}
	}

	public void Read(){
		Debug.Log ("reading");
		if (!beingRead) {
			wordsIndex = -1;
			beingRead = true;
			myTextMesh.renderer.enabled = true;
		}
		NextSentence ();

	}

	void DisplayWords()
	{
		if (beingRead) {
			if (sentenceIndex < sentence.Length) {
				textDisplay += sentence [sentenceIndex];
				sentenceIndex++;
			}
		}
		myTextMesh.text = textDisplay;
	}

	void NextSentence(){
		textDisplay = "";
		wordsIndex+=1;
		sentenceIndex = 0;
		if (wordsIndex >= words.Length) {
			wordsIndex = 0;
			beingRead = false;
			myTextMesh.renderer.enabled = false;
			sentenceIndex = 0;
		}
		sentence = words [wordsIndex];
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			amelia.SetReadSign(true);
			amelia.SetCurrentSign(this.gameObject); 
			//Debug.Log ("read now");
			//beingRead = true;
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			amelia.SetReadSign(false);
			//Debug.Log ("stop reading");
			beingRead = false;
			textDisplay = "";
			wordsIndex = 0;
			myTextMesh.renderer.enabled = false;
		}
	}

}
