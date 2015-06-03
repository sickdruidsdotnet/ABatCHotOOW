using UnityEngine;
using System.Collections;

public class FP_Relocator : MonoBehaviour {

	public Vector3 point;
	GameObject focusPoint;

	void Start () {
		focusPoint = GameObject.Find ("Dynamic_Focus_Point");
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.tag == "Player") {
			focusPoint.transform.position = point;
		}
	}
}
