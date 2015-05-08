using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SignPost : MonoBehaviour {
	private int maxCharCount = 150;
	public bool continueCurrentPassage = false;
	int wordsIndex = 0;
	GameObject player;
	Player amelia;
	public string[] words; //Lines of txt file
	bool beingRead = false;
	public TextAsset file;
	int sentenceIndex = 0;
	string textDisplay = "";
	public string sentence = ""; //current line being printed
	public string[] sentenceWords; //words in current Sentence
	float delay = 0.03f;
	float nextUse;
	public string startingPassage;
	public string nextPassage;
	public string currentPassage;
	public string speaker;
	public string connection;

	char[] charsToTrim = { '[' ,']'};


 	GameObject uiTextObj;

	GameObject buttonObj;
	Image uiButtonSprite;

	GameObject textBoxObj;
	Image uiTextBoxSprite;

	GameObject portraitObj;
	Image uiPortraitSprite;

	GameObject nameObj;

	Text uiText;
	Text nameText;


	int newLineIndex = 75;
	// Use this for initialization
	void Start () {
		nextPassage = startingPassage;
		nextUse = Time.time + delay;
		//file = (TextAsset)Resources.Load ("SignPosts_Notes/test");
		words = file.text.Split ('\n');
		player = GameObject.FindGameObjectWithTag ("Player");
		amelia = player.GetComponent<Player> ();

		uiTextObj = GameObject.Find ("Words");
		uiText = uiTextObj.GetComponent<Text>();
		uiText.text = words[wordsIndex];
		uiText.enabled = false;

		textBoxObj = GameObject.Find ("TextBox");
		uiTextBoxSprite = textBoxObj.GetComponent<Image>();
		uiTextBoxSprite.enabled = false;

		portraitObj = GameObject.Find ("Portrait");
		uiPortraitSprite = portraitObj.GetComponent<Image>();
		uiPortraitSprite.enabled = false;

		buttonObj = GameObject.Find ("Button");
		uiButtonSprite = buttonObj.GetComponent<Image>();
		uiButtonSprite.enabled = false;

		nameObj = GameObject.Find ("Name");
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
		if (currentPassage == nextPassage && !continueCurrentPassage) {
			beingRead = false;
			uiText.enabled = false;
			nextPassage = startingPassage;
			currentPassage = "";
			return;
		}
	//	Debug.Log ("reading");
		if (!beingRead) {
			wordsIndex = -1;
			beingRead = true;

			uiText.enabled = true;
		}
		NextSentence ();
		//CheckFlags ();
		
	}

	void CheckFlags(){
		//Debug.Log (words [0]);
		if (sentenceWords[0] == "::") {
			currentPassage = sentenceWords[1];
			speaker = sentenceWords[2];
			//Debug.Log ("Title is next");
		}		
	}

	void DisplayWords()
	{
		if (beingRead) {

			if (sentenceIndex < sentence.Length && !continueCurrentPassage) {

				textDisplay += sentence [sentenceIndex];
				newLineIndex--;
				//if(uiText.cachedTextGenerator.lineCount % 3 == 0){
				Debug.Log(textDisplay.Length);
				if(textDisplay.Length > maxCharCount){
					if(sentence [sentenceIndex] == '.' || sentence [sentenceIndex] == ' '){
						Debug.Log ("Stop Here");
						continueCurrentPassage = true;
					}
				}

				if(!continueCurrentPassage){
					sentenceIndex++;
				}
			}
			uiText.text = textDisplay;
			//Debug.Log (uiText.text.Length);
			//Debug.Log ("Line Count: " + uiText.cachedTextGenerator.lineCount);
			nameText.text = speaker;
		}

	}

	void NextSentence(){
		if (!continueCurrentPassage) {
			bool lookingForPassage = true;
			int currentIndex = 0;
			textDisplay = "";
			sentence = "";
			while (lookingForPassage) {
				if (words [currentIndex].StartsWith ("::")) {
					sentenceWords = words [currentIndex].Split (' ');

					if (sentenceWords [1] == nextPassage) {
						lookingForPassage = false;
						currentPassage = nextPassage;
						wordsIndex = currentIndex;
						if (sentenceWords.Length > 2) {
							speaker = sentenceWords [2];
							speaker = speaker.Trim (charsToTrim);
						} else {
							speaker = "";
						}

					}
				}
				currentIndex++;
				if (currentIndex >= words.Length) {
					lookingForPassage = false;
				}
			}
			wordsIndex += 1;
			sentenceIndex = 0;
			if (wordsIndex >= words.Length) {
				wordsIndex = 0;
				beingRead = false;

				uiText.enabled = false;
				sentenceIndex = 0;
			}

			sentenceWords = words [wordsIndex].Split (' ');
			for (int i = 0; i < sentenceWords.Length; i++) {
				if (!sentenceWords [i].StartsWith ("[[")) {
					sentence += sentenceWords [i];
					sentence += " ";
				}
			}
			if (sentenceWords [sentenceWords.Length - 1].Contains ("[[")) {
				connection = sentenceWords [sentenceWords.Length - 1];
				connection = connection.Trim (charsToTrim);

				nextPassage = connection;

			}
		} else {
			continueCurrentPassage = false;
			textDisplay = "";
		}

	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			amelia.SetReadSign(true);
			amelia.SetCurrentSign(this.gameObject); 

		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {

			textDisplay = "";
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") {

			amelia.SetReadSign(false);

			textDisplay = "";
			beingRead = false;
			wordsIndex = 0;

			uiText.enabled = false;
			nextPassage = startingPassage;
			currentPassage = "";
		}
	}

}
