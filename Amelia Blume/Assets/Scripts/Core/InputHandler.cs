using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

	public bool paused = false;

	public GameObject pauseCanvas;

	public string[] connectedJoysticks;
	public string primaryInput;

	//axes
	public float xMove;
	public float yMove;
	public float xSelect;
	public float ySelect;

	//buttons and their up and down states
	#region
	public bool jumpDown;
	public bool jump;
	public bool jumpUp;
	#endregion

	#region
	public bool throwSeedDown;
	public bool throwSeed;
	public bool throwSeedUp;
	#endregion

	#region
	public bool sunDown;
	public bool sun;
	public bool sunUp;
	#endregion

	#region
	public bool waterDown;
	public bool water;
	public bool waterUp;
	public float waterStrength;
	#endregion

	#region
	public bool dashDown;
	public bool dash;
	public bool dashUp;
	#endregion

	#region
	public bool startDown;
	public bool start;
	public bool startUp;
	#endregion

	#region
	public bool selectDown;
	public bool select;
	public bool selectUp;
	#endregion

	//keyboard only seed selection
	#region
	public bool firstSeedDown;
	public bool firstSeed;
	public bool secondSeedDown;
	public bool secondSeed;
	public bool thirdSeedDown;
	public bool thirdSeed;
	#endregion

	// Use this for initialization
	void Start () {
		connectedJoysticks = Input.GetJoystickNames ();
		if (connectedJoysticks.Length == 0)
			primaryInput = "Keyboard";
		else {
			primaryInput = connectedJoysticks[0];
		}

		pauseCanvas = GameObject.FindGameObjectWithTag ("Pause Canvas");
	}
	
	// Update is called once per frame
	void Update () {
		//switch to keyboard if key pressed
		if (Input.anyKeyDown && primaryInput != "Keyboard") {
			primaryInput = "Keyboard";
		}

		//switch to gamepad if any button pressed. There is no elegant way of doing this.
		if (connectedJoysticks.Length != 0 && primaryInput != connectedJoysticks[0]) {
			if(Input.GetButtonDown("joystick button 1") || Input.GetButtonDown("joystick button 2") ||
			   Input.GetButtonDown("joystick button 3") || Input.GetButtonDown("joystick button 4") ||
			   Input.GetButtonDown("joystick button 5") || Input.GetButtonDown("joystick button 6") ||
			   Input.GetButtonDown("joystick button 7") || Input.GetButtonDown("joystick button 8") ||
			   Input.GetButtonDown("joystick button 9") || Input.GetButtonDown("joystick button 0") ||
			   Input.GetButtonDown("joystick button 10") || Input.GetButtonDown("joystick button 11") ||
			   Input.GetButtonDown("joystick button 12") || Input.GetButtonDown("joystick button 13") ||
			   Input.GetButtonDown("joystick button 14") || Input.GetButtonDown("joystick button 15") ||
			   Input.GetButtonDown("joystick button 16") || Input.GetButtonDown("joystick button 17") ||
			   Input.GetButtonDown("joystick button 18") || Input.GetButtonDown("joystick button 19"))
			{
				primaryInput = connectedJoysticks[0];
			}
		}

		switch (primaryInput) {
		case "Keyboard":
			KeyBoard();
			break;
		case "Controller (XBOX 360 For Windows)":
			Xbox360Controller();
			break;
		case "Sony Computer Entertainment Wireless Controller":
			Playstation4Controller();
			break;
		case "©Microsoft Corporation Xbox 360 Wired Controller":
			Xbox360ControllerMac();
			break;
		}

		if(startDown)
		{
			if(!paused){
				paused = true;
				pauseCanvas.BroadcastMessage("Pause");
			}
			else
			{
				paused = false;
				pauseCanvas.BroadcastMessage("UnPause");
			}
		}

	}

	void KeyBoard(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");

		firstSeedDown = Input.GetKeyDown (KeyCode.Alpha1);
		firstSeed = Input.GetKey (KeyCode.Alpha1);
		secondSeedDown = Input.GetKeyDown (KeyCode.Alpha2);
		secondSeed = Input.GetKey (KeyCode.Alpha2);
		thirdSeedDown = Input.GetKeyDown (KeyCode.Alpha3);
		thirdSeed = Input.GetKey (KeyCode.Alpha3);

		jumpDown = Input.GetButtonDown ("Jump");
		jump = Input.GetButton ("Jump");
		jumpUp = Input.GetButtonUp ("Jump");

		sunDown = Input.GetButtonDown ("Sun");
		sun = Input.GetButton ("Sun");
		sunUp = Input.GetButtonUp ("Sun");

		throwSeedDown = Input.GetButtonDown ("ThrowSeed");
		throwSeed = Input.GetButton ("ThrowSeed");
		throwSeedUp = Input.GetButtonUp ("ThrowSeed");

		dashDown = Input.GetButtonDown ("Dash");
		dash = Input.GetButton ("Dash");
		dashUp = Input.GetButtonUp ("Dash");

		waterDown = Input.GetButtonDown ("Water");
		water = Input.GetButton ("Water");
		waterUp = Input.GetButtonUp ("Water");

		startDown = Input.GetKeyDown (KeyCode.Return);
		start = Input.GetKey (KeyCode.Return);
		startUp = Input.GetKeyUp (KeyCode.Return);
	}

	void Xbox360Controller(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		xSelect = Input.GetAxis ("Horizontal 3");
		ySelect = Input.GetAxis ("Vertical 3");

		jumpDown = Input.GetButtonDown ("joystick button 0");
		jump = Input.GetButton ("joystick button 0");
		jumpUp = Input.GetButtonUp ("joystick button 0");

		throwSeedDown = Input.GetButtonDown ("joystick button 2");
		throwSeed = Input.GetButton ("joystick button 2");
		throwSeedUp = Input.GetButtonUp ("joystick button 2");

		waterDown = Input.GetButtonDown ("joystick button 1");
		water = Input.GetButton ("joystick button 1");
		waterUp = Input.GetButtonUp ("joystick button 1");

		sunDown = Input.GetButtonDown ("joystick button 3");
		sun = Input.GetButton ("joystick button 3");
		sunUp = Input.GetButtonUp ("joystick button 3");

		dashDown = Input.GetButtonDown ("joystick button 5");
		dash = Input.GetButton ("joystick button 5");
		dashUp = Input.GetButtonUp ("joystick button 5");

		selectDown = Input.GetButtonDown ("joystick button 6");
		select = Input.GetButton ("joystick button 6");
		selectUp = Input.GetButtonUp ("joystick button 6");

		startDown = Input.GetButtonDown ("joystick button 7");
		start = Input.GetButton ("joystick button 7");
		startUp = Input.GetButtonUp ("joystick button 7");

	}

	void Xbox360ControllerMac(){
		//Debug.Log (Input.GetButtonDown ());
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		xSelect = Input.GetAxis ("Horizontal 4");
		ySelect = Input.GetAxis ("Vertical 4");
		Debug.Log ("X-Axis: " + xSelect);
		Debug.Log ("Y-Axis: " + ySelect);

		jumpDown = Input.GetButtonDown ("joystick button 16");
		jump = Input.GetButton ("joystick button 16");
		jumpUp = Input.GetButtonUp ("joystick button 16");
		
		throwSeedDown = Input.GetButtonDown ("joystick button 18");
		throwSeed = Input.GetButton ("joystick button 18");
		throwSeedUp = Input.GetButtonUp ("joystick button 18");
		
		waterDown = Input.GetButtonDown ("joystick button 17");
		water = Input.GetButton ("joystick button 17");
		waterUp = Input.GetButtonUp ("joystick button 17");
		
		sunDown = Input.GetButtonDown ("joystick button 19");
		sun = Input.GetButton ("joystick button 19");
		sunUp = Input.GetButtonUp ("joystick button 19");
		
		dashDown = Input.GetButtonDown ("joystick button 14");
		dash = Input.GetButton ("joystick button 14");
		dashUp = Input.GetButtonUp ("joystick button 14");
		
		selectDown = Input.GetButtonDown ("joystick button 10");
		select = Input.GetButton ("joystick button 10");
		selectUp = Input.GetButtonUp ("joystick button 10");
		
		startDown = Input.GetButtonDown ("joystick button 9");
		start = Input.GetButton ("joystick button 9");
		startUp = Input.GetButtonUp ("joystick button 9");

		
	}


	void Playstation4Controller(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		xSelect = Input.GetAxis ("Horizontal 3");
		ySelect = Input.GetAxis ("Vertical 3");

		jumpDown = Input.GetButtonDown ("joystick button 1");
		jump = Input.GetButton ("joystick button 1");
		jumpUp = Input.GetButtonUp ("joystick button 1");
		
		throwSeedDown = Input.GetButtonDown ("joystick button 0");
		throwSeed = Input.GetButton ("joystick button 0");
		throwSeedUp = Input.GetButtonUp ("joystick button 0");
		
		waterDown = Input.GetButtonDown ("joystick button 2");
		water = Input.GetButton ("joystick button 2");
		waterUp = Input.GetButtonUp ("joystick button 2");
		
		sunDown = Input.GetButtonDown ("joystick button 3");
		sun = Input.GetButton ("joystick button 3");
		sunUp = Input.GetButtonUp ("joystick button 3");
		
		dashDown = Input.GetButtonDown ("joystick button 5");
		dash = Input.GetButton ("joystick button 5");
		dashUp = Input.GetButtonUp ("joystick button 5");
		
		selectDown = Input.GetButtonDown ("joystick button 6");
		select = Input.GetButton ("joystick button 6");
		selectUp = Input.GetButtonUp ("joystick button 6");
		
		startDown = Input.GetButtonDown ("joystick button 7");
		start = Input.GetButton ("joystick button 7");
		startUp = Input.GetButtonUp ("joystick button 7");
	}
}
