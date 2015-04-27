using UnityEngine;
using System.Collections;

public class Bramble : MonoBehaviour {
	GameObject player;
	Player amelia;
	float delay = 0.5f;
	float nextUse;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		amelia = player.GetComponent<Player> ();
		nextUse = Time.time + delay;
	}
	
	// Update is called once per frame
	void Update () {
	
	}


	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			if(Time.time > nextUse){
				//Debug.Log ("Player Hit");
				amelia.ReduceHealth(1);
				nextUse = Time.time + delay;
			}
		}
	}	
}
