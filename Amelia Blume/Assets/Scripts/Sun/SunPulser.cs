using UnityEngine;
using System.Collections;

public class SunPulser : MonoBehaviour {

	Quaternion startRot;
	Vector3 startScale;
	float counter;
	float pulseTime = 0.66666f;
	bool growing;
	float growthRate = 0.06f;
	// Use this for initialization
	void Start () {
		startRot = transform.rotation;
		growing = true;
	}
	

	void FixedUpdate () {
		if (counter <= 0) {
			transform.rotation = startRot;
			counter = pulseTime;
			growing = !growing;
		} else {
			counter-= Time.deltaTime;
			if(growing){
				transform.localScale = new Vector3(transform.localScale.x + (growthRate * Time.deltaTime), 
			                                   transform.localScale.y + (growthRate * Time.deltaTime), 
			                                   transform.localScale.z + (growthRate * Time.deltaTime));
			}
			else
			{
				transform.localScale = new Vector3(transform.localScale.x - (growthRate * Time.deltaTime), 
				                                   transform.localScale.y - (growthRate * Time.deltaTime), 
				                                   transform.localScale.z - (growthRate * Time.deltaTime));
			}
			
		}
		transform.Rotate(new Vector3(0f, 0f, -4f));
	}
}
