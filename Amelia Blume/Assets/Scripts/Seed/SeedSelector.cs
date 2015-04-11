using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SeedSelector : MonoBehaviour {

	public GameObject stickImage;
	GameObject player;
	Renderer[] childRenderers;
	public Renderer[] sections;
	public SelectionEffect[] seeds;
	string prevDirection;
	string direction;

	void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
		transform.position = new Vector3 (player.transform.position.x,
		                                 player.transform.position.y + 3f,
		                                 player.transform.position.z);
		childRenderers = GetComponentsInChildren<Renderer> ();
		foreach(Renderer renderer in childRenderers)
			renderer.enabled = false;
		direction = "Up";
		prevDirection = "Up";
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
			float angle = Vector2.Angle(Vector2.up * -1f, new Vector2(horizontal2, vertical2));
			if( angle >= 45 && angle < 135 && horizontal2 > 0){
				sections[0].enabled = false;
				sections[2].enabled = false;
				sections[3].enabled = false;
				direction = "Right";
			}
			else if( angle >= 135 && angle <= 180){
				sections[0].enabled = false;
				sections[1].enabled = false;
				sections[3].enabled = false;
				direction = "Down";
			}
			else if( angle >= 45 && angle < 135 && horizontal2 < 0 ){
				sections[0].enabled = false;
				sections[2].enabled = false;
				sections[1].enabled = false;
				direction = "Left";
			}
			else{
				sections[1].enabled = false;
				sections[2].enabled = false;
				sections[3].enabled = false;
				direction = "Up";
			}
			stickImage.transform.position = new Vector3 (transform.position.x + (horizontal2 / 3.5f),
			                                  			 transform.position.y - (vertical2 / 3.5f),
			                                             stickImage.transform.position.z);

			//let's get some cool rotation in here
			if(prevDirection != direction){
			switch (direction){
				case "Up":
					seeds[0].StartEffect();
					break;
				case "Right":
					seeds[1].StartEffect();
					break;
				case "Down":
					seeds[2].StartEffect();
					break;
				case "Left":
					seeds[3].StartEffect();
					break;
				}

				switch (prevDirection){
				case "Up":
					seeds[0].EndEffect();
					break;
				case "Right":
					seeds[1].EndEffect();
					break;
				case "Down":
					seeds[2].EndEffect();
					break;
				case "Left":
					seeds[3].EndEffect();
					break;
				}
			}
			prevDirection = direction;

		} else {
			foreach(Renderer renderer in childRenderers)
				renderer.enabled = false;
		}
	}
}
