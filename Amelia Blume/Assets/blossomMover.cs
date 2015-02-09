using UnityEngine;
using System.Collections;

public class blossomMover : MonoBehaviour {

	public bool attached;
	public bool falling;

	Vector3 prevPos;
	public int count;

	public float amplitude;
	public float frequency;

	// Use this for initialization
	void Start () {
		attached = true;
		falling = false;
		prevPos = transform.position;
		count = 0;
	}

	// Update is called once per frame
	void Update () {

		if (count != 0)
				falling = false;
		else
				falling = true;

		if (!attached && falling) {
			transform.position += amplitude * (Mathf.Sin (2 * Mathf.PI * frequency * Time.time) - Mathf.Sin (2 * Mathf.PI * frequency * (Time.time - Time.deltaTime))) * new Vector3(-1.0f,0,0);
			transform.Translate(new Vector3(0, -0.01f, 0), Space.World);
		}


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
