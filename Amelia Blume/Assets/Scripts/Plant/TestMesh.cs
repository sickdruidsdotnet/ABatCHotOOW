using UnityEngine;
using System.Collections.Generic;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class TestMesh : MonoBehaviour
{
	// global class variables

	[System.Serializable]
	public class MeshSettings {
		public int resolution = 8;
		public int numSegments = 3;
		public int radius = 10;
		public int segmentHeight = 25;
	}

	public MeshSettings meshSettings = new MeshSettings();
	
	private int numFaces;
	private int numTriangles;
	private int numTriVerts;

	private Mesh mesh;
	private List<Vector3> vertices;
	private List<int> triangles;
	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;



	public TestMesh()
	{
		// constructor
		Debug.Log("THis is the constructor");
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
		numFaces = meshSettings.resolution * meshSettings.numSegments;
		numTriangles = 2 * meshSettings.resolution * meshSettings.numSegments;
		numTriVerts = 3 * numTriangles;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "TestMesh";

		drawMesh();
	}

	void Update()
	{
		if(meshRenderer.enabled)
		{
			drawMesh();
		}
	}

	// fills mesh.vertices and mesh.triangles with the appropriate values based on meshSettings
	private void drawMesh()
	{
		mesh.Clear();
		
		float ringRadians = 2 * Mathf.PI / meshSettings.resolution;

		// draw the points for each ring in the cylinder tower

		for (int ringNum = 0; ringNum < (meshSettings.numSegments + 1); ringNum++)
		{
			for (int ringVert = 0; ringVert < meshSettings.resolution; ringVert++)
			{
				float angle = ringVert * ringRadians;
				float v_x = meshSettings.radius * Mathf.Cos(angle);
				float v_z = meshSettings.radius * Mathf.Sin(angle);
				float v_y = ringNum * meshSettings.segmentHeight;

				vertices.Add(new Vector3(v_x, v_y, v_z));
			}
		}

		mesh.vertices = vertices.ToArray();

		// draw the two triangles for each face connecting the rings

		for (int segNum = 0; segNum < meshSettings.numSegments; segNum++)
		{
			for (int faceNum = 0; faceNum < meshSettings.resolution; faceNum++)
			{
				// NOTE: normal will point outwards according to right-hand rule.
				// So, add the vertices counter-clockwise in order for normal to face outwards

				// add first triangle's vertices
				triangles.Add(faceNum); // bottom-left
				triangles.Add(faceNum + (meshSettings.resolution + 1)); // top-right
				triangles.Add(faceNum + meshSettings.resolution); // top-left

				// add second tringle's vertices
				triangles.Add(faceNum); // bottom-right
				triangles.Add(faceNum + 1); // bottom-left
				triangles.Add(faceNum + (meshSettings.resolution + 1)); // top-right

			}
		}

		mesh.triangles = triangles.ToArray();
	}
}