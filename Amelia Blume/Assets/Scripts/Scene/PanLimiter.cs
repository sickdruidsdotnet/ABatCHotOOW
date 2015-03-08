using UnityEngine;
using System.Collections;

public class PanLimiter : MonoBehaviour {

	public bool limitLeft;
	public bool limitRight;
	public bool limitUp;
	public bool limitDown;

	//total Limiters will follow the play and are meant to define the absolute sides of a level
	public bool totalVerticalLimiter;
	public bool totalHorizontalLimiter;

	GameObject mainCameraObject;
	//Camera mainCamera;
	SideScrollerCameraController cameraScript;

	// Use this for initialization
	void Start () {
		//scale down to make invisible to the player when running game
		transform.localScale = new Vector3 (0.0000002f, 0.0000002f, 0.0000002f);
		mainCameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCameraObject == null) {
			Debug.LogError ("No Main Camera in scene");
		} else {
			//mainCamera = mainCameraObject.GetComponent<Camera>();
			cameraScript = mainCameraObject.GetComponent<SideScrollerCameraController>();
		}
	}
	void Update()
	{
		if (totalVerticalLimiter) {
			transform.position = new Vector3(cameraScript.transform.position.x,
			                                 transform.position.y,
			                                 transform.position.z);
		}
		if(totalHorizontalLimiter) {
			transform.position = new Vector3(transform.position.x,
			                                 cameraScript.transform.position.y,
			                                 transform.position.z);
		}
		if (renderer.isVisible) {
			if (limitLeft) {
				cameraScript.canPanLeft = false;
			}
		
			if (limitRight) {
				cameraScript.canPanRight = false;
			}
		
			if (limitUp) {
				cameraScript.canPanUp = false;
			}
		
			if (limitDown) {
				cameraScript.canPanDown = false;
			}
		}
	}


	void OnBecameVisible()
	{
		if (Camera.current.tag != "MainCamera") 
			return;

		if (limitLeft) {
			cameraScript.canPanLeft = false;
		}

		if (limitRight) {
			cameraScript.canPanRight = false;
		}

		if (limitUp) {
			cameraScript.canPanUp = false;
		}

		if (limitDown) {
			cameraScript.canPanDown = false;
		}
	}

	void OnBecameInvisible()
	{
		/*if (Camera.current.tag != "MainCamera") 
			return;*/

		if (limitLeft) {
			cameraScript.canPanLeft = true;
		}
		
		if (limitRight) {
			cameraScript.canPanRight = true;
		}
		
		if (limitUp) {
			cameraScript.canPanUp = true;
		}
		
		if (limitDown) {
			cameraScript.canPanDown = true;
		}
	}
}
