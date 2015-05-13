using UnityEngine;
using System.Collections;

public class Bramble : MonoBehaviour {
	GameObject player;
	Player amelia;
	//float delay = 0.5f;
	//float nextUse;
	int invulCounter;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		amelia = player.GetComponent<Player> ();
		//nextUse = Time.time + delay;
		invulCounter = 0;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if(invulCounter >0)
			invulCounter --;
	}

	void OnTriggerEnter(Collider other)
	{
		/*if (other.tag == "Player") {
			//make sure the player isn't in stun before bouncing to prevent exponential force addition
			if (other.GetComponent<PlayerController> ().stunTimer <= 0 || other.GetComponent<PlayerController> ().canControl == true) {	
				int hitDirection = other.GetComponent<PlayerController> ().faceDirection;
				rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
				rigidbody.freezeRotation = true;
				other.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * -4, 8f, 0f), 100f);
				other.GetComponent<PlayerController> ().canControl = false;
				other.GetComponent<PlayerController> ().stunTimer = 30;
				amelia.ReduceHealth(10);
			}
		}*/
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player" && invulCounter <= 0) {
			if (other.GetComponent<PlayerController> ().stunTimer <= 0 || other.GetComponent<PlayerController> ().canControl == true) {	
				int hitDirection = other.GetComponent<PlayerController> ().faceDirection;
				other.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * -4, 8f, 0f), 100f);
				other.GetComponent<PlayerController> ().canControl = false;
				other.GetComponent<PlayerController> ().stunTimer = 25;
				invulCounter = 60;
				amelia.ReduceHealth(10);
			}
		}
	}	
}
