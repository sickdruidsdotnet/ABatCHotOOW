﻿using UnityEngine;
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
		if (platParent == null)
		{
			platParent = transform.parent;
		}
		if (amelia == null)
		{
			Debug.Log("amelia null");
		}
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
		if (platParent == null)
		{
			platParent = transform.parent;
		}
		if (other == null){Debug.Log("platformTrigger \"other\" is null.");}
		if (platParent == null){Debug.Log("platformTrigger \"platParent\" is null.");}
		if (platParent.collider == null){Debug.Log("platformTrigger \"platParent.collider\" is null.");}
		Physics.IgnoreCollision (other, platParent.collider, true);
	}
	
	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Player") {
			playerColliding = false;
		}
		if (platParent == null)
		{
			platParent = transform.parent;
		}
		
		Physics.IgnoreCollision (other, platParent.collider, false);
	}
}
