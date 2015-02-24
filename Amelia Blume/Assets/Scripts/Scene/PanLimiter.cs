using UnityEngine;
using System.Collections;

public class PanLimiter : MonoBehaviour {

	public bool limitLeft;
	public bool limitRight;
	public bool limitUp;
	public bool limitDown;

	GameObject mainCameraObject;
	//Camera mainCamera;
	SideScrollerCameraController cameraScript;

	// Use this for initialization
	void Start () {
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
