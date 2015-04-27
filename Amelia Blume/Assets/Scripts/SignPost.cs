using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SignPost : MonoBehaviour {
	int wordsIndex = 0;
	GameObject player;
	Player amelia;
	public string[] words; //Lines of txt file
	bool beingRead = false;
	public TextAsset file;
	//string[] lines;
	int sentenceIndex = 0;
	string textDisplay = "";
	public string sentence = ""; //current line being printed
	public string[] sentenceWords; //words in current Sentence
	float delay = 0.03f;
	float nextUse;


	public GameObject uiTextObj;

	public GameObject buttonObj;
	Image uiButtonSprite;

	public GameObject textBoxObj;
	Image uiTextBoxSprite;

	public GameObject portraitObj;
	Image uiPortraitSprite;

	public GameObject nameObj;

	Text uiText;
	Text nameText;


	public int newLineIndex = 75;
	// Use this for initialization
	void Start () {
		nextUse = Time.time + delay;
		//file = (TextAsset)Resources.Load ("SignPosts_Notes/test");
		words = file.text.Split ('\n');
		player = GameObject.FindGameObjectWithTag ("Player");
		amelia = player.GetComponent<Player> ();

		uiText = uiTextObj.GetComponent<Text>();
		uiText.text = words[wordsIndex];
		uiText.enabled = false;

		uiTextBoxSprite = textBoxObj.GetComponent<Image>();
		uiTextBoxSprite.enabled = false;

		uiPortraitSprite = portraitObj.GetComponent<Image>();
		uiPortraitSprite.enabled = false;

		uiButtonSprite = buttonObj.GetComponent<Image>();
		uiButtonSprite.enabled = false;

		nameText = nameObj.GetComponent<Text> ();
		nameText.enabled = false;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Time.time > nextUse) {
			DisplayWords ();
			nextUse = Time.time + delay;
		}

		uiButtonSprite.enabled = uiText.enabled;
		uiTextBoxSprite.enabled = uiText.enabled;
		nameText.enabled = uiText.enabled;
		uiPortraitSprite.enabled = uiText.enabled;
	}

	public void Read(){
	//	Debug.Log ("reading");
		if (!beingRead) {
			wordsIndex = -1;
			beingRead = true;
			//myTextMesh.renderer.enabled = true;
			uiText.enabled = true;
		}
		NextSentence ();

	}

	void DisplayWords()
	{
		if (beingRead) {
			if (sentenceIndex < sentence.Length) {
				textDisplay += sentence [sentenceIndex];
				newLineIndex--;
				if(newLineIndex <= 0){
					if(sentence [sentenceIndex] == ' '){
						//Debug.Log ("Space Key in if");
						//sentence.Insert(sentenceIndex," sdf ");
						//textDisplay += "\n";
						newLineIndex = 30;
					}
					//Debug.Log(sentence [sentenceIndex]);
					//New Line IF current char is "space"
				}
				sentenceIndex++;
			}
		}
		//myTextMesh.text = textDisplay;
		uiText.text = textDisplay;
	}

	void NextSentence(){
		textDisplay = "";
		wordsIndex+=1;
		sentenceIndex = 0;
		if (wordsIndex >= words.Length) {
			wordsIndex = 0;
			beingRead = false;
			//myTextMesh.renderer.enabled = false;
			uiText.enabled = false;
			sentenceIndex = 0;
		}
		//if(wordsIndex > 0)
		//	wordsIndex--;
		sentence = words [wordsIndex];
		sentenceWords = sentence.Split (' ');
		//wordsIndex++;
		//while (sentence[sentence.Length-1] != '.') {
		//	sentence += words [wordsIndex];
		//	wordsIndex++;
		//}
		//Debug.Log (sentence [sentence.Length - 1]);
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
			//myTextMesh.renderer.enabled = false;
			uiText.enabled = false;
		}
	}

}
