using UnityEngine;
using System.Collections;

public class ringPulser : MonoBehaviour {
	
	Vector3 startScale;
	float transparency = 0.8f;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (transparency <= 0) {
			transform.localScale = startScale;
			transparency = 0.8f;
		} else {
			transparency -= 0.01f;
			transform.localScale = new Vector3(transform.localScale.x + 0.13f, 
			                                   transform.localScale.y, 
			                                   transform.localScale.z + 0.13f);
			Color newColor = new Color(renderer.material.color.r, 
			                           renderer.material.color.g,
			                           renderer.material.color.b,
			                           transparency);
			renderer.material.SetColor("_Color", newColor);
		}
		transform.Rotate(new Vector3(0f, 4f, 0f));
	}
}
