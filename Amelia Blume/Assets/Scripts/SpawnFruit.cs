using UnityEngine;
using System.Collections;

public class SpawnFruit : MonoBehaviour {

	// Use this for initialization
	void Start () {
		//ignore collisions with the player
        Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider> (), transform.collider, true);
		Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController> (), transform.collider, true);
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision other){
		if (other.transform.tag != "Player") {
			GameObject amelia = GameObject.Find ("Player");
			Player script = amelia.GetComponent<Player> ();
			script.SetCanGrow (true);
			Debug.Log ("Destroy fruit");
			Destroy (this.gameObject);
		}
	}
}
