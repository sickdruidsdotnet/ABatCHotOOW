using UnityEngine;
using System.Collections;

public class InputHandler : MonoBehaviour {

	public string[] connectedJoysticks;
	public string primaryInput;

	public float xMove;
	public float yMove;
	public float xSelect;
	public float ySelect;
	public bool jump;
	public bool throwSeed;
	public bool sun;
	public bool water;
	public float waterStrength;
	public bool dash;
	public bool start;
	public bool select;

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
		switch (primaryInput) {
		case "Keyboard":
			KeyBoard();
			break;
		case "Controller (XBOX 360 For Windows)":
			Xbox360Controller();
			break;
		}
	}

	void KeyBoard(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		//need keyboard seed selection input

		jump = Input.GetButtonDown ("Jump");
		throwSeed = Input.GetButtonDown ("ThrowSeed");
	}

	void Xbox360Controller(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		xSelect = Input.GetAxis ("Horizontal 3");
		ySelect = Input.GetAxis ("Vertical 3");
		jump = Input.GetButtonDown ("joystick button 0");
		throwSeed = Input.GetButtonDown ("joystick button 2");
	}

	void Playstation4Controller(){
		xMove = Input.GetAxis("Horizontal");
		yMove = Input.GetAxis ("Vertical");
		xSelect = Input.GetAxis ("Horizontal 3");
		ySelect = Input.GetAxis ("Vertical 3");
		jump = Input.GetButtonDown ("joystick button 3");
		throwSeed = throwSeed = Input.GetButtonDown ("joystick button 6");
	}
}
