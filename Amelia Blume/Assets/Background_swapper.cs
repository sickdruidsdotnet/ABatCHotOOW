using UnityEngine;
using System.Collections;

public class Background_swapper : MonoBehaviour {

	public Color leftColor;
	public Color rightColor;

	public bool leftActive = true;

	Camera mainCamera;
	public Color currentColor;

	public float transitionTime = 1;
	float startTime;
	public float deltaR, deltaG, deltaB, deltaA;
	bool transitioning = false;
	public float progress;
	Color oldColor;
	Color nextColor;

	void Start () {
		GameObject MainCameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		if (MainCameraObject == null) {
			Debug.Log ("Cannot find maincamera, deleting color changer");
			Destroy (gameObject);
		} else {
			mainCamera = MainCameraObject.GetComponent<Camera>();
			currentColor = mainCamera.backgroundColor;
		}

	}

	void FixedUpdate(){
		//code for smoothly transitioning between colors
		if (transitioning) {
			//check if it's hit its max time
			if(Time.time - startTime > transitionTime){
				transitioning = false;
				mainCamera.backgroundColor = nextColor;
				progress = 0;
			} else {
				progress = (Time.time - startTime) / transitionTime;
				Color temp = new Color (oldColor.r + (deltaR * progress),
				                        oldColor.g + (deltaG * progress),
				                        oldColor.b + (deltaB * progress),
				                        oldColor.a + (deltaA * progress));
				mainCamera.backgroundColor = temp;
			}
		}
		currentColor = mainCamera.backgroundColor;
	}
	
	void OnTriggerExit(Collider other){
		if (other.tag == "Player") {
			//check if on left or right
			if (other.transform.position.x - transform.position.x < 0)
			{
				//on the left
				ChangeCameraColor(leftColor);
			} else {
				//on the right
				ChangeCameraColor(rightColor);
			}
		}
	}

	void ChangeCameraColor(Color newColor)
	{
		transitioning = true;
		startTime = Time.time;
		deltaR = newColor.r - currentColor.r;
		deltaG = newColor.g - currentColor.g;
		deltaB = newColor.b - currentColor.b;
		deltaA = newColor.a - currentColor.a;
		oldColor = currentColor;
		nextColor = newColor;
		progress = 0;
	}
}
