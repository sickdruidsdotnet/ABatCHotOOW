using UnityEngine;
using System.Collections;

public class SpawnFruit : MonoBehaviour {
	GameObject amelia;
	Player script;
	PlayerController controller;

	// Use this for initialization
	void Start () {
		amelia = GameObject.Find ("Player");
		script = amelia.GetComponent<Player> ();
		controller = amelia.GetComponent<PlayerController> ();
		//ignore collisions with the player
        Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider> (), transform.collider, true);
		Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController> (), transform.collider, true);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (script.GetSpawn ()) {
			this.GetComponent<Rigidbody>().useGravity = true;
		} else {
			this.GetComponent<Rigidbody>().useGravity = false;
		}
	}

	void OnCollisionEnter(Collision other){
		if (other.transform.tag != "Player") {
			script.SetSpawn(false);
			script.SetCanGrow (true);
			script.SetDead(false);
			controller.canControl = true;
			Destroy (this.gameObject);
		}
	}
}
