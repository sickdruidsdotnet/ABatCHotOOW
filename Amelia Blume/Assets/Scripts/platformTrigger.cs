using UnityEngine;
using System.Collections;

public class platformTrigger : MonoBehaviour {
	public Transform platParent;
	//need the Player controller to tell if crouching
	public GameObject amelia;
	public PlayerController aController;
	bool playerColliding;


	// Use this for initialization
	void Awake () {
		platParent = transform.parent;

		amelia = GameObject.FindGameObjectWithTag ("Player");
		if (amelia != null) {
			aController = amelia.GetComponent<PlayerController> ();
		}

		playerColliding = false;
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetAxis ("Vertical") < (-1f * (aController.inputDeadZone + 0.4))) {
			Physics.IgnoreCollision (amelia.GetComponent<BoxCollider> (), platParent.collider, true);
			Physics.IgnoreCollision (amelia.GetComponent<CharacterController> (), platParent.collider, true);
		} else if(!playerColliding) {
			Physics.IgnoreCollision (amelia.GetComponent<BoxCollider>(), platParent.collider, false);
			Physics.IgnoreCollision (amelia.GetComponent<CharacterController>(), platParent.collider, false);
		}

	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player") {
			playerColliding = true;
		}
		Physics.IgnoreCollision (other, platParent.collider, true);
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") {
			playerColliding = false;
		}
		Physics.IgnoreCollision (other, platParent.collider, false);
	}
}
