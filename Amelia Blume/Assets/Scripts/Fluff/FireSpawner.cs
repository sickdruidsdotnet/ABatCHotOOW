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

	public GameObject focusObject;
	public Vector3 focus;

	public AudioClip fireSound;
	private AudioSource source;
	public float vol;

	void Start()
	{
		spawnFlame();
		lastSpawnTime = Time.time;
		extents = GetComponent<Collider>().bounds.extents;
		waitTime = 1.0f / (density * extents.x * extents.z);
		focusObject = transform.Find("Focus").gameObject;
		focus = focusObject.transform.position;

		source = GetComponent<AudioSource>();
		source.loop = true;
		source.clip = fireSound;
		vol = density*0.02F;
		source.volume = vol;
		//source.volume = 0.5F;
		source.Play();
	}

	void Update()
	{
		focus = focusObject.transform.position;
		waitTime = 1.0f / (density * extents.x * extents.z);
		
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
		Vector3 end = Vector3.MoveTowards(start, focus, flameTargetSpread);
		flame.GetComponent<Flame>().setTrajectory(start, end);
		flame.transform.parent = transform;
	}
}