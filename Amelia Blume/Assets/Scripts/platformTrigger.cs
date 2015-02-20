using UnityEngine;
using System.Collections;

public class platformTrigger : MonoBehaviour {
	Transform platParent;
	// Use this for initialization
	void Start () {
		platParent = transform.parent;
	}
	
	// Update is called once per frame
	void Update () {
	}
	
	void OnTriggerStay(Collider other)
	{
		Physics.IgnoreCollision (other, platParent.collider, true);
	}
	
	void OnTriggerExit(Collider other)
	{
		Physics.IgnoreCollision (other, platParent.collider, false);
	}
}
