﻿using UnityEngine;
using System.Collections;

public class Unlock_Flower : MonoBehaviour {

	SignPost sign;

	bool hasStartedReading = false;

	public GameObject fseed;

	Player amelia;
	// Use this for initialization
	void Start () {
		sign = GetComponent<SignPost> ();
		amelia = GameObject.FindGameObjectWithTag ("Player").GetComponent<Player> ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (hasStartedReading) {
			//check if they have finished reading, if so unlock flower and destroy
			if(!sign.beingRead)
			{
				amelia.fluerUnlocked = true;
				Destroy (fseed);
				transform.Translate(new Vector3(0,0, 6));
				hasStartedReading = false;
			}
		} else {
			//check if they are reading, if so mark hasStartedReading to true
			if(sign.beingRead)
			{
				hasStartedReading = true;
			}
		}
	}
}
