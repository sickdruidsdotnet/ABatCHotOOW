using UnityEngine;
using System.Collections;

public class SelectionEffect : MonoBehaviour {

	bool active = false;
	bool grow = false;
	float startTime;
	
	// Update is called once per frame
	void FixedUpdate () {
		if (active) {
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
					active = false;
			}
		}
	
	}

	public void StartEffect()
	{
		active = true;
		grow = true;
		startTime = Time.time;

	}

	public void EndEffect()
	{
		active = true;
		grow = false;
		startTime = Time.time;
		
	}
}
