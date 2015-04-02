using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SideScrollerCameraController : MonoBehaviour {
	public Transform target;
	Vector3 prevTargetPos;
	Vector3 prevPos;
	bool lastFaceDirection;
	bool recentlyRotated1;
	bool recentlyRotated2;

	public List<Transform> paraLayers;
	public List<Transform> frontLayers;
	float layerDifference;

	//this camera for easy access
	Camera thisCam;

	//we'll put empty's limiting the camera in places where we want the camera to stop
	public bool canPanLeft;
	public bool canPanRight;
	public bool canPanUp;
	public bool canPanDown;
	bool panLeft;
	bool panRight;
	bool panUp;
	bool panDown;

	bool zooming;
	float zoomRate = 4f;
	float startTime;
	float startSize;
	float endSize;
	float zoomLength;

	public float panSpeedX = 0.4f;
	public float panSpeedY = 0.1f;
	Vector3 panTo;

	//tracking stuff
	List<GameObject> trackables;
	List<bool> tracking;
	public float maxSize = 7;
	public float minSize = 4;

	public float maxScreenDist;
	public float minScreenDist;

	void Start()
	{
		zooming = false;
		thisCam = gameObject.GetComponent<Camera> ();
		target = GameObject.FindGameObjectWithTag ("Player").transform;
		canPanUp = true;
		canPanRight = true;
		canPanLeft = true;
		canPanDown = true;

		IEnumerable<GameObject> tempLayers = GameObject.FindGameObjectsWithTag ("Parallax");
		GameObject[] tempList = GameObject.FindGameObjectsWithTag ("Parallax");
		int temp = tempList.Length;
		tempLayers = tempLayers.OrderBy(parallax => parallax.transform.position.z);
		frontLayers = new List<Transform> ();
		paraLayers = new List<Transform> ();
		for (int i = 0; i < temp; i++) {
			if(tempLayers.ElementAt (i).transform.position.z < target.position.z)
				frontLayers.Add (tempLayers.ElementAt(i).transform);
			else
				paraLayers.Add (tempLayers.ElementAt(i).transform);
		}
		layerDifference = 0.5f / (float)(tempLayers.Count());

		lastFaceDirection = target.GetComponent<Player> ().isFacingRight;
		recentlyRotated1 = false;
		recentlyRotated2 = false;
		panTo = transform.position;
		prevTargetPos = target.position;

		//get all animals that the camera should know about
		trackables = new List<GameObject> ();
		tracking = new List<bool>();
		GameObject[] tempTrackables = GameObject.FindGameObjectsWithTag ("Animal");
		for (int i = 0; i < tempTrackables.Count(); i++) {
			trackables.Add (tempTrackables[i]);
			tracking.Add (false);
		}
		//tracking will store true in that animal's index if it is currently being tracked

		//calculate distances from center to threshold based on camera sizes
		float startSize = gameObject.GetComponent<Camera>().orthographicSize;
		gameObject.GetComponent<Camera> ().orthographicSize = maxSize;
		Vector3 point1 = GetComponent<Camera> ().ViewportToWorldPoint (new Vector3(0.5f, 0f, 0f));
		Vector3 point2 = GetComponent<Camera> ().ViewportToWorldPoint (new Vector3(0.9f, 0f, 0f));
		maxScreenDist = point2.x - point1.x;
		gameObject.GetComponent<Camera> ().orthographicSize = minSize;
		point1 = GetComponent<Camera> ().ViewportToWorldPoint (new Vector3(0.5f, 0f, 0f));
		point2 = GetComponent<Camera> ().ViewportToWorldPoint (new Vector3(0.9f, 0f, 0f));
		minScreenDist = point2.x - point1.x;

		gameObject.GetComponent<Camera> ().orthographicSize = startSize;

		prevPos = transform.position;
	}

	// Update is called once per frame
	void FixedUpdate () 
	{
		//handle parallax changes that may have happened outside of this script immediately
		if (prevPos != transform.position) {
			HandleParallax (new Vector3(transform.position.x - prevPos.x, transform.position.y - prevPos.y, 0f));
		}

		trackTargets ();
		if (zooming) {
			float newSize = Mathf.Lerp(startSize, endSize, ((Time.time - startTime) * zoomRate) / zoomLength );
			thisCam.orthographicSize = newSize;
			if(thisCam.orthographicSize == endSize)
			{
				zooming = false;
			}
		}

		//get player's position in viewport
		Vector3 point = GetComponent<Camera> ().WorldToViewportPoint (target.position);

		//new camera movement. Adjust dynamically depending on Player's direction
		//geet the world distance of 0.1 to properly adjust the camera
		float xDistance = GetComponent<Camera> ().ViewportToWorldPoint (new Vector2 (0.4f, 0.5f)).x -
			GetComponent<Camera> ().ViewportToWorldPoint (new Vector2 (0.5f, 0.5f)).x;

		//horizontal movement is entirely reliant on whether the player has recently turned around or not;
		//recentlyroated 1 & 2 allow for some player movement leeway before adjusting the camera
		if (recentlyRotated1)
			recentlyRotated2 = (lastFaceDirection != target.GetComponent<Player> ().isFacingRight);
		else
			recentlyRotated1 = (lastFaceDirection != target.GetComponent<Player> ().isFacingRight);
		if (recentlyRotated2) {
			recentlyRotated1 = false;
			recentlyRotated2 = false;
		}

		//if the player is facing right, give 3/5ths the screen of lead space to the right
		if (target.GetComponent<Player> ().isFacingRight) {
			if (point.x >= 0.4f && canPanRight && !recentlyRotated1) {
				panTo = new Vector3 (target.transform.position.x - xDistance,
				                    panTo.y, transform.position.z);
				panRight = true;
				panLeft = false;
			} else if (recentlyRotated1 && point.x >= 0.8f && canPanRight) {
				//player has reached the threshold of leeway for space after turning around from left
				recentlyRotated1 = false;
				panTo = new Vector3 (target.transform.position.x - xDistance,
				                    panTo.y, transform.position.z);
				panRight = true;
				panLeft = true;
			} else {
				panRight = false;
			}
		} else { //if the player is facing left, give 3/5ths the screen of lead space to the left
			if (point.x <= 0.6f && canPanLeft && !recentlyRotated1) {
				panTo = new Vector3 (target.transform.position.x + xDistance,
				                     panTo.y, transform.position.z);
				panLeft = true;
				panRight = false;
			} else if (recentlyRotated1 && point.x <= 0.2f && canPanLeft) {
				//player has reached the threshold of leeway for space after turning around from right
				recentlyRotated1 = false;
				panTo = new Vector3 (target.transform.position.x + xDistance,
				                    panTo.y, transform.position.z);
				panLeft = true;
				panRight = false;
			} else {
				panLeft = false;
			}
		}

		//vertical movement now; unlike hori, mostly dependent on user's vertical position
		// we do want to give the player a view of where they're falling though.

		float yDistance = GetComponent<Camera> ().ViewportToWorldPoint (new Vector2 (0.4f, 0.6f)).y -
			GetComponent<Camera> ().ViewportToWorldPoint (new Vector2 (0.5f, 0.5f)).y;

		//panning up
		if (point.y >= 0.8f && canPanUp) {
			panTo = new Vector3 (panTo.x, transform.position.y + (4f * yDistance), transform.position.z);
			panUp = true;
			panDown = false;
		} else if (point.y <= 0.1f && canPanDown) {//panning down
			panTo = new Vector3 (panTo.x, transform.position.y - (4f * yDistance), transform.position.z);
			panUp = false;
			panDown = true;
		}

		//let's save the current position for later use;
		Vector3 oldPos = transform.position;
		if (panRight) {
			if(Mathf.Abs (panTo.x - transform.position.x) <= panSpeedX)
			{
				transform.position = new Vector3(panTo.x, transform.position.y, transform.position.z);
				panRight = false;
			}
			else
			{
				transform.position = new Vector3(transform.position.x + panSpeedX, 
				                                 transform.position.y, transform.position.z);
			}
		} else if (panLeft) {
			if(Mathf.Abs (panTo.x - transform.position.x)<= panSpeedX)
			{
				transform.position = new Vector3(panTo.x, transform.position.y, transform.position.z);
				panLeft = false;
			}
			else
			{
				transform.position = new Vector3(transform.position.x - panSpeedX, 
				                                 transform.position.y, transform.position.z);
			}
		}
		if (Mathf.Abs (prevTargetPos.y - target.position.y) >= 0.2f) {

			panSpeedY = Mathf.Abs (prevTargetPos.y - target.position.y);
		} else
			panSpeedY = 0.2f;

		if (panUp && canPanUp) {
			if(Mathf.Abs (panTo.y - transform.position.y) <= panSpeedY)
			{
				transform.position = new Vector3(transform.position.x, panTo.y, transform.position.z);
				panUp = false;
			}
			else
			{
				transform.position = new Vector3(transform.position.x, 
				                                 transform.position.y + panSpeedY, transform.position.z);
			}
		} else if (panDown && canPanDown) {
			if(Mathf.Abs (panTo.y - transform.position.y) <= panSpeedY)
			{
				transform.position = new Vector3(transform.position.x, panTo.y, transform.position.z);
				panDown = false;
			}
			else
			{
				transform.position = new Vector3(transform.position.x, 
				                                 transform.position.y - panSpeedY, transform.position.z);
			}
		}

		//let's get how much has changed between frames and handle that parallax
		Vector3 delta = new Vector3 ( transform.position.x - oldPos.x, transform.position.y - oldPos.y, 0f);
		HandleParallax (delta);
		//get the face direction to properly check for change next frame
		lastFaceDirection = target.GetComponent<Player> ().isFacingRight;
		prevTargetPos = target.position;
		prevPos = transform.position;
	}

	void trackTargets()
	{
		int index = 0;
		bool isTracking = false;
		List<int> removeIndices = new List<int>();
		foreach(GameObject trackable in trackables) {
			//for starters, let's make sure it should be in this list
			if(trackable.tag == "Animal" && trackable.GetComponent<Animal>().isInfected == false)
			{
				//remove uninfected animals from the list, they're no longer important enough
				removeIndices.Add (trackables.IndexOf(trackable));
			}
			else {
				Vector3 centerCamPoint = GetComponent<Camera>().ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
				float distFromCenter = Mathf.Abs(centerCamPoint.x - trackable.transform.position.x);
				index = trackables.IndexOf(trackable);
				//let's check if it should be being tracked
				if(distFromCenter <= maxScreenDist)
				{
					if(tracking[index])
					{
						isTracking = true;
						//already being tracked, put code handling that in here
					}
					else{
						Debug.Log ("Begin Tracking");
						tracking[index] = true;
						startSize = thisCam.orthographicSize;
						endSize = maxSize;
						zoomLength = Mathf.Abs(startSize - endSize);
						zooming = true;
						startTime = Time.time;
					}
					//it should be tracked, but is it already being tracked?

				} else {
					//it shouldn't be tracked, let's do some checking
					if(tracking[index])
					{
						Debug.Log ("End Tracking");
						//being tracked, that needs to change and the camera needs to resize accordingly
						tracking[index] = false;
						//check to make sure there's not another thing being tracked on screen
						foreach(bool isBeingTracked in tracking)
						{
							if(isBeingTracked)
							{
								isTracking = true;
								break;
							}
						}

						if(!isTracking)
						{
							startSize = thisCam.orthographicSize;
							endSize = minSize;
							zoomLength = Mathf.Abs(startSize - endSize);
							zooming = true;
							startTime = Time.time;
						}
					}
				}
			}
		}

		//remove all gameobject that were marked no longer necessary to track
		for (int i = removeIndices.Count - 1; i >= 0; i--) {
			trackables.RemoveAt(removeIndices[i]);
			tracking.RemoveAt(removeIndices[i]);
			Debug.Log ("Removing at " + i);
		}

		foreach(bool isBeingTracked in tracking)
		{
			if(isBeingTracked)
			{
				isTracking = true;
				break;
			}
		}

		if(!isTracking && thisCam.orthographicSize != minSize && !zooming)
		{
			startSize = thisCam.orthographicSize;
			endSize = minSize;
			zoomLength = Mathf.Abs(startSize - endSize);
			zooming = true;
			startTime = Time.time;
		}

	}

	void HandleParallax (Vector3 difference)
	{
		Vector3 newDest;
		//backLayers
		for (int i = 0; i < paraLayers.Count; i++) {
			newDest = new Vector3( difference.x * (1f-(layerDifference* (float)((paraLayers.Count + 1) - (i+1)))), 
			                      difference.y * (1f-(layerDifference * (float)((paraLayers.Count + 1) - (i + 1)))));
			paraLayers[i].transform.position = new Vector3 (paraLayers[i].transform.position.x + newDest.x,
			                                                paraLayers[i].transform.position.y + newDest.y,
			                                                paraLayers[i].position.z);
		}
		//frontLayers
		for (int i = 0; i < frontLayers.Count; i++) {
			newDest = new Vector3( difference.x * -1*(1f-(layerDifference* (float)((frontLayers.Count + 1) + (i+1)))), 
			                      difference.y * -1*(1f-(layerDifference * (float)((frontLayers.Count + 1) + (i + 1)))));
			frontLayers[i].transform.position = new Vector3 (frontLayers[i].transform.position.x + newDest.x,
			                                                frontLayers[i].transform.position.y + newDest.y,
			                                                frontLayers[i].position.z);
		}
	}

	public void MoveToPlayer(float xCoord, float yCoord )
	{
		//GameObject amelia = GameObject.Find ("Player");
		Vector3 oldPos = transform.position;
		transform.position = new Vector3(xCoord, yCoord, transform.position.z);
		Vector3 delta = new Vector3 (xCoord - oldPos.x, yCoord - oldPos.y, transform.position.z);
		HandleParallax (delta);
	}
}