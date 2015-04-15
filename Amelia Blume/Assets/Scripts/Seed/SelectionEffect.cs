using UnityEngine;
using System.Collections;

public class SelectionEffect : MonoBehaviour {

	bool activated = false;
	bool grow = false;
	float startTime;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (activated) {
			if(grow)
			{
				transform.localScale = new Vector3(Mathf.SmoothStep(transform.localScale.x, 4.4f, (Time.time - startTime)/0.4f),
				                                   Mathf.SmoothStep(transform.localScale.y, 4.4f, (Time.time - startTime)/0.4f),
				                                   Mathf.SmoothStep(transform.localScale.z, 4.4f, (Time.time - startTime)/0.4f));
			}
			else{
				transform.localScale = new Vector3(Mathf.SmoothStep(transform.localScale.x, 3f, (Time.time - startTime)/0.4f),
				                                   Mathf.SmoothStep(transform.localScale.y, 3f, (Time.time - startTime)/0.4f),
				                                   Mathf.SmoothStep(transform.localScale.z, 3f, (Time.time - startTime)/ 0.4f));
				if(transform.localScale.x == 3)
					activated = false;
			}
		}
	
	}

	public void StartEffect()
	{
		activated = true;
		grow = true;
		startTime = Time.time;

	}

	public void EndEffect()
	{
		activated = true;
		grow = false;
		startTime = Time.time;
		
	}
}
