using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class Fluff_spawner : MonoBehaviour {

	public int preloadedSeed;
	public int activeSeed;
	public GameObject[] fluffObjects;
	public bool weighObjects;
	public float[] weights;
	//roughly how many gameObjects we want spawned in this area
	public float density;

	public float spawnOffset = 1.0f;
	public bool preventOverlap = false;

	public bool overrideColor;
	public Color newColor;

	List<GameObject> spawnedFluff;

	float spawnHeight;
	float spawnDepth;
	float xLeft;
	float xRight;
	float length;

	float totalFreq = 0;

	// Use this for initialization
	void Start () {
		//we flat-out can't handly rotation; none of our fluff allows for it

		if (transform.parent.transform.rotation.x != 0 && transform.parent.transform.rotation.x != 270 && transform.parent.transform.rotation.x != 90 )
			Destroy (gameObject);
		//seed random so we can get a consistant experience if we so chose
		if (preloadedSeed == 0) {
			Random.seed = (int) System.DateTime.Now.Ticks;
		} else {
			Random.seed = preloadedSeed;
		}

		if (density == 0) {
			density = Random.Range(10, 30);
		}

		//to see if there's a seed we really like and want to save
		activeSeed = Random.seed;

		Bounds boxBounds = gameObject.GetComponent<BoxCollider> ().bounds;
		spawnHeight = transform.position.y;
		spawnDepth = transform.position.z; 
		xLeft = transform.position.x - boxBounds.extents.x;
		xRight = transform.position.x + boxBounds.extents.x;
		length = Mathf.Abs (xLeft - xRight);

		if (weighObjects) {
			if(weights.Length != fluffObjects.Length)
			{
				Debug.Log ("Fluff spawner error: weight array needs to be the same length as the fluff objects array!");
				weighObjects = false;
			} else{// do this once:
				for(int i=0; i<weights.Length; i++)
					totalFreq += weights[i];
			}
		}

		spawnedFluff = new List<GameObject> ();

		spawnFluff ();
		if (overrideColor) {
			ReassignColors();
		}

	}
	
	public void spawnFluff(){
		if (fluffObjects.Length == 0) {
			Debug.Log ("This instance of Fluff_spawner has no fluff in its array to spawn!");
			return;
		}

		float roughDist = length / density;

		Vector3 currPos = new Vector3 (xLeft, spawnHeight, spawnDepth);
		GameObject prev = null;
		while (currPos.x < xRight) {
			Vector3 spawnPoint = new Vector3(Mathf.Clamp(currPos.x + Random.Range(-1 * spawnOffset, spawnOffset), xLeft, xRight),
			                                 currPos.y, currPos.z);
			int index;
			if(weighObjects){
				index = Roll();
			}else
				index = Random.Range(0, fluffObjects.Length);
			GameObject newFluff = Instantiate (fluffObjects[index], spawnPoint, fluffObjects[index].transform.rotation) as GameObject;
			newFluff.transform.SetParent(transform);
			currPos = new Vector3(spawnPoint.x + roughDist, currPos.y, currPos.z);
			if(preventOverlap)
			{
	
				if(prev != null)
				{
					Bounds currCombinedBounds;

					if(newFluff.renderer != null){
						currCombinedBounds = newFluff.renderer.bounds;
					}
					else
					{
						currCombinedBounds = newFluff.GetComponentInChildren<Renderer>().bounds;
					}
					Renderer[] currRenderers = newFluff.GetComponentsInChildren<Renderer>();
					foreach (Renderer render in currRenderers) {
						if (render != renderer) 
							currCombinedBounds.Encapsulate(render.bounds);
					}
					Bounds prevCombinedBounds;
					if(prev.renderer != null){
						prevCombinedBounds = prev.renderer.bounds;
					}
					else
					{
						prevCombinedBounds = prev.GetComponentInChildren<Renderer>().bounds;
					}
					Renderer[] prevRenderers = prev.GetComponentsInChildren<Renderer>();
					foreach (Renderer render in prevRenderers) {
						if (render != renderer) 
							prevCombinedBounds.Encapsulate(render.bounds);
					}
					float currLeft = newFluff.transform.position.x - currCombinedBounds.extents.x;
					float prevRight = prev.transform.position.x + prevCombinedBounds.extents.x;
					while(currLeft < prevRight)
					{
						newFluff.transform.position = new Vector3(newFluff.transform.position.x +0.1f, newFluff.transform.position.y, newFluff.transform.position.z);
						currPos = new Vector3(currPos.x +0.1f, currPos.y, currPos.z);
						currLeft = newFluff.transform.position.x - currCombinedBounds.extents.x;
					}
				}
				prev = newFluff;
				spawnedFluff.Add(newFluff);
			}
		}

	}

	void ReassignColors()
	{
		foreach (GameObject item in spawnedFluff) {
			if(item.renderer != null)
			{
				item.renderer.material.color = newColor;
			}
			Renderer[] childRenderers = item.GetComponentsInChildren<Renderer>();
			foreach(Renderer render in childRenderers)
			{
				foreach(Material mat in render.materials)
				{
					mat.color = newColor;
				}
			}
		}
	}

	int Roll(){
		float roll= Random.Range(0,totalFreq);
		// Ex: we roll 0.68
		//   #0 subtracts 0.25, leaving 0.43
		//   #1 subtracts 0.4, leaving 0.03
		//   #2 is a hit
		int index = -1;
		for(int i=0; i<weights.Length; i++) {
			if(roll<=weights[i]) 
			{ 
				index=i; 
				break; 
			}
			roll -= weights[i];
		}
		// just in case we manage to roll 0.0001 past the highest:
		if(index==-1) 
			index = Random.Range(0, fluffObjects.Length);

		return index;
	}
}
