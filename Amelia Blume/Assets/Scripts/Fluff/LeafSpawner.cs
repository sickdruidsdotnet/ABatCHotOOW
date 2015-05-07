using UnityEngine;
using System.Collections;

public class LeafSpawner : MonoBehaviour {

	//resize the trigger box's x size in editor and it will encompass that area.
	float width;

	public bool leaf_override = false;
	public GameObject sampleLeaf;

	//how many leaves per second you want to spawn
	public float spawnRate = 0.1f;
	float lastSpawnTime;

	// Use this for initialization
	void Start () {
		width = gameObject.GetComponent<BoxCollider> ().size.x;
		lastSpawnTime = Time.time;
		if(spawnRate == 0)
		{
			Destroy(gameObject);
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (Mathf.Abs (lastSpawnTime - Time.time) > (1f/spawnRate)) {
			spawnLeaf();
			lastSpawnTime = Time.time;
		}
		
	}

	void spawnLeaf()
	{

		GameObject leaf;
		if (leaf_override) {
			leaf = Instantiate((Object)sampleLeaf) as GameObject;
		} else {
			leaf = Instantiate(Resources.Load("Falling_Leaf")) as GameObject;
		}
		leaf.transform.position = new Vector3 (transform.position.x + (Random.Range (0, width) - (width / 2f)),
		                                      transform.position.y, transform.position.z);
		leaf.BroadcastMessage ("randomize", leaf_override);

		leaf.transform.parent = transform;

	}
}
