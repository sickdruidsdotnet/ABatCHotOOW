using UnityEngine;
using System.Collections;

public class Sun : MonoBehaviour {
	private GameObject player;
	// Use this for initialization
	void Start () {
		player = GameObject.Find ("Player");
		this.gameObject.name = "Sun";
	}
	
	// Update is called once per frame
	void Update () {
		HandleInput ();
	}

	protected void HandleInput() {
		if (Input.GetButtonUp ("Sun")) {
			player.GetComponent<Player>().SetSunning(false);
			Destroy (this.gameObject);
		}
	}
}
