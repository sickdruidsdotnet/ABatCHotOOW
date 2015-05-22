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

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		GetComponent<MeshFilter>().mesh = mesh;
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
}