using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Sun : MonoBehaviour {
	private GameObject player;
	public InputHandler playerInput;
	public List<GameObject> targets;
	// Use this for initialization
	void Start () {
		playerInput = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();
		player = GameObject.Find ("Player");
		gameObject.name = "Sun";
		//get all plants this will target
		transform.position = new Vector3((player.transform.position.x +(0.5f* player.GetComponent<PlayerController>().faceDirection)),
		                                 player.transform.position.y + 2f, transform.position.z);
		targets = new List<GameObject> ();
		GameObject[] allPlants = GameObject.FindGameObjectsWithTag ("Plant");
		foreach (GameObject plant in allPlants) {
			float dist = Vector3.Distance(transform.position, plant.transform.position);
			if( dist <=8)
			{
				targets.Add (plant);
			}
		}

		SunRay[] childRays = gameObject.GetComponentsInChildren<SunRay> ();
		for (int i = 0; i < targets.Count; i++) {
			//redundant check to fix errors
			if(i < targets.Count){
				childRays[i].target = targets[i];
			}
		}
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
