using UnityEngine;
using System.Collections.Generic;

public class FireSpawner : MonoBehaviour
{
	public float fireHeight = 1.5f;
	public float heightVariation = 0.4f;
	public float flameTargetSpread = 0.15f;
	// number of flames per second per square unit
	public float density = 20f;
	public float initialScale = 0.7f;
	public float lastSpawnTime;
	// how long (in seconds) to wait in between flame spawns
	private float waitTime;

	private Vector3 extents;

	void Start()
	{
		spawnFlame();
		lastSpawnTime = Time.time;
		extents = GetComponent<Collider>().bounds.extents;
		waitTime = 1.0f / (density * extents.x * extents.z);
	}

	void Update()
	{
		if (Time.time > lastSpawnTime + waitTime)
		{
			spawnFlame();
			lastSpawnTime = Time.time;
		}
	}

	void spawnFlame()
	{
		float xPos = Random.Range(-extents.x, extents.x);
		float zPos = Random.Range(-extents.z, extents.z);
		Vector3 pos = transform.position + new Vector3(xPos, 0, zPos);
		Quaternion rot = Quaternion.identity;
		GameObject flame = Instantiate(Resources.Load("Flame"), pos, rot) as GameObject;
		Vector3 start = pos + new Vector3(0,fireHeight + Random.Range(-heightVariation, heightVariation),0);
		Vector3 end = start + new Vector3(Random.Range(-flameTargetSpread, flameTargetSpread), 0, Random.Range(-flameTargetSpread, flameTargetSpread));
		flame.GetComponent<Flame>().setTrajectory(start, end);
		flame.transform.parent = transform;
	}
}