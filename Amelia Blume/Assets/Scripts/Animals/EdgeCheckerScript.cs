using UnityEngine;
using System.Collections;

public class EdgeCheckerScript : MonoBehaviour {

	private Deer parentDeer;
	private int count;

	// Use this for initialization
	void Start () {
		parentDeer = transform.parent.GetComponent<Deer> ();
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (count == 0) {
			parentDeer.beginRotate();
		}
	}

	void OnTriggerEnter()
	{
		count++;
	}

	void OnTriggerExit()
	{
		count--;
	}
}
