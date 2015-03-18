using UnityEngine;
using System.Collections;

public class ConverterRay : MonoBehaviour {
	float transparencyMax = 0.5f;
	float rotationSpeed;
	Vector3 pivot;
	public bool isMainRay;
	public int startLife;
	public int life = 60;
	public int lifeThirds;
	public float transparencySpeed;
	
	// Use this for initialization
	void Start () {
		if (!isMainRay) {
			Color newColor = new Color(0.95f, 
			                           0.95f,
			                           0.95f,
			                           0f);
			renderer.material.SetColor ("_Color", newColor);
			pivot = transform.parent.transform.position;
			life = Random.Range (40, 180);
			startLife = life;
			lifeThirds = life / 3;
			transform.RotateAround (pivot, Vector3.back, Random.Range (0, 180) - 90);
			//set up a rotation speed that will land it squarely on the bottom
			float distanceFromBottom;
			if(transform.rotation.eulerAngles.x > 270f)
				distanceFromBottom = 90f + (360f - transform.rotation.eulerAngles.x);
			else
				distanceFromBottom = 90f - transform.rotation.eulerAngles.x;
			rotationSpeed = distanceFromBottom/ startLife;
			transparencySpeed = (float)transparencyMax / (float)lifeThirds;
		} else {
			startLife = 200;
			life = startLife;
			transform.Rotate(new Vector3(180f, 0f, 0f));
			Color newColor = new Color(0.95f, 
			                           0.95f,
			                           0.95f,
			                           1f);
			renderer.material.SetColor ("_Color", newColor);
			transform.localScale = new Vector3(0.01f, transform.localScale.y, transform.localScale.z);
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (isMainRay) {
			HandleMainRay();
			return;
		}
		if (GetComponentInParent<Converter> ().life < 20) {
			transform.rotation = new Quaternion(270, 0, 0, transform.rotation.w);
			return;
		}
		life--;
		if (life <= 0) {
			Color newColor = new Color (renderer.material.color.r, 
			                            renderer.material.color.g,
			                            renderer.material.color.b,
			                            0f);
			renderer.material.SetColor ("_Color", newColor);
			transform.RotateAround (pivot, Vector3.back, Random.Range (0, 180) - 90);
			life = Random.Range (40, 180);
			startLife = life;
			lifeThirds = life / 3;
			transparencySpeed = (float)transparencyMax / (float)lifeThirds;
			//set up a rotation speed that will land it squarely on the bottom
			float distanceFromBottom;
			if(transform.rotation.eulerAngles.x > 270f)
				distanceFromBottom = 90f + (360f - transform.rotation.eulerAngles.x);
			else
				distanceFromBottom = 90f - transform.rotation.eulerAngles.x;
			rotationSpeed = distanceFromBottom/ startLife;
			//rotationSpeed = (transform.rotation.eulerAngles.x) / startLife;
		} else if (life > startLife - lifeThirds) {
			Color newColor = new Color (renderer.material.color.r, 
			                            renderer.material.color.g,
			                            renderer.material.color.b,
			                            renderer.material.color.a + transparencySpeed);
			renderer.material.SetColor ("_Color", newColor);
			//need to make sure it's rotating towards the bottom
			if( life == startLife - 1){
				float oldAngle = transform.rotation.eulerAngles.x;
				transform.RotateAround (pivot, Vector3.back, rotationSpeed);
				if(oldAngle - transform.rotation.eulerAngles.x > 0)
				{
					rotationSpeed *= -1;
				}
			}
			else
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

	void HandleMainRay(){
		life--;
		if (life > 80) {
			transform.localScale = transform.localScale = new Vector3 (transform.localScale.x + 0.0025f, 
			                                                          transform.localScale.y, transform.localScale.z);
		} else if (life <= 20 && life > 0) {
			transform.localScale = transform.localScale = new Vector3 (transform.localScale.x - 0.018f, 
			                                                           transform.localScale.y, transform.localScale.z);
		}

	}
}
