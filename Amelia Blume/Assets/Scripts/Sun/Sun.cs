using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	private GameObject player;
	public InputHandler playerInput;
	// Use this for initialization
	void Start () {
		playerInput = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();
		player = GameObject.Find ("Player");
		gameObject.name = "Sun";

	}
	
	// Update is called once per frame
	void Update () {
		HandleInput ();
	}

	void FixedUpdate()
	{
		//if (transform.position.x != player.transform.position.x || transform.position.y != player.transform.position.y + 2f) {
		transform.position = new Vector3((player.transform.position.x +(0.5f* player.GetComponent<PlayerController>().faceDirection)),
			                                 player.transform.position.y + 2f, transform.position.z);
		//}
	}

	protected void HandleInput() {
		if (playerInput.sunUp) {
			player.GetComponent<Player>().SetSunning(false);
			Destroy (gameObject);
		}
	}
}
