using UnityEngine;
using System.Collections.Generic;

/*
This class is gonna be chill as fuck.

The vine mesh will be procedurally generated.
We will have a data structires that hold information about the tip of the vine and each ring.
Functions that can find the specific vertices for a given ring (or the tip), so they can be transformed to the proper position.
Functions to grow the vine by adding a new ring just before the tip
Functions that add randomly placed aesthetic elements on the vines to give them variation

The idea is that the VinePlant class will have several Vine children that it gives direction to,
And the actual computation, logic, and movement of the vines is handled here.
*/

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Vine : MonoBehaviour
{
	// global class variables

	[System.Serializable]
	public class VineSettings {
		// how many faces does each vine segment have? 4 = square, 6 = hexagonal, etc.
		public int resolution = 4;
		public float growthAcceleration = 1.0f;
		public float ringRadiusVariation = 0.1f;
		public float ringDirectionVariation = 0.1f;
	}

	public VineSettings vineSettings = new VineSettings();

	// number of segments, not including the tip.
	private int numSegments = 1;
	private float initialRadius = 0.1f;
	private float initialSegLength = 0.4f;
	
	private int numRings;
	private int numFaces;
	private int numTriangles;
	private int numTriVerts;
	private float ringRadians;

	private Mesh mesh;

	/* vertices has the following format:
	Vertices 0 through (resolution - 1) are the base ring.
	Vertices (resolution) through (2 * resolution - 1) are the tip ring.
	Vertex (2 * resolution) is the tip point.
	All vertices after (2* resolution) belong to the middle rings (in order from the base). */
	private List<Vector3> vertices;
	private List<int> triangles;

	/* ringRadii has the following format:
	Index 0 refers to the base ring.
	Index 1 refers to the point ring.
	All indexes after 1 refer to the middle rings (in order from the base). */
	private List<float> ringRadii;
	private List<float> segLengths;
	/* ringDirections has the following format:
	Index 0 refers to the base ring.
	Index 1 refers to the point ring.
	All indexes after 1 refer to the middle rings (in order from the base). */
	private List<Vector3> ringDirections;

	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;


	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();

		// cache the transform so we don't have to do expensive lookups
		// idk why, but it's a thing
		// but some Googling revealed that it may no longer be necessary?
		// I'm just following the sample code I found, and they do it.
		_transform = transform;

		// some helpful calculations based on vineSettings
		numRings = numSegments + 1;
		numFaces = vineSettings.resolution * numSegments;
		numTriangles = 2 * vineSettings.resolution * numSegments;
		numTriVerts = 3 * numTriangles;
		ringRadians = 2 * Mathf.PI / vineSettings.resolution;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();

		// initialize ringRadii to the default radius value from vineSettings
		ringRadii = new List<float>();
		for (int i = 0; i < numRings; i++)
		{
			ringRadii.Add(initialRadius);
		}

		// initialize segLengths to a small initial segment length
		segLengths = new List<float>();
		for (int i = 0; i < numSegments; i++)
		{
			segLengths.Add(initialRadius);
		}

		// initialize ringDirections to all point up
		ringDirections = new List<Vector3>();
		for (int i = 0; i < numRings; i++)
		{
			ringDirections.Add(Vector3.up);
		}

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "Vine";


		createInitialVineMesh();

		int debugCount = 0;
	}

	void Update()
	{
		updateVineData();

		// update mesh, if needed (like when we add a new segment)

		// apply transformations to position the mesh (this should really be sent to the vertex shader)

		if (debugCount == 1000)
		{
			addNewSegment();
			debugCount = 0;
		}
	}

	private void refreshMeshValues()
	{
	}

	// starts off the mesh with just a base ring, a tip
	// this will get called only once, in Start
	private void createInitialVineMesh()
	{
		int res = vineSettings.resolution;

		// just in case, clear things that should already be empty
		mesh.Clear();
		vertices.Clear();
		triangles.Clear();

		// add the vertices for the base ring
		for (int ringVert = 0; ringVert < res; ringVert++)
		{
			float angle = ringVert * ringRadians * -1;
			float v_x = ringRadii[0] * Mathf.Cos(angle);
			float v_z = ringRadii[0] * Mathf.Sin(angle);
			float v_y = 0;

			vertices.Add(new Vector3(v_x, v_y, v_z));
		}

		// add all the vertices in the tip ring
		for (int ringVert = 0; ringVert < res; ringVert++)
		{
			float angle = ringVert * ringRadians * -1;
			float v_x = ringRadii[numRings - 1] * Mathf.Cos(angle);
			float v_z = ringRadii[numRings - 1] * Mathf.Sin(angle);
			float v_y = initialSegLength * numSegments;

			vertices.Add(new Vector3(v_x, v_y, v_z));
		}

		// add the vertex for the tip's point

		vertices.Add(new Vector3(0,initialSegLength * (numSegments + 1),0));

		mesh.vertices = vertices.ToArray();

		// now, define how to triangles are to be drawn between these vertices

		// define the faces of the segment between base ring and tip ring
		for (int faceNum = 0; faceNum < res; faceNum++)
		{
			// NOTE: normal will point outwards according to right-hand rule.
			// So, add the vertices counter-clockwise in order for normal to face outwards

			int bottomLeft = faceNum;
			int bottomRight = (faceNum + 1) % res;
			int topLeft = bottomLeft + res;
			int topRight = bottomRight + res;

			// add first triangle's vertices
			triangles.Add(bottomLeft);
			triangles.Add(topRight);
			triangles.Add(topLeft);

			// add second triangle's vertices
			triangles.Add(bottomLeft);
			triangles.Add(bottomRight);
			triangles.Add(topRight);
		}

		// define the triangles between the tip ring and tip point
		for (int triNum = 0; triNum < res; triNum++)
		{
			// NOTE: normal will point outwards according to right-hand rule.
			// So, add the vertices counter-clockwise in order for normal to face outwards

			int bottomLeft = res + triNum;
			int bottomRight = res + (triNum + 1) % res;
			int top = res * 2;

			// add the triangle's vertices
			triangles.Add(bottomLeft);
			triangles.Add(bottomRight);
			triangles.Add(top);
		}

		mesh.triangles = triangles.ToArray();

	}

	private void addNewSegment()
	{
		int res = vineSettings.resolution;

		// a new ring will be added before the tip's ring.
		numSegments++;
		ringRadii.Add(initialRadius);
		segLengths.Add(initialSegLength);
		ringDirections.Add(Vector3.up);

		updateVineData();

		// it will be placed in the same position as the tip ring
		for (int ringVert = 0; ringVert < res; ringVert++)
		{
			float angle = ringVert * ringRadians * -1;
			float v_x = ringRadii[numRings - 1] * Mathf.Cos(angle);
			float v_z = ringRadii[numRings - 1] * Mathf.Sin(angle);
			float v_y = vertices[res + ringVert].y;

			vertices.Add(new Vector3(v_x, v_y, v_z));
		}

		// move the tip to make room for the new segment
		// note the <=, since this will include the tip point's vertex as well
		for (int ringVert = 0; ringVert <= res; ringVert++)
		{
			vertices[res+ringVert].y += initialSegLength;
		}

		mesh.vertices = vertices.ToArray();

		// now redefine how the faces are drawn.

		// delete the info for the last segment, which we are splitting up now
		int rangeStart = triangles.Count - 1 - (6 * res);
		int rangeEnd = triangles.Count - 1;
		triangles.RemoveRange(rangeStart, rangeEnd);

		// now define the faces for the two segments before the tip
	}

	private void updateVineData()
	{
		numRings = numSegments + 1;
		numFaces = vineSettings.resolution * numSegments;
		numTriangles = 2 * vineSettings.resolution * numSegments;
		numTriVerts = 3 * numTriangles;
		ringRadians = 2 * Mathf.PI / vineSettings.resolution;
	}
}