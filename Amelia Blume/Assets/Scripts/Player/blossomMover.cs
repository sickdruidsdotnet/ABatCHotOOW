using UnityEngine;
using System.Collections;

/*Parent this object to the player, then save it into an
 * array that will deal with dropping them at the proper times
 */

public class blossomMover : MonoBehaviour {
	
	public bool attached;
	public bool falling;
	
	bool canSwayLeft;
	bool canSwayRight;
	
	public int destroyTimer;
	
	float rotateRate = 300;
	float rotateX;
	float rotateY;
	float rotateZ;
	
	public float amplitude;
	public float frequency;
	
	// Use this for initialization
	void Start () {
		destroyTimer = 900;
		
		canSwayLeft = true;
		canSwayRight = true;
		
		if(transform.parent != null)
		{
			attached = true;
			falling = false;
		}
		else
			attached = false;
	}
	
	// Update is called once per frame
	void Update () {

		//raycasting instead of colliders, allows for more concise code
		if (!attached) {
			//check down
			//Debug.DrawRay(transform.position, -Vector3.up * 0.01f, Color.blue);
			RaycastHit[] downHits;
			downHits = Physics.RaycastAll (new Vector3(transform.position.x, transform.position.y+0.1f), -Vector3.up, 0.01f);
			foreach(RaycastHit hit in downHits)
			{
				if (hit.transform.tag!= "Blossom" && !hit.transform.collider.isTrigger) 
					falling = false;
			}
			if(downHits.Length ==0)
				falling = true;
			//check right
			if (Physics.Raycast (transform.position, Vector3.right, 0.02f)) {
				canSwayRight = false;
			} else {
				canSwayRight = true;
			}
			//check left
			if (Physics.Raycast (transform.position, -Vector3.right, 0.02f)) {
				canSwayLeft = false;
			} else {
				canSwayLeft = true;
			}
		}

		//handling movement after falling off
		if (!attached) {
			//countdown to destruction
			destroyTimer--;
			//falling movement
			if(falling){
				Vector3 nextPosition = transform.position + amplitude * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) 
				                                                         - Mathf.Sin (2 * Mathf.PI * frequency 
				             * (Time.time - Time.deltaTime))) * new Vector3(-1.0f,0,0);
				//check if it can sway left/right before it tries
				if ((nextPosition.x > transform.position.x && canSwayRight) ||
				    (nextPosition.x < transform.position.x && canSwayLeft))
				{
					transform.position = nextPosition;
				}
				if(transform.rotation != (new Quaternion(0f, 0f, 0f, 1.0f)))
				{
					transform.rotation = new Quaternion(transform.rotation.x - rotateX,
					                                    transform.rotation.y - rotateY,
					                                    transform.rotation.z - rotateZ,
					                                    1.0f);
				}
				transform.Translate(new Vector3(0, -0.01f, 0), Space.World);
			}
		}

		//object cleanup
		if (destroyTimer <= 0)
			Destroy (this.gameObject);
		
		//handling transparency fade-out
		if (destroyTimer <= 180) {
			//change this to handle multiple materials.
			Color newColor = new Color(transform.GetChild(0).renderer.material.color.r, 
			                           transform.GetChild(0).renderer.material.color.g,
			                           transform.GetChild(0).renderer.material.color.b,
			                           ((float)destroyTimer/180f));
			transform.GetChild(0).renderer.material.SetColor("_Color", newColor);
		}
	}
	
	//this will unparent it from the player and cause it to start moving
	public void detach()
	{
		transform.parent = null;
		attached = false;
		if(transform.rotation != new Quaternion(0, 0, 0, 1.0f))
		{
			rotateX = transform.rotation.x / rotateRate;
			rotateY = transform.rotation.y / rotateRate;
			rotateZ = transform.rotation.z / rotateRate;
		}
	}
	
}
