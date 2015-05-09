using UnityEngine;
using System.Collections;

public class SunRay : MonoBehaviour {
	float transparencyMax = 0.5f;
	float rotationSpeed;
	Vector3 pivot;
	public int startLife;
	public int life = 60;
	public int lifeThirds;
	public float transparencySpeed;

	// Use this for initialization
	void Start () {
		Color newColor = new Color(renderer.material.color.r, 
		                           renderer.material.color.g,
		                           renderer.material.color.b,
		                           0f);
		renderer.material.SetColor ("_Color", newColor);
		pivot = transform.parent.transform.position;
		life = Random.Range(40, 180);
		startLife = life;
		lifeThirds = life/3;
		transform.RotateAround (pivot, Vector3.back, Random.Range(0, 360));
		rotationSpeed = Random.Range (1, 3);
		if (Random.Range (0, 5) > 2)
			rotationSpeed *= -1;
		transparencySpeed = (float)transparencyMax / (float)lifeThirds;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		pivot = transform.parent.transform.position;
		life--;
		if (life <= 0) {
			Color newColor = new Color (renderer.material.color.r, 
			                           renderer.material.color.g,
			                           renderer.material.color.b,
			                           0f);
			renderer.material.SetColor ("_Color", newColor);
			transform.RotateAround (pivot, Vector3.back, Random.Range (0, 360));
			life = Random.Range (40, 180);
			startLife = life;
			lifeThirds = life / 3;
			transparencySpeed = (float)transparencyMax / (float)lifeThirds;
			rotationSpeed = Random.Range (1, 3);
			if (Random.Range (0, 5) > 2)
				rotationSpeed *= -1;

			//change the length for more variety
			transform.localScale = new Vector3(transform.localScale.x, transform.localScale.y, Random.Range(0.1f, 0.5f));
		} else if (life > startLife - lifeThirds) {
			Color newColor = new Color (renderer.material.color.r, 
			                            renderer.material.color.g,
			                            renderer.material.color.b,
			                            renderer.material.color.a + transparencySpeed);
			renderer.material.SetColor ("_Color", newColor);
			transform.RotateAround (pivot, Vector3.back, rotationSpeed);
		} else if (life < startLife - (2 * lifeThirds) ) {
			Color newColor = new Color (renderer.material.color.r, 
			                            renderer.material.color.g,
			                            renderer.material.color.b,
			                            renderer.material.color.a - transparencySpeed);
			renderer.material.SetColor ("_Color", newColor);
			transform.RotateAround (pivot, Vector3.back, rotationSpeed);
		} else {
			//rotate as normal;
			transform.RotateAround (pivot, Vector3.back, rotationSpeed);
		}
	}
}
