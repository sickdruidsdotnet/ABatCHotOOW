using UnityEngine;
using System.Collections;

public class SignPost : MonoBehaviour {
	public int wordsIndex = 0;
	//public int INDEX = 100;
	TextMesh myTextMesh;
	public GameObject player;
	public Player amelia;
	string[] words;
	bool beingRead = false;
	// Use this for initialization
	void Start () {
		words = new string[]{"Words are here","This too", "Part 3"};
		Debug.Log (words.Length);
		myTextMesh = GetComponentInChildren<TextMesh> ();
		myTextMesh.text = words[wordsIndex];
		myTextMesh.renderer.enabled = false;
		player = GameObject.FindGameObjectWithTag ("Player");
		amelia = player.GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		myTextMesh.text = words[wordsIndex];
	}

	public void Read(){
		Debug.Log ("reading");
		if (!beingRead) {
			//wordsIndex = -1;
		}
		NextSentence ();
		//wordsIndex += 1;
		//INDEX++;
		myTextMesh.renderer.enabled = true;
		myTextMesh.text = words[wordsIndex];
	}

	void NextSentence(){
		wordsIndex+=1;
		if (wordsIndex >= words.Length) {
			wordsIndex = 0;
			beingRead = false;
			myTextMesh.renderer.enabled = false;
		}

	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			amelia.SetReadSign(true);
			amelia.SetCurrentSign(this.gameObject); 
			Debug.Log ("read now");
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			amelia.SetReadSign(false);
			Debug.Log ("stop reading");
			beingRead = false;
			//wordsIndex = 0;
		}
	}

}
