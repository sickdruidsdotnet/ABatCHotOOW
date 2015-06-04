using UnityEngine;
using System.Collections;

public class Bramble : MonoBehaviour {
	GameObject player;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}

	void FixedUpdate () {
	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Player") {
			player.GetComponent<PlayerController>().damagePlayer(10);
		}
	}	
}
