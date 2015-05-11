using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class pause_handler : MonoBehaviour {
	InputHandler input;
	public Text pauseText;
	public Text[] children;
	public Button[] childButtons;
	public Image controllerMap;
	ColorBlock[] cbs;

	bool controllerControlled;
	public bool recentDirection = false;
	public int activeButton = 0;
		// Use this for initialization
	void Start () {

		//grab the camera
		this.GetComponent<Canvas> ().worldCamera = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();

		pauseText = this.GetComponent<Text> ();
		pauseText.text = "";

		input = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();

		foreach (Text item in children) {
			item.text = "";
		}

		foreach (Button thing in childButtons) {
			thing.enabled = false;
		}

		if (input.primaryInput != "Keyboard") {
			controllerControlled = true;
		}
		cbs = new ColorBlock[childButtons.Length];

		for (int i = 0; i < 3; i++) {
			cbs [i] = childButtons [i].colors;
		}

		controllerMap.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (controllerControlled && input.paused) {
			//if they selected an action
			if(input.jumpDown)
			{
				PushedSelectable();
			}
			else
			{
				//if they pressed down and hadn't previously
				if(!recentDirection)
				{
					if(input.yMove < -0.5f)
					{
						MoveSelectionDown();
						recentDirection = true;
					}
					else if(input.yMove > 0.5f)
					{
						MoveSelectionUp();
						recentDirection = true;
					}
				}
				else if(Mathf.Abs(input.yMove) < 0.5f)
					recentDirection = false;
			}
		}
	}

	public void Pause(){
		Time.timeScale = 0;

		pauseText.text = "Paused";
		children [0].text = "Continue";
		children [1].text = "Main Menu";
		children [2].text = "Exit Game";
		controllerMap.enabled = true;
		foreach (Button thing in childButtons) {
			thing.enabled = true;
		}
		input.paused = true;

		//if it's not keyboard make sure these actions are doable via controller input
		if (input.primaryInput != "Keyboard") {
			Color tempColor = childButtons[0].colors.highlightedColor;
			ColorBlock cb = childButtons[0].colors;
			cb.normalColor = tempColor;

			childButtons[0].colors = cb;
		}
	}

	public void UnPause(){
		Time.timeScale = 1.0f;
		pauseText.text = "";

		foreach (Text item in children) {
			item.text = "";
		}

		foreach (Button thing in childButtons) {
			thing.enabled = false;
		}
		input.paused = false;

		for (int i = 0; i < 3; i++) {
			childButtons[i].colors = cbs[i];
		}
		controllerMap.enabled = false;
	}

	public void GoToMain(){
		UnPause ();
		Application.LoadLevel (0);
	}

	public void ExitGame(){
		UnPause ();
		Application.Quit ();
	}

	void PushedSelectable(){
		switch (activeButton) {
		case 0:
			UnPause();
			break;
		case 1:
			GoToMain();
			break;
		case 2:
			ExitGame();
			break;
		}
	}

	void MoveSelectionUp(){
		Color tempColor;
		ColorBlock cb;
		switch (activeButton) {
		case 0:
			//recolor next button
			tempColor = cbs[2].highlightedColor;
			cb = cbs[2];
			cb.normalColor = tempColor;
			childButtons[2].colors = cb;
			//recolor old button
			tempColor = cbs[0].normalColor;
			cb = cbs[0];
			cb.normalColor = tempColor;
			childButtons[0].colors = cb;
			//set new active button
			activeButton = 2;
			break;
		case 1:
			//recolor next button
			tempColor = cbs[0].highlightedColor;
			cb = cbs[0];
			cb.normalColor = tempColor;
			childButtons[0].colors = cb;
			//recolor old button
			tempColor = cbs[1].normalColor;
			cb = cbs[1];
			cb.normalColor = tempColor;
			childButtons[1].colors = cb;
			//set new active button
			activeButton = 0;
			break;
		case 2:
			//recolor next button
			tempColor = cbs[1].highlightedColor;
			cb = cbs[1];
			cb.normalColor = tempColor;
			childButtons[1].colors = cb;
			//recolor old button
			tempColor = cbs[2].normalColor;
			cb = cbs[2];
			cb.normalColor = tempColor;
			childButtons[2].colors = cb;
			//set new active button
			activeButton = 1;
			break;
		}
	}

	void MoveSelectionDown(){
		Color tempColor;
		ColorBlock cb;
		switch (activeButton) {
		case 0:
			//recolor next button
			tempColor = cbs[1].highlightedColor;
			cb = cbs[1];
			cb.normalColor = tempColor;
			childButtons[1].colors = cb;
			//recolor old button
			tempColor = cbs[0].normalColor;
			cb = cbs[0];
			cb.normalColor = tempColor;
			childButtons[0].colors = cb;
			//set new active button
			activeButton = 1;
			break;
		case 1:
			//recolor next button
			tempColor = cbs[2].highlightedColor;
			cb = cbs[2];
			cb.normalColor = tempColor;
			childButtons[2].colors = cb;
			//recolor old button
			tempColor = cbs[1].normalColor;
			cb = cbs[1];
			cb.normalColor = tempColor;
			childButtons[1].colors = cb;
			//set new active button
			activeButton = 2;
			break;
		case 2:
			//recolor next button
			tempColor = cbs[0].highlightedColor;
			cb = cbs[0];
			cb.normalColor = tempColor;
			childButtons[0].colors = cb;
			//recolor old button
			tempColor = cbs[2].normalColor;
			cb = cbs[2];
			cb.normalColor = tempColor;
			childButtons[2].colors = cb;
			//set new active button
			activeButton = 0;
			break;
		}
	}
}
