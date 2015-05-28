using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/*
This vine shred does not need any of the crazy cool functionality 
of a living vine. No IK, no dynamic growth, no trigger colliders, 
not even a skeleton! However, this GameObject WILL have a physics 
collider, something that other vine NEVER had. So it's special.
*/

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class VineShred : MonoBehaviour
{
	private Mesh mesh;
	private MeshRenderer meshRenderer;
	private MeshCollider meshCollider;
	public float lifespan = 2f;
	public float fadeLength = 1f;
	private float spawnTime;
	private float fadeStartTime;
	private bool killSwitchEngaged = false;

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		GetComponent<MeshFilter>().mesh = mesh;

		spawnTime = Time.time;
	}

	void Update()
	{
		if (Time.time > spawnTime + lifespan)
		{
			if (!killSwitchEngaged)
			{
				Destroy(gameObject, fadeLength);
				fadeStartTime = Time.time;
				killSwitchEngaged = true;
			}
			if (meshRenderer.material != null)
			{
				float fadePercentage = (Time.time - fadeStartTime) / fadeLength;
				float alpha = Mathf.Lerp(1, 0, fadePercentage);
				Color prevColor = meshRenderer.material.color;
				Color newColor = new Color(prevColor.r, prevColor.g, prevColor.b, alpha);
				meshRenderer.material.SetColor("_Color", newColor);
			}
		}
	}

	public void setMesh(Mesh m)
	{
		mesh = m;
		createMeshCollider();
	}

	private void createMeshCollider()
	{
		meshCollider = gameObject.AddComponent(typeof(MeshCollider)) as MeshCollider;

		meshCollider.sharedMesh = mesh; 
	}

	public void setMaterial(Material mat)
	{
		if (meshRenderer == null) {meshRenderer = GetComponent<MeshRenderer>();}
		meshRenderer.material = mat;
	}

	public void ignore(GameObject ignoredObject)
	{
		if (ignoredObject == null) {Debug.Log("can't ignore null object"); return;}
		Physics.IgnoreCollision(ignoredObject.collider, meshCollider);
	}
}