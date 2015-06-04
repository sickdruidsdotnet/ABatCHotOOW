using UnityEngine;
using System.Collections;

public class ConverterRay : MonoBehaviour {
	float transparencyMax = 0.5f;
	float rotationSpeed;
	Vector3 pivot;
	public bool isMainRay;
	public float startTime;
	public float startLife;
	public float life = 1f;
	public float lifeThirds;
	public float transparencySpeed;
	public float currTransparency;
	
	// Use this for initialization
	void Start () {
		startTime = Time.time;
		if (!isMainRay) {
			Color newColor = new Color(0.95f, 
			                           0.95f,
			                           0.95f,
			                           0f);
			renderer.material.SetColor ("_Color", newColor);
			pivot = transform.parent.transform.position;
			life = Random.Range (0.66f, 3f);
			startLife = life;
			lifeThirds = life / 3;
			transform.RotateAround (pivot, Vector3.back, Random.Range (0, 180) - 90);
			//set up a rotation speed that will land it squarely on the bottom
			float distanceFromBottom;
			if(transform.rotation.eulerAngles.x > 270f)
				distanceFromBottom = 90f + (360f - transform.rotation.eulerAngles.x);
			else
				distanceFromBottom = 90f - transform.rotation.eulerAngles.x;
			rotationSpeed = distanceFromBottom / (startLife * 60f);
			transparencySpeed = (float)transparencyMax / (lifeThirds * 60f);
		} else {
			startLife = 3.33333f;
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
	void FixedUpdate () {
		currTransparency = renderer.material.color.a;
		if (isMainRay) {
			HandleMainRay();
			return;
		}
		if (GetComponentInParent<Converter> ().conversionSuccess) {
			transform.rotation = new Quaternion(270, 0, 0, transform.rotation.w);
			return;
		}
		if (Time.time - startTime >= life) {
			Color newColor = new Color (renderer.material.color.r, 
			                            renderer.material.color.g,
			                            renderer.material.color.b,
			                            0f);
			renderer.material.SetColor ("_Color", newColor);
			transform.RotateAround (pivot, Vector3.back, Random.Range (0, 180) - 90);
			life = Random.Range (0.66f, 3f);
			startTime = Time.time;
			startLife = life;
			lifeThirds = life / 3;
			transparencySpeed = (float)transparencyMax / (lifeThirds * 60f);
			//set up a rotation speed that will land it squarely on the bottom
			float distanceFromBottom;
			if(transform.rotation.eulerAngles.x > 270f)
				distanceFromBottom = 90f + (360f - transform.rotation.eulerAngles.x);
			else
				distanceFromBottom = 90f - transform.rotation.eulerAngles.x;
			rotationSpeed = distanceFromBottom/ (startLife * 60f);
			//rotationSpeed = (transform.rotation.eulerAngles.x) / startLife;
		} else if (Time.time - startTime <= startLife - lifeThirds) {	//(life <= startLife - lifeThirds
			Color newColor = new Color (renderer.material.color.r, 
			                            renderer.material.color.g,
			                            renderer.material.color.b,
			                            renderer.material.color.a + transparencySpeed);
			renderer.material.SetColor ("_Color", newColor);
			//need to make sure it's rotating towards the bottom
			if (Time.time - startTime > 0.01f && Time.time - startTime < 0.03f){ //( life == startLife - 1){
				float oldAngle = transform.rotation.eulerAngles.x;
				transform.RotateAround (pivot, Vector3.back, rotationSpeed);
				if(oldAngle - transform.rotation.eulerAngles.x > 0)
				{
					rotationSpeed *= -1;
				}
			}
			else
				transform.RotateAround (pivot, Vector3.back, rotationSpeed);
		} else if (Time.time - startTime > startLife - (2 * lifeThirds) ) {//(life < startLife - (2 * lifeThirds) ) {
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
		if (Time.time - startTime < 2f) {
			transform.localScale = transform.localScale = new Vector3 (transform.localScale.x + 0.0025f, 
			                                                          transform.localScale.y, transform.localScale.z);
		} else if (Time.time - startTime >= 3) {
			transform.localScale = transform.localScale = new Vector3 (transform.localScale.x - 0.018f, 
			                                                           transform.localScale.y, transform.localScale.z);
		}

	}
}
