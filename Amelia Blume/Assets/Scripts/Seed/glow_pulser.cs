using UnityEngine;
using System.Collections;

public class glow_pulser : MonoBehaviour {


	// Update is called once per frame
	void Update () {
			transform.localScale = new Vector3((Mathf.Sin(Time.time * 2) * 0.05f) + 0.15f, 
			                                   (Mathf.Sin(Time.time * 2) * 0.05f) + 0.15f, 
			                                   (Mathf.Sin(Time.time * 2) * 0.05f) + 0.15f);

	}
}
