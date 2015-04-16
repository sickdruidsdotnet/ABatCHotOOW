using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

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

	// Use this for initialization
	void Start () {
		connectedJoysticks = Input.GetJoystickNames ();
		if (connectedJoysticks.Length == 0)
			primaryInput = "Keyboard";
		else {
			primaryInput = connectedJoysticks[0];
		}
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
			   Input.GetButtonDown("joystick button 9") || Input.GetButtonDown("joystick button 0"))
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
		}
	}

	void KeyBoard(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		//need keyboard seed selection input

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
