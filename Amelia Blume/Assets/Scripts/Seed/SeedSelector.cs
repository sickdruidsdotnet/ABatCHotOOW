using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SeedSelector : MonoBehaviour {

	public GameObject stickImage;
	GameObject player;
	Renderer[] childRenderers;

	void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
		transform.position = new Vector3 (player.transform.position.x,
		                                 player.transform.position.y + 3f,
		                                 player.transform.position.z);
		childRenderers = GetComponentsInChildren<Renderer> ();
		foreach(Renderer renderer in childRenderers)
			renderer.enabled = false;
	}

	// Update is called once per frame
	void FixedUpdate () {

		transform.position = new Vector3 (player.transform.position.x,
		                                  player.transform.position.y + 3f,
		                                  player.transform.position.z);
		float horizontal2 = Input.GetAxis("Horizontal 3");
		float vertical2 = Input.GetAxis ("Vertical 3");
		if (new Vector2 (horizontal2, vertical2).magnitude >= 0.3f) {
			foreach(Renderer renderer in childRenderers)
				renderer.enabled = true;
			stickImage.transform.position = new Vector3 (transform.position.x + (horizontal2 / 3f),
			                                   transform.position.y - (vertical2 / 3f),
			                                   transform.position.z);
		} else {
			foreach(Renderer renderer in childRenderers)
				renderer.enabled = false;
		}
	}
}
