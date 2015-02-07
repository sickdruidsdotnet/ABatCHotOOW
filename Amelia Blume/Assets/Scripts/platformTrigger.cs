using UnityEngine;
using System.Collections;

public class platformTrigger : MonoBehaviour {

	// Use this for initialization
	void Start () {
		transform.parent.collider.isTrigger = true;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerEnter(Collider other)
	{
		//Debug.Log("OnTriggerEnter called");
		if (other.name == "Player")
			transform.parent.collider.isTrigger = false;
	}
	
	void OnTriggerExit(Collider other)
	{
		//Debug.Log("OnTriggerExit called");
		if (other.name == "Player")
			transform.parent.collider.isTrigger = true;
	}
}
