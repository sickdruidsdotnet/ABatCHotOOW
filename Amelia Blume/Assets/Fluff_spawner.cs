using UnityEngine;
using System.Collections;

public class Fluff_spawner : MonoBehaviour {

	public int preloadedSeed;
	public GameObject[] fluffObjects;
	//roughly how many gameObjects we want spawned in this area
	public float density;

	public float spawnOffset = 1.0f;

	float spawnHeight;
	float spawnDepth;
	float xLeft;
	float xRight;
	float length;

	// Use this for initialization
	void Start () {
		//seed random so we can get a consistant experience if we so chose
		if (preloadedSeed == 0) {
			Random.seed = (int) System.DateTime.Now.Ticks;
		} else {
			Random.seed = preloadedSeed;
		}

		if (density == 0) {
			density = Random.Range(10, 30);
		}

		Bounds boxBounds = gameObject.GetComponent<BoxCollider> ().bounds;
		spawnHeight = transform.position.y;
		spawnDepth = transform.position.z; 
		xLeft = transform.position.x - boxBounds.extents.x;
		xRight = transform.position.x + boxBounds.extents.x;
		length = Mathf.Abs (xLeft - xRight);

		spawnFluff ();


	}
	
	public void spawnFluff(){
		if (fluffObjects.Length == 0) {
			Debug.Log ("This instance of Fluff_spawner has no fluff in its array to spawn!");
			return;
		}

		float roughDist = length / density;

		Vector3 currPos = new Vector3 (xLeft, spawnHeight, spawnDepth);
		while (currPos.x < xRight) {
			Vector3 spawnPoint = new Vector3(Mathf.Clamp(currPos.x + Random.Range(-1 * spawnOffset, spawnOffset), xLeft, xRight),
			                                 currPos.y, currPos.z);
			/*if(spawnPoint.x > xRight)
			{
				break;
			}*/
			int index = Random.Range(0, fluffObjects.Length);
			GameObject newFluff = Instantiate (fluffObjects[index], spawnPoint, fluffObjects[index].transform.rotation) as GameObject;
			newFluff.transform.SetParent(transform);
			currPos = new Vector3(spawnPoint.x + roughDist, currPos.y, currPos.z);
		}

	}
}
