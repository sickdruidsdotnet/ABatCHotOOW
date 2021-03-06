﻿using UnityEngine;
using System.Collections;

public class PanLimiter : MonoBehaviour {

	public bool limitLeft;
	public bool limitRight;
	public bool limitUp;
	public bool limitDown;

	//total Limiters will follow the play and are meant to define the absolute sides of a level
	public bool totalVerticalLimiter;
	public bool totalHorizontalLimiter;

	//forces the camera to pan until this is just barely in sight. Prevent zooming from looking at 
	//normally out of sight geometry
	public bool forcePan;

	bool isVisible;

	GameObject mainCameraObject;
	Camera mainCamera;
	SideScrollerCameraController cameraScript;


	// Use this for initialization
	void Start () {
		//scale down to make invisible to the player when running game
		transform.localScale = new Vector3 (0.0000002f, 0.0000002f, 0.0000002f);
		mainCameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		if (mainCameraObject == null) {
			Debug.LogError ("No Main Camera in scene");
		} else {
			mainCamera = mainCameraObject.GetComponent<Camera>();
			cameraScript = mainCameraObject.GetComponent<SideScrollerCameraController>();
		}
	}

	void FixedUpdate()
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

		//handling force pan movement stuff
		if (isVisible && forcePan) {
			checkForcePan();
		}

		//ignoring the editor camera when unrendering is more difficult than it should be
		Vector2 camPosition = mainCameraObject.camera.WorldToViewportPoint (transform.position);
		if (camPosition.x < 0 || camPosition.x > 1 || camPosition.y < 0 || camPosition.y > 1) {
			isVisible = false;
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

	//returns true it if moved the camera
	public bool checkForcePan()
	{
		if (!forcePan)
			return false;

		bool moved = false;
		Vector3 viewPoint  = mainCamera.WorldToViewportPoint(transform.position);
		if (limitLeft) {
			while(viewPoint.x > 0.000001f && viewPoint.x <= 1)
			{
				mainCameraObject.transform.Translate(new Vector3(0.001f, 0f, 0f));
				viewPoint  = mainCamera.WorldToViewportPoint(transform.position);
				moved = true;
			}
		}			
		
		if (limitRight) {
			while(viewPoint.x < 0.999999f && viewPoint.x >= 0f)
			{
				mainCameraObject.transform.Translate(new Vector3(-0.001f, 0f, 0f));
				viewPoint  = mainCamera.WorldToViewportPoint(transform.position);
				moved = true;
			}
		}
		
		if (limitUp) {
			while(viewPoint.y < 0.999999f && viewPoint.y >= 0f)
			{
				mainCameraObject.transform.Translate(new Vector3(0f, -0.001f, 0f));
				viewPoint  = mainCamera.WorldToViewportPoint(transform.position);
				moved = true;
			}
		}
		
		if (limitDown) {
			//reposition camera until it is at 0.01f
			while(viewPoint.y > 0.000001f && viewPoint.y <= 1f)
			{
				mainCameraObject.transform.Translate(new Vector3(0f, 0.001f, 0f));
				viewPoint  = mainCamera.WorldToViewportPoint(transform.position);
				moved = true;
			}
		}
		return moved;
	}

	void OnWillRenderObject()
	{
		isVisible = true;
		if (Camera.current.tag == "MainCamera") {
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
}
