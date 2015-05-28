using UnityEngine;
using System.Collections;

public class Bramble : MonoBehaviour {
	GameObject player;
	int invulCounter;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		invulCounter = 0;
	}

	void FixedUpdate () {
		if(invulCounter >0)
			invulCounter --;
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player") {
			player.GetComponent<PlayerController>().damagePlayer(10);
		}
	}	
}
