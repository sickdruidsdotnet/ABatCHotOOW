using UnityEngine;
using System.Collections;

public class FallingLeaf : MonoBehaviour {

	float fallrate = 0.01f;
	float xMoveRate = 0.01f;
	float amplitude = 0.03f;
	float frequency = 1.5f;
	float offset = 0f;
	float bottomLimit = -2;

	GameObject mainCamera;

	//edit this on the falling leaf prefab (Resources) in editor to have more colors.
	public Color[] leafColors;

	void Start()
	{
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
	}

	void FixedUpdate () {

		transform.position = new Vector3 (transform.position.x + Mathf.Sin ((Time.time + offset) * frequency) * amplitude, transform.position.y, transform.position.z);
		transform.position = new Vector3 (transform.position.x + xMoveRate, transform.position.y - fallrate, transform.position.z);

		transform.RotateAround (transform.position, Vector3.right, 400 * Time.deltaTime);	

		//Out of bounds clean-up
		if (transform.position.y <= mainCamera.transform.position.y - 10f) {
			Destroy(gameObject);
		}
	}

	void randomize(bool colorAndSize = false)
	{
		transform.localScale = new Vector3 (transform.localScale.x * Random.Range (0.75f, 1f), 
		                                    transform.localScale.y * Random.Range (0.75f, 1f), 
		                                    transform.localScale.z);
		fallrate = Random.Range (0.02f, 0.035f);
		xMoveRate = Random.Range (-0.02f, 0.02f);
		offset = Random.Range (0, 4);
		amplitude = Random.Range (0.01f, 0.05f);
		frequency = Random.Range (1f, 2f);
		if (!colorAndSize) {
			int index = Random.Range (0, leafColors.Length);
			renderer.material.SetColor ("_Color", leafColors [index]);
		}

		//they look bad when they're all facing the same direction
		if (Random.Range (0, 11) > 5) {
			transform.Rotate(new Vector3(0, 180f, 0f));
		}
	}
}
