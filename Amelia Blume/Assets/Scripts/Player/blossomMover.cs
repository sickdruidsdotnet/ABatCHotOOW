using UnityEngine;
using System.Collections;

/*Parent this object to the player, then save it into an
 * array that will deal with dropping them at the proper times
 */

public class blossomMover : MonoBehaviour {

	public bool attached;
	public bool falling;
	
	public int count;

	public int destroyTimer;

	float rotateRate = 300;
	float rotateX;
	float rotateY;
	float rotateZ;

	public Transform initialTransform;

	// this will be what number flower it is
	public int healthNum = 0;

	public float amplitude;
	public float frequency;

	// Use this for initialization
	void Start () {
		destroyTimer = 900;
		initialTransform = transform;
		//initialTransform.position = transform.localPosition;

		if(transform.parent != null)
		{
			attached = true;
			falling = false;
		}
		else
			attached = false;
		count = 0;
	}

	// Update is called once per frame
	void Update () {

		if (count != 0)
				falling = false;
		else
				falling = true;

		if (!attached) {
			destroyTimer--;
			if(falling){
				transform.position += amplitude * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) 
				                    	- Mathf.Sin (2 * Mathf.PI * frequency 
				          	 			* (Time.time - Time.deltaTime))) * new Vector3(-1.0f,0,0);
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
		if (destroyTimer <= 0)
			Destroy (this.gameObject);

		//handling transparency fade-out, not functional yet
		if (destroyTimer <= 180) {
			//change this to handle multiple materials.
			Color newColor = new Color(gameObject.renderer.material.color.r, 
				          gameObject.renderer.material.color.b,
				          gameObject.renderer.material.color.g,
				          ((float)destroyTimer/180f));
			this.gameObject.renderer.material.SetColor("_Color", newColor);

			Debug.Log("Alpha is: " + this.gameObject.renderer.material.color.a);
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

	void OnTriggerEnter(Collider other)
	{
		if( other.tag != "Player" && !other.isTrigger)
			count++;
	}
	
	void OnTriggerExit(Collider other)
	{
		if( other.tag != "Player" && !other.isTrigger)
			count--;
	}

}
