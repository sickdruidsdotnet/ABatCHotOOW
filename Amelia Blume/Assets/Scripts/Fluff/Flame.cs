using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Flame : MonoBehaviour
{
	// octahedron dimensions
	public float topLength = 0.8f;
	public float bottomLength = 0.2f;
	public float width = 0.4f;
	public float initialScaleFactor = 1.0f;


	// procedural mesh stuff
	private MeshRenderer meshRenderer;
	private Mesh mesh;
	private List<Vector3> vertices;

	// transformation stuff
	public float flameSpeed = 1.0f;
	private float startTime;
	Vector3 targetLoc;
	Vector3 initialPos;
	Vector3 initialScale;
	Vector3 goalScale = Vector3.zero;



	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		vertices = new List<Vector3>();
		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "Flame_Mesh";
		createMesh();
		setMaterial();

		initialPos = transform.position;
		initialScale = new Vector3(initialScaleFactor,initialScaleFactor,initialScaleFactor);
		startTime = Time.time;
	}

	void Update()
	{
		if (targetLoc != null)
		{
			beFire();
			if (transform.position == targetLoc)
			{
				Destroy(gameObject);
			}
		}
	}

	private void createMesh()
	{
		// We want the object's origin to be at the tippy top of the flame,
		// so that shen we scale it as it rises, it appers to shrink from 
		// the bottom up.

		// top
		vertices.Add(Vector3.zero);
		// front right
		vertices.Add(new Vector3(width/2, -topLength, -width/2));
		// front left
		vertices.Add(new Vector3(-width/2, -topLength, -width/2));
		// back left
		vertices.Add(new Vector3(-width/2, -topLength, width/2));
		// back right
		vertices.Add(new Vector3(width/2, -topLength, width/2));
		// bottom
		vertices.Add(new Vector3(0, -topLength - bottomLength, 0));

		mesh.vertices = vertices.ToArray();

		// Might want to split this into submeshes later
		mesh.triangles = new int[] {0, 1, 2, 0, 2, 3, 0, 3, 4, 0, 4, 1, 
									2, 1, 5, 3, 2, 5, 4, 3, 5, 1, 4, 5};
		mesh.uv = new Vector2[] {Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero, Vector2.zero};
	}

	private void setMaterial()
	{
		Material fireMat = Resources.Load("Materials/Fire_base", typeof(Material)) as Material;
		meshRenderer.material = fireMat;
	}

	private void beFire()
	{
		float percent = (Time.time - startTime) * flameSpeed / Vector3.Distance(initialPos, targetLoc);
		transform.position = Vector3.Lerp(initialPos, targetLoc, percent);
		transform.localScale = Vector3.Lerp(initialScale, goalScale, percent);
	}

	public void setTrajectory(Vector3 target)
	{
		targetLoc = target;
	}

}