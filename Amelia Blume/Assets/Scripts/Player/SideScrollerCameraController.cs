using UnityEngine;
using System.Collections;

public class SideScrollerCameraController : MonoBehaviour {
	
	public float dampTime = 0.2f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

	//we'll put empty's limiting the camera in places where we want the camera to stop
	public bool canPanLeft;
	public bool canPanRight;
	public bool canPanUp;
	public bool canPanDown;

	float panSpeed;

	float leftThreshold;
	float rightThreshold;
	float upThreshold;
	float downThreshold;

	float leftThreshLoc;
	float rightThreshLoc;

	float deltaHorizontalW;
	float deltaHorizontalV;

	void Start()
	{
		canPanUp = true;
		canPanRight = true;
		canPanLeft = true;
		canPanDown = true;

		//set threshholds, change functions to get different results
		//do not hardcode values to prevent it messing up at different resolutions
		leftThreshold = (Screen.width / 8f) * 3f;
		leftThreshLoc = GetComponent<Camera>().ViewportToWorldPoint (new Vector3(leftThreshold, 0f, 0f)).x;
		rightThreshold = (Screen.width / 8f) * 5f ;
		rightThreshLoc = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(rightThreshold, 0f, 0f)).x;
		deltaHorizontalW = Mathf.Abs( leftThreshLoc - transform.position.x);
		deltaHorizontalV = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(deltaHorizontalW, 0f, 0f)).x;
	}

	// Update is called once per frame
	void Update () 
	{

		//update threshold locations
		leftThreshLoc = GetComponent<Camera>().ViewportToWorldPoint (new Vector3(leftThreshold, 0f, 0f)).x;
		rightThreshLoc = GetComponent<Camera>().ViewportToWorldPoint (new Vector3(rightThreshold, 0f, 0f)).x;

		//player's position in viewport
		Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
		if( target.GetComponent<PlayerController>().running || target.GetComponent<PlayerController>().alwaysRun)
		{
			panSpeed = target.GetComponent<PlayerMotor>().movement.runSpeed;
		}
		else
		{
			panSpeed = target.GetComponent<PlayerMotor>().movement.walkSpeed;
		}

		//to the left
		if (point.x < 0.36f)
		{
			Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.64f, point.y, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, panSpeed);
		}
		else if (point.x > 0.64f)
		{
			Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.36f, point.y, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, panSpeed);
		}

		/*if (target)
		{
			Vector3 point = camera.WorldToViewportPoint(target.position);
			Vector3 delta = target.position - camera.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, point.z)); //(new Vector3(0.5, 0.5, point.z));
			Vector3 destination = transform.position + delta;
			transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime);
		}*/
		
	}
}