using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class SignPost : MonoBehaviour {
	public bool stillWritingCurrentPassage = false;
	private int maxCharCount = 150;
	public bool continueCurrentPassage = false;
	int wordsIndex = 0;
	GameObject player;
	Player amelia;
	string[] words; //Lines of txt file
	bool beingRead = false;
	public TextAsset file;
	int sentenceIndex = 0;
	string textDisplay = "";
	string sentence = ""; //current line being printed
	string[] sentenceWords; //words in current Sentence
	float delay = 0.03f;
	float nextUse;
	public string startingPassage;
	string nextPassage;
	string currentPassage;
	string speaker;
	string connection;

	bool personSpeaking = false;

	public Sprite[] portraits = new Sprite[2];

	char[] charsToTrim = { '[' ,']'};
	char[] titleSplit = { '[', ']' , ':'};


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

	RectTransform portraitRect;
	RectTransform nameRect;

	//get the canvas we'll be working with
	Canvas uiCanvas;


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
		portraitRect = portraitObj.GetComponent<RectTransform> ();

		buttonObj = GameObject.Find ("Button");
		uiButtonSprite = buttonObj.GetComponent<Image>();
		uiButtonSprite.enabled = false;

		nameObj = GameObject.Find ("Name");
		nameText = nameObj.GetComponent<Text> ();
		nameText.enabled = false;
		nameRect = nameObj.GetComponent<RectTransform> ();

		//load/put the canvas behind the camera for easier editor experience
		uiCanvas = GameObject.Find ("UI").GetComponent<Canvas> ();
		if (uiCanvas != null) {
			uiCanvas.planeDistance = -1;
		}
	}

	//sorry to jam this in here but it's running into issues with the gamecontroller;
	void ReloadResources()
	{
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
		portraitRect = portraitObj.GetComponent<RectTransform> ();
		
		buttonObj = GameObject.Find ("Button");
		uiButtonSprite = buttonObj.GetComponent<Image>();
		uiButtonSprite.enabled = false;
		
		nameObj = GameObject.Find ("Name");
		nameText = nameObj.GetComponent<Text> ();
		nameText.enabled = false;
		nameRect = nameObj.GetComponent<RectTransform> ();
		
		//load/put the canvas behind the camera for easier editor experience
		uiCanvas = GameObject.Find ("UI").GetComponent<Canvas> ();
		if (uiCanvas != null) {
			uiCanvas.planeDistance = -1;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (uiCanvas == null || uiButtonSprite == null) {
			ReloadResources();
			return;
		}
		if (Time.time > nextUse) {
			DisplayWords ();
			nextUse = Time.time + delay;
		}
		//Debug.Log(amelia.GetReadSign());
		uiButtonSprite.enabled = uiText.enabled;
		uiTextBoxSprite.enabled = uiText.enabled;
		nameText.enabled = uiText.enabled;
<<<<<<< HEAD
		uiPortraitSprite.enabled = uiText.enabled;
		//Debug.Log(uiText.enabled);
=======
		uiPortraitSprite.enabled = (personSpeaking && uiText.enabled);
		if (uiButtonSprite.enabled) {
			if (uiCanvas != null) {
				uiCanvas.planeDistance = 2;
			}
		}

		if (speaker == "Amelia") {
//			Debug.Log (portraitObj.GetComponent<RectTransform>().anchoredPosition);
			personSpeaking = true;
			uiPortraitSprite.sprite = portraits[0];
			nameRect.anchoredPosition = new Vector2(-145.8f, -53.7f);
			portraitRect.anchoredPosition = new Vector2(-265.2f, -66.5f);
		} else if (speaker == "Ig") {
			uiPortraitSprite.sprite = portraits[0];
			personSpeaking = true;
			portraitRect.anchoredPosition = new Vector2(352f, -66.5f);
			nameRect.anchoredPosition = new Vector2(307f, -53.7f);
		} else {
			nameText.enabled = false;
			personSpeaking = false;
		}

>>>>>>> master
	}

	public void Read(){
		if (currentPassage == nextPassage && !continueCurrentPassage) {
			if(beingRead && stillWritingCurrentPassage){
				DisplayFullText();
			}else{
			beingRead = false;
			uiText.enabled = false;
			nextPassage = startingPassage;
			currentPassage = "";
			}
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

	public void Read(string passage){
		if (passage == nextPassage && !continueCurrentPassage) {
			if(beingRead && stillWritingCurrentPassage){
				DisplayFullText();
			}else{
				beingRead = false;
				uiText.enabled = false;
				nextPassage = passage;
				currentPassage = "";
			}
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

	void DisplayFullText(){
		if (beingRead) {
			bool keepWriting = true;
				while(keepWriting){
					if(sentenceIndex > sentence.Length-1){
						keepWriting = false;
						return;
					}
					textDisplay += sentence [sentenceIndex];
					if(textDisplay.Length > maxCharCount){
						if(sentence [sentenceIndex] == '.' || sentence [sentenceIndex] == ' '){
							Debug.Log ("Stop Here");
							continueCurrentPassage = true;
							keepWriting = false;
						}
					}

					if(sentenceIndex <= sentence.Length - 1){
						sentenceIndex++;
					}else{
						keepWriting = false;
					}

				}
			uiText.text = textDisplay;
			nameText.text = speaker;
		}
		
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
				stillWritingCurrentPassage = true;
				textDisplay += sentence [sentenceIndex];
				newLineIndex--;
				//if(uiText.cachedTextGenerator.lineCount % 3 == 0){
				//Debug.Log(textDisplay.Length);
				if(textDisplay.Length > maxCharCount){
					if(sentence [sentenceIndex] == '.' || sentence [sentenceIndex] == ' '){
						Debug.Log ("Stop Here");
						continueCurrentPassage = true;
					}
				}

				if(!continueCurrentPassage){
					sentenceIndex++;
				}
			}else{
				stillWritingCurrentPassage = false;
			}
			uiText.text = textDisplay;
<<<<<<< HEAD
			nameText.text = speaker;
		}
		//Debug.Log (textDisplay);

		//speaker.Trim(charsToTrim);
		//nameText.text = speaker;
=======
			//Debug.Log (uiText.text.Length);
			//Debug.Log ("Line Count: " + uiText.cachedTextGenerator.lineCount);
			nameText.text = speaker;
		}

>>>>>>> master
	}

	void NextSentence(){
		if (!continueCurrentPassage) {
			bool lookingForPassage = true;
			int currentIndex = 0;
			textDisplay = "";
			sentence = "";
			while (lookingForPassage) {
				if (words [currentIndex].StartsWith ("::")) {
					sentenceWords = words [currentIndex].Split (titleSplit);
					sentenceWords[2].Trim(' ');
					for(int i = 0 ; i < sentenceWords.Length; i++){
//						Debug.Log (sentenceWords[i] + "  " + i);
					}
					if (sentenceWords [2].Contains(nextPassage)) {
						lookingForPassage = false;
						currentPassage = nextPassage;
						wordsIndex = currentIndex;
						if (sentenceWords.Length > 3) {
							speaker = sentenceWords [3];
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

			sentenceWords = words [wordsIndex].Split (titleSplit);
			sentence+= sentenceWords[0];

			if(sentenceWords.Length > 2){
				connection = sentenceWords[2];
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

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			Debug.Log ("enter");
			textDisplay = "";
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") {
<<<<<<< HEAD
			Debug.Log ("Exit");
			amelia.SetReadSign(false);
			//Debug.Log ("stop reading");
=======

			amelia.SetReadSign(false);

>>>>>>> master
			textDisplay = "";
			beingRead = false;
			wordsIndex = 0;
			continueCurrentPassage = false;
			stillWritingCurrentPassage = false;
			uiText.enabled = false;
			nextPassage = startingPassage;
			currentPassage = "";
		}
	}

}
