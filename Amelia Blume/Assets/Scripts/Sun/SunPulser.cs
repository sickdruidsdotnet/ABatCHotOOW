using UnityEngine;
using System.Collections;

public class SunPulser : MonoBehaviour {

	Quaternion startRot;
	Vector3 startScale;
	int counter;
	int pulseTime = 40;
	bool growing;
	// Use this for initialization
	void Start () {
		startRot = transform.rotation;
		growing = true;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (counter <= 0) {
			transform.rotation = startRot;
			counter = pulseTime;
			growing = !growing;
		} else {
			counter--;
			if(growing){
				transform.localScale = new Vector3(transform.localScale.x + 0.001f, 
			                                   transform.localScale.y + 0.001f, 
			                                   transform.localScale.z + 0.001f);
			}
			else
			{
				transform.localScale = new Vector3(transform.localScale.x - 0.001f, 
				                                   transform.localScale.y - 0.001f, 
				                                   transform.localScale.z - 0.001f);
			}
			
		}
		transform.Rotate(new Vector3(0f, 0f, -4f));
	}
}
