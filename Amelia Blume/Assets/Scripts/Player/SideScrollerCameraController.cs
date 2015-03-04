using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SideScrollerCameraController : MonoBehaviour {

	public float dampTime = 0.2f;
	private Vector3 velocity = Vector3.zero;
	public Transform target;

	public List<Transform> paraLayers;

	//we'll put empty's limiting the camera in places where we want the camera to stop
	public bool canPanLeft;
	public bool canPanRight;
	public bool canPanUp;
	public bool canPanDown;

	float minPanSpeed = 6f;
	float panSpeedx;
	float panSpeedy;

	public float leftThreshold = 0.36f;
	public float rightThreshold = 0.64f;
	public float upThreshold = 0.8f;
	public float downThreshold = 0.1f;
	

	void Start()
	{
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		canPanUp = true;
		canPanRight = true;
		canPanLeft = true;
		canPanDown = true;

		IEnumerable<GameObject> tempLayers = GameObject.FindGameObjectsWithTag ("Parallax");
		GameObject[] tempList = GameObject.FindGameObjectsWithTag ("Parallax");
		int temp = tempList.Length;
		tempLayers = tempLayers.OrderBy(parallax => parallax.transform.position.z);
		paraLayers = new List<Transform>();
		for (int i = 0; i < temp; i++) {
			paraLayers.Add (tempLayers.ElementAt(i).transform);
		}

	}

	// Update is called once per frame
	void Update () 
	{
		panSpeedx = Mathf.Abs(target.GetComponent<CharacterController> ().velocity.x)+1f;
		if (panSpeedx < minPanSpeed)
			panSpeedx = minPanSpeed;

		panSpeedy = Mathf.Abs(target.GetComponent<CharacterController> ().velocity.y)+1f;
		if (panSpeedy < minPanSpeed)
			panSpeedy = minPanSpeed;

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
		transform.position = new Vector3 (Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime, panSpeedx).x,
		                                 Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime, panSpeedy).y,
		                                 transform.position.z);
		HandleParallax (panSpeedx, panSpeedy, delta);

		
	}

	void HandleParallax (float panSpeedX, float panSpeedY, Vector3 difference)
	{
		float dif = 1f / (float)(paraLayers.Count );
		Vector3 newDest;
		for (int i = 0; i < paraLayers.Count; i++) {
			newDest = paraLayers[i].position + (new Vector3( difference.x * (1f-(dif * (float)((paraLayers.Count + 1) - (i+1)))), 
			                                                difference.y * (1f-(dif * (float)((paraLayers.Count + 1) - (i + 1))))));
			paraLayers[i].transform.position = new Vector3 (Vector3.SmoothDamp (paraLayers[i].position, newDest, ref velocity, dampTime, panSpeedx).x,
			                                                Vector3.SmoothDamp (paraLayers[i].position, newDest, ref velocity, dampTime, panSpeedy).y,
			                                                paraLayers[i].position.z);
			
		}
	}

	public void MoveToPlayer(float xCoord, float yCoord )
	{
		//GameObject amelia = GameObject.Find ("Player");
		transform.position = new Vector3(xCoord, yCoord, transform.position.z);
	}
}