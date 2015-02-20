using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestMesh : MonoBehaviour
{
	// global class variables

	[System.Serializable]
	public class MeshSettings {
		public int resolution = 16;
		public int numSegments = 8;
		public float radius = 1.0f;
		public float segmentHeight = 0.2f;
		public float radiusGoal = 2.0f;
		public float heightGoal = 8.0f;
		public float growthAcceleration = 1.0f;
		public float ringRadiusVariation = 0.1f;
		public float ringDirectionVariation = 0.1f;
	}

	public MeshSettings meshSettings = new MeshSettings();
	
	private int numRings;
	private int numFaces;
	private int numTriangles;
	private int numTriVerts;

	private float[] ringRadii;
	private Vector3[] ringDirections;



	private Mesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;



	public TestMesh()
	{
		// constructor
		Debug.Log("This is the constructor");

		Vector3 testVec = new Vector3(1.0f,1.0f,0.0f).normalized;
		Debug.Log(testVec);
	}

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();

		// cache the transform so we don't have to do expensive lookups
		// idk why, but it's a thing
		// but some Googling revealed that it may no longer be necessary?
		// I'm just following the sample code I found, and they do it.
		_transform = transform;

		// some helpful calculations based on meshSettings
		numRings = meshSettings.numSegments + 1;
		numFaces = meshSettings.resolution * meshSettings.numSegments;
		numTriangles = 2 * meshSettings.resolution * meshSettings.numSegments;
		numTriVerts = 3 * numTriangles;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();

		// initialize ringRadii to the default radius value from meshSettings
		ringRadii = new float[numRings];
		for (int i = 0; i < ringRadii.Length; i++)
		{
			ringRadii[i] = meshSettings.radius;
		}

		// initialize ringDirections to all point up
		ringDirections = new Vector3[numRings];
		for (int i = 0; i < ringDirections.Length; i++)
		{
			ringDirections[i] = Vector3.up;
		}


		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "TestMesh";

		generateRandomizedMeshElements();
		drawMesh(meshSettings.resolution, meshSettings.numSegments, ringRadii, meshSettings.segmentHeight);
	}

	void Update()
	{
		// refreshMeshValues();
	}

	void FixedUpdate()
	{
		// update the mesh
		if(meshRenderer.enabled)
		{
			drawMesh(meshSettings.resolution, meshSettings.numSegments, ringRadii, meshSettings.segmentHeight);
		}
	}

	private void generateRandomizedMeshElements()
	{
		// vary the radius of each ring;
		for (int i = 0; i < ringRadii.Length; i++)
		{
			ringRadii[i] = meshSettings.radius * Random.Range(1.0f - meshSettings.ringRadiusVariation, 1.0f + meshSettings.ringRadiusVariation);
		}

		// vary the direction of each ring, offset by the ring below it
		for (int i = 0; i < ringDirections.Length; i++)
		{
			if (i != 0)
			{
				// set direction parallel to previous, and then we will offset it
				ringDirections[i] = ringDirections[i-1];
			}

			float offsetX = Random.Range(-meshSettings.ringDirectionVariation, meshSettings.ringDirectionVariation);
			float offsetY = Random.Range(-meshSettings.ringDirectionVariation, meshSettings.ringDirectionVariation);
			float offsetZ = Random.Range(-meshSettings.ringDirectionVariation, meshSettings.ringDirectionVariation);

			Vector3 newVec = new Vector3(ringDirections[i].x + offsetX, ringDirections[i].y + offsetY, ringDirections[i].z + offsetZ);

			ringDirections[i] = newVec.normalized;

		}


	}

	private void refreshMeshValues()
	{
		float meshHeight = meshSettings.numSegments * meshSettings.segmentHeight;

		meshSettings.segmentHeight = Mathf.Lerp(meshHeight, meshSettings.heightGoal, meshSettings.growthAcceleration * Time.deltaTime) / meshSettings.numSegments;
		meshSettings.radius = Mathf.Lerp(meshSettings.radius, meshSettings.radiusGoal, meshSettings.growthAcceleration * Time.deltaTime);
	}

	// fills mesh.vertices and mesh.triangles with the appropriate values based on meshSettings
	private void drawMesh(int res, int segCount, float[] rads, float segHeight)
	{
		mesh.Clear();
		vertices.Clear();
		triangles.Clear();
		
		float ringRadians = 2 * Mathf.PI / res;

		// draw the points for each ring in the cylinder tower

		for (int ringNum = 0; ringNum < (segCount + 1); ringNum++)
		{
			
			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = rads[ringNum] * Mathf.Cos(angle);
				float v_z = rads[ringNum] * Mathf.Sin(angle);
				float v_y = ringNum * segHeight;

				vertices.Add(new Vector3(v_x, v_y, v_z));
			}
		}

		mesh.vertices = vertices.ToArray();

		// draw the two triangles for each face connecting the rings

		for (int segNum = 0; segNum < segCount; segNum++)
		{
			for (int faceNum = 0; faceNum < res; faceNum++)
			{
				// NOTE: normal will point outwards according to right-hand rule.
				// So, add the vertices counter-clockwise in order for normal to face outwards

				int bottomLeft = segNum * res + faceNum;
				int bottomRight = segNum * res + (faceNum + 1) % res;
				int topLeft = bottomLeft + res;
				int topRight = bottomRight + res;

				// add first triangle's vertices
				triangles.Add(bottomLeft); // bottom-left
				triangles.Add(topRight); // top-right
				triangles.Add(topLeft); // top-left

				// add second tringle's vertices
				triangles.Add(bottomLeft); // bottom-left
				triangles.Add(bottomRight); // bottom-right
				triangles.Add(topRight); // top-right

			}
		}

		mesh.triangles = triangles.ToArray();

		//triangles.ForEach(item => Debug.Log(item));
	}
}