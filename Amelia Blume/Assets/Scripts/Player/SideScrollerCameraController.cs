using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SideScrollerCameraController : MonoBehaviour {
	
	private Vector3 velocity = Vector3.zero;
	public Transform target;
	bool lastFaceDirection;
	bool recentlyRotated1;
	bool recentlyRotated2;

	public List<Transform> paraLayers;

	//we'll put empty's limiting the camera in places where we want the camera to stop
	public bool canPanLeft;
	public bool canPanRight;
	public bool canPanUp;
	public bool canPanDown;
	bool panLeft;
	bool panRight;

	public float panSpeed = 0.4f;
	bool panning;
	Vector3 panTo;
	

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
		lastFaceDirection = target.GetComponent<Player> ().isFacingRight;
		recentlyRotated1 = false;
		recentlyRotated2 = false;
		panning = true;
		panTo = transform.position;
	}

	// Update is called once per frame
	void Update () 
	{
		//get player's position in viewport
		Vector3 point = GetComponent<Camera>().WorldToViewportPoint(target.position);

		//new camera movement. Adjust dynamically depending on Player's direction
		//geet the world distance of 0.1 to properly adjust the camera
		float xDistance = GetComponent<Camera> ().ViewportToWorldPoint (new Vector2 (0.4f, 0.5f)).x -
			GetComponent<Camera> ().ViewportToWorldPoint (new Vector2 (0.5f, 0.5f)).x;
		//recentlyroated 1 &2 allow for some player movement leeway before adjusting the camera
		if(recentlyRotated1)
			recentlyRotated2 = (lastFaceDirection != target.GetComponent<Player> ().isFacingRight);
		else
			recentlyRotated1 = (lastFaceDirection != target.GetComponent<Player> ().isFacingRight);
		if (recentlyRotated2) {
			recentlyRotated1 = false;
			recentlyRotated2 = false;
		}
		//if the player is facing right, give 3/5ths the screen of lead space to the right
		if (target.GetComponent<Player> ().isFacingRight) {
			if(point.x >= 0.4f && canPanRight && !recentlyRotated1)
			{
				panTo = new Vector3(target.transform.position.x - xDistance,
				                    transform.position.y, transform.position.z);
				panRight = true;
				panLeft = false;
			}
			else if(recentlyRotated1 && point.x >= 0.8f && canPanRight )
			{
				//player has reached the threshold of leeway for space after turning around from left
				recentlyRotated1 = false;
				panTo = new Vector3(target.transform.position.x - xDistance,
				                    transform.position.y, transform.position.z);
				panRight = true;
				panLeft = true;
			}
			else
			{
				panRight = false;
			}
		} else { //if the player is facing left, give 3/5ths the screen of lead space to the left
			if(point.x <= 0.6f && canPanLeft && !recentlyRotated1)
			{
				panTo = new Vector3(target.transform.position.x + xDistance,
				                                 transform.position.y, transform.position.z);
				panLeft = true;
				panRight = false;
			}
			else if(recentlyRotated1 && point.x <= 0.2f && canPanLeft )
			{
				//player has reached the threshold of leeway for space after turning around from right
				recentlyRotated1 = false;
				panTo = new Vector3(target.transform.position.x + xDistance,
				                    transform.position.y, transform.position.z);
				panLeft = true;
				panRight = false;
			}
			else{
				panLeft = false;
			}
		}

		/*transform.position = new Vector3 (Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime, panSpeedx).x,
		                                 Vector3.SmoothDamp (transform.position, destination, ref velocity, dampTime, panSpeedy).y,
		                                 transform.position.z);*/
		//HandleParallax (panSpeedx, panSpeedy, delta);
		if (panRight) {
			if(Mathf.Abs (panTo.x - transform.position.x) <= panSpeed)
			{
				transform.position = new Vector3(panTo.x, transform.position.y, transform.position.z);
				panRight = false;
			}
			else
			{
				transform.position = new Vector3(transform.position.x + panSpeed, 
				                                 transform.position.y, transform.position.z);
			}
		} else if (panLeft) {
			if(Mathf.Abs (panTo.x - transform.position.x)<= panSpeed)
			{
				transform.position = new Vector3(panTo.x, transform.position.y, transform.position.z);
				panLeft = false;
			}
			else
			{
				transform.position = new Vector3(transform.position.x - panSpeed, 
				                                 transform.position.y, transform.position.z);
			}
		}


		//get the face direction to properly check for change next frame
		lastFaceDirection = target.GetComponent<Player> ().isFacingRight;
		
	}

	void HandleParallax (float panSpeedX, float panSpeedY, Vector3 difference)
	{
		float dif = 1f / (float)(paraLayers.Count );
		Vector3 newDest;
		for (int i = 0; i < paraLayers.Count; i++) {
			newDest = paraLayers[i].position + (new Vector3( difference.x * (1f-(dif * (float)((paraLayers.Count + 1) - (i+1)))), 
			                                                difference.y * (1f-(dif * (float)((paraLayers.Count + 1) - (i + 1))))));
			paraLayers[i].transform.position = new Vector3 (Vector3.SmoothDamp (paraLayers[i].position, newDest, ref velocity, Mathf.Infinity, panSpeedX).x,
			                                                Vector3.SmoothDamp (paraLayers[i].position, newDest, ref velocity, Mathf.Infinity, panSpeedY).y,
			                                                paraLayers[i].position.z);
			
		}
	}

	public void MoveToPlayer(float xCoord, float yCoord )
	{
		//GameObject amelia = GameObject.Find ("Player");
		transform.position = new Vector3(xCoord, yCoord, transform.position.z);
	}
}