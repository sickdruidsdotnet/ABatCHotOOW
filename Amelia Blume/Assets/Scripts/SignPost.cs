using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class SignPost : MonoBehaviour {
	public bool stillWritingCurrentPassage = false;
	private int maxCharCount = 145;
	public bool continueCurrentPassage = false;
	int wordsIndex = 0;
	GameObject player;
	Player amelia;
	string[] words; //Lines of txt file
	public bool beingRead = false;
	public TextAsset file;
	int sentenceIndex = 0;
	string textDisplay = "";
	string sentence = ""; //current line being printed
	string[] sentenceWords; //words in current Sentence
	float delay = 0.03f;
	float nextUse;
	public string startingPassage;
	public string nextPassage;
	public string currentPassage;
	public string speaker;
	string connection;

	public bool doneReading = false;
	public bool cutSceneStart;
	public bool inCutscene;

	bool personSpeaking = false;

	public Sprite[] portraits = new Sprite[3];

	char[] charsToTrim = { '[' ,']'};
	char[] titleSplit = { '[', ']' , ':'};


 	GameObject uiTextObj;

	GameObject buttonObj;
	Image uiButtonSprite;

	GameObject textBoxObj;
	Image uiTextBoxSprite;

	GameObject portraitObj;
	public Image uiPortraitSprite;

	GameObject nameObj;

	public Text uiText;
	Text nameText;

	RectTransform portraitRect;
	RectTransform nameRect;

	//get the canvas we'll be working with
	Canvas uiCanvas;

	private AudioClip pageSound1;
	private AudioClip pageSound2;
	private AudioClip pageSound3;
	private AudioClip pageSound4;
	private AudioSource source;
	public float volLowRange = 0.8F;
	public float volHighRange = 1.0F;
	public float lowPitchRange = 0.85F;
    public float highPitchRange = 1.0F;

	int newLineIndex = 75;
	// Use this for initialization
	void Start () {
		inCutscene = false;
		cutSceneStart = false;
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
		uiPortraitSprite.sprite = portraits [0];
		portraitRect = portraitObj.GetComponent<RectTransform> ();

		buttonObj = GameObject.Find ("Button");
		uiButtonSprite = buttonObj.GetComponent<Image>();
		uiButtonSprite.enabled = false;

		nameObj = GameObject.Find ("Name");
		nameText = nameObj.GetComponent<Text> ();
		nameText.enabled = false;
		nameRect = nameObj.GetComponent<RectTransform> ();

		source = gameObject.AddComponent<AudioSource>();
		
		pageSound1 = Resources.Load("scrollingpageflip_0_edit") as AudioClip;
		pageSound2 = Resources.Load("scrollingpageflip_1_edit") as AudioClip;
		pageSound3 = Resources.Load("scrollingpageflip_5_edit") as AudioClip;
		pageSound4 = Resources.Load("scrollingpageflip_7_edit") as AudioClip;

		//load/put the canvas behind the camera for easier editor experience
		uiCanvas = GameObject.Find ("UI").GetComponent<Canvas> ();
		if (uiCanvas != null) {
			uiCanvas.planeDistance = -1;
		}
	}

	//sorry to jam this in here but it's running into issues with the gamecontroller;
	void ReloadResources()
	{
		beingRead = false;
		Debug.Log ("Reload");
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
		if (uiCanvas == null || uiButtonSprite == null || uiText == null) {
			Debug.Log ("Null stuff");
			ReloadResources();
			return;
		}
		if (Time.time > nextUse) {
			DisplayWords ();
			nextUse = Time.time + delay;
		}

		uiButtonSprite.enabled = uiText.enabled;
		uiTextBoxSprite.enabled = uiText.enabled;
		if (!inCutscene) {
			//Debug.Log("Not Cutscene");
			nameText.enabled = uiText.enabled;
			//Debug.Log (personSpeaking);
			uiPortraitSprite.enabled = (personSpeaking && uiText.enabled);

		} else {
			//Debug.Log ("Cutscene");
			nameText.enabled = uiText.enabled;
			uiPortraitSprite.enabled = uiText.enabled;
		}
		//Debug.Log ("NAME: " + nameText.enabled);
		//Debug.Log ("PORTRAIT: " + uiPortraitSprite.enabled);
		if (uiButtonSprite.enabled) {
			if (uiCanvas != null) {
				uiCanvas.planeDistance = 2;
			}
		}

		if (nameText.text == "Amelia") {
			//Debug.Log (portraitObj.GetComponent<RectTransform>().anchoredPosition);
			personSpeaking = true;
//			Debug.Log ("Amelia Speaking");
			uiPortraitSprite.sprite = portraits [0];
			nameRect.position = GameObject.Find ("Left Name Anchor").GetComponent<RectTransform>().position;
			portraitRect.position = GameObject.Find ("Left Portrait Anchor").GetComponent<RectTransform>().position;
		} else if (nameText.text == "Ignatius") {
			//Debug.Log ("Ig Speaking");
			uiPortraitSprite.sprite = portraits [1];
			personSpeaking = true;
			portraitRect.position = GameObject.Find ("Right Portrait Anchor").GetComponent<RectTransform>().position;
			nameRect.position = GameObject.Find ("Right Name Anchor").GetComponent<RectTransform>().position;
		} else if (nameText.text == "Heart") {
			uiPortraitSprite.sprite = portraits [2];
			personSpeaking = true;
			portraitRect.position = GameObject.Find ("Right Portrait Anchor").GetComponent<RectTransform>().position;
			nameRect.position = GameObject.Find ("Right Name Anchor").GetComponent<RectTransform>().position;
		}else {
			//Debug.Log ("ELSE");
			//uiPortraitSprite.enabled = false;
			//nameText.enabled = false;
			//if(nameText.text == "Ignatius" || nameText.text == "Amelia" || nameText.text == "Heart"){
			//		uiPortraitSprite.enabled = true;
					//personSpeaking = true;
			//}else{
			personSpeaking = false;
			uiPortraitSprite.enabled = false;
			nameText.enabled = false;
			//}
		}
	}

	public void Read(){
		doneReading = false;
		if (currentPassage == nextPassage && !continueCurrentPassage) {
			if(beingRead && stillWritingCurrentPassage){
				DisplayFullText();
				//if(inCutscene)
					//beingRead = false;
				return;
			}else{
				pageFlipSound();			

				if(!cutSceneStart){
					//Debug.Log ("DONE");
					if(inCutscene)
						BroadcastMessage("NextEvent", SendMessageOptions.DontRequireReceiver);
					beingRead = false;
					doneReading = true;
					uiText.enabled = false;
					nextPassage = startingPassage;
					currentPassage = "";
				}else{
					cutSceneStart = false;
					beingRead = true;
				}
			doneReading = true;
			}
			return;
		}

		if(beingRead && stillWritingCurrentPassage){
			DisplayFullText();
			return;
		}
		else {
			pageFlipSound();
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
	/*
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
	*/

	void pageFlipSound(){
		float vol = Random.Range (volLowRange, volHighRange);
		source.pitch = Random.Range(lowPitchRange, highPitchRange);
		switch(Random.Range(0, 4)){
			case 0: source.PlayOneShot(pageSound1, 1.0F);
					break;
			case 1: source.PlayOneShot(pageSound2, 1.0F);
					break;
			case 2: source.PlayOneShot(pageSound3, 1.0F);
					break;
			case 3: source.PlayOneShot(pageSound4, 1.0F);
					break;
		}	
	}

	void DisplayFullText(){
//		Debug.Log ("Show Full");
		if (beingRead) {
			bool keepWriting = true;
				while(keepWriting){
					if(sentenceIndex > sentence.Length-1){
						keepWriting = false;
						//stillWritingCurrentPassage = false;
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
						//stillWritingCurrentPassage = false;
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
						//Debug.Log ("Stop Here");
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
					sentenceWords = words [currentIndex].Split (titleSplit);
					sentenceWords[2].Trim(' ');
					for(int i = 0 ; i < sentenceWords.Length; i++){
						//Debug.Log (sentenceWords[i] + "  " + i);
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
				beingRead = false;
				sentenceIndex = 0;
				Debug.Log ("DONE");
				//BroadcastMessage("PrintThis");
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

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag == "Player") {

			amelia.SetReadSign(false);

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

	public void CutsceneStart(string startPass){
		if (uiText == null){Debug.Log("uiText null!");ReloadResources();}
		uiText.enabled = true;
		inCutscene = true;
		cutSceneStart = true;
		startingPassage = startPass;
		nextPassage = startingPassage;
		//textDisplay = "";
		amelia.SetReadSign(true);
		amelia.SetCurrentSign(this.gameObject);
		Read ();
	}
}
