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
	Vector3 targetStart;
	Vector3 targetEnd;
	Vector3 initialPos;
	Vector3 initialScale;
	Vector3 goalScale = Vector3.zero;
	Quaternion initialRot;
	Quaternion goalRotation;



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
		initialRot = transform.rotation;
		float rot = Random.Range(90, 180);
		if (Random.Range(0, 1f) < 0.5f) {rot *= -1f;}
		goalRotation = Quaternion.AngleAxis(rot, Vector3.up);
		initialScale = new Vector3(initialScaleFactor,initialScaleFactor,initialScaleFactor);
		startTime = Time.time;
	}

	void Update()
	{
		beFire();
		if (transform.position == targetEnd)
		{
			Destroy(gameObject);
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
		Material fireMat = new Material(Shader.Find("ABlumeUnlit"));
		fireMat.color = new Color(Random.Range(0.9f, 1f), Random.Range(0.5f, 0.65f), 0);
		meshRenderer.material = fireMat;
	}

	private void beFire()
	{
		float percent = (Time.time - startTime) * flameSpeed / Vector3.Distance(initialPos, targetEnd);
		Vector3 targetLoc = Vector3.Lerp(targetStart, targetEnd, percent);
		transform.position = Vector3.Lerp(initialPos, targetLoc, percent);
		Vector3 rotAxis = Vector3.Cross(Vector3.up, targetLoc - transform.position);
		// first rotation points towards target, second rotation twists about its local y-axis
		transform.rotation = Quaternion.AngleAxis(Vector3.Angle(Vector3.up, targetLoc - transform.position), rotAxis) * Quaternion.Lerp(initialRot, goalRotation, percent);
		transform.localScale = Vector3.Lerp(initialScale, goalScale, percent);
	}

	public void setTrajectory(Vector3 start, Vector3 end)
	{
		targetStart = start;
		targetEnd = end;
	}

}