using UnityEngine;
using System.Collections;

public class blossomMover : MonoBehaviour {

	public bool attached;
	public bool falling;
	
	public int count;

	// this will be what number flower it is
	public int healthNum = 0;

	public float amplitude;
	public float frequency;

	// Use this for initialization
	void Start () {
		attached = true;
		falling = false;
		count = 0;
	}

	// Update is called once per frame
	void Update () {

		if (count != 0)
				falling = false;
		else
				falling = true;

		if (!attached && falling) {
			transform.position += amplitude * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) 
			                    	- Mathf.Sin (2 * Mathf.PI * frequency 
			          	 			* (Time.time - Time.deltaTime))) * new Vector3(-1.0f,0,0);
			/*transform.rotation += amplitude * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) 
			                                   - Mathf.Sin (2 * Mathf.PI * frequency 
			             * (Time.time - Time.deltaTime))) * new Quaternion(0,0,-1.0f, 0);*/
			transform.Translate(new Vector3(0, -0.01f, 0), Space.World);
		}

		if(Input.GetKey("1"))
		   detach();
	}

	//this will unparent it from the player and cause it to start moving
	public void detach()
	{
		transform.parent = null;
		attached = false;
	}


	void OnTriggerEnter()
	{
		count++;
	}
	
	void OnTriggerExit()
	{
		count--;
	}

}
