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

	public float leftThreshold = 0.36f;
	public float rightThreshold = 0.64f;
	public float upThreshold = 0.8f;
	public float downThreshold = 0.1f;
	

	void Start()
	{
		canPanUp = true;
		canPanRight = true;
		canPanLeft = true;
		canPanDown = true;

	}

	// Update is called once per frame
	void Update () 
	{

		if( target.GetComponent<PlayerController>().running || target.GetComponent<PlayerController>().alwaysRun)
		{
			panSpeed = target.GetComponent<PlayerMotor>().movement.runSpeed;
		}
		else
		{
			panSpeed = target.GetComponent<PlayerMotor>().movement.walkSpeed;
		}

		//get player's position in viewport
		Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);
		float horizontalPan = point.x;
		float verticalPan = point.y;

		//to the left
		if (canPanLeft && point.x < leftThreshold)
		{
			horizontalPan = rightThreshold;
		}
		else if (canPanRight && point.x > rightThreshold) //to the right
		{
			horizontalPan = leftThreshold;
		}

		//go up?
		if (canPanUp && point.y > upThreshold)
		{
			verticalPan = downThreshold;
		}
		else if(canPanDown && point.y < downThreshold)
		{
			verticalPan = upThreshold;
		}

		Vector3 delta = target.position - GetComponent<Camera>().ViewportToWorldPoint(new Vector3(horizontalPan, verticalPan, point.z));
		Vector3 destination = transform.position + delta;
		transform.position = Vector3.SmoothDamp(transform.position, destination, ref velocity, dampTime, panSpeed);

		
	}

	public void MoveToPlayer(float xCoord )
	{
		//GameObject amelia = GameObject.Find ("Player");
		transform.position = new Vector3(xCoord, transform.position.y, transform.position.z);
	}
}