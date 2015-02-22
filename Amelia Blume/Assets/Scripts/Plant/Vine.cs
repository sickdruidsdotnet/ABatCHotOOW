using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;
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
		public int resolution = 5;
		public float growthAcceleration = 1.0f;
		public float ringRadiusVariation = 0.1f;
		public float ringDirectionVariation = 0.1f;
	}

	public class VineNode {
		public float radius;
		public Vector3 startPoint;
		public Vector3 nodeRay;

		public VineNode(float rad, Vector3 start, Vector3 ray)
		{
			radius = rad;
			startPoint = start;
			nodeRay = ray;
		}
	}

	public List<VineNode> vineSkeleton;

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
	Vertex 0 is the tip.
	Vertices (VineNodeNum, resolution * VineNodeNum) correspond to the ring for each VineNode */
	private List<Vector3> vertices;
	private List<int> triangles;

	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;

	int debugCount = 0;


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
		vineSkeleton = new List<VineNode>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "Vine";


		createInitialVineSkeleton();

		
	}

	void Update()
	{
		// update mesh, if needed (like when we add a new segment)

		// apply transformations to position the mesh (this should really be sent to the vertex shader)

	}

	private void refreshMeshValues()
	{
	}

	private void createInitialVineSkeleton()
	{
		vineSkeleton.Add(new VineNode(initialRadius, Vector3.zero, new Vector3(0,initialSegLength,0)));
		Debug.Log("Node 0 start: " + vineSkeleton[0].startPoint);
		
		vineSkeleton.Add(new VineNode(initialRadius, vineSkeleton.Last().startPoint + vineSkeleton.Last().nodeRay, new Vector3(initialSegLength,initialSegLength,0)));
		Debug.Log("Node 1 start: " + vineSkeleton[1].startPoint);
		
		vineSkeleton.Add(new VineNode(initialRadius, vineSkeleton.Last().startPoint + vineSkeleton.Last().nodeRay, new Vector3(-initialSegLength,initialSegLength,0)));
		Debug.Log("Node 2 start: " + vineSkeleton[2].startPoint);

		vineSkeleton.Add(new VineNode(initialRadius, vineSkeleton.Last().startPoint + vineSkeleton.Last().nodeRay, new Vector3(initialSegLength,initialSegLength,0)));
		Debug.Log("Node 3 start: " + vineSkeleton[3].startPoint);
		

		createMesh();

		string vertString = "";
		for (int v = 0; v < mesh.triangles.Length; v++)
		{
			vertString += mesh.triangles[v];
			vertString += ", ";
		}

		Debug.Log(vertString);

	}

	void createMesh()
	{

		int res = vineSettings.resolution;

		// just in case, clear things that should already be empty
		mesh.Clear();
		vertices.Clear();
		triangles.Clear();

		// push the tip vertex
		vertices.Add(vineSkeleton.Last().startPoint + vineSkeleton.Last().nodeRay);
		Debug.Log("tip: " + vertices[0]);

		// push vertices for each ring

		for (int node = 0; node < vineSkeleton.Count; node++)
		{
			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = vineSkeleton[node].radius * Mathf.Cos(angle);
				float v_z = vineSkeleton[node].radius * Mathf.Sin(angle);
				float v_y = 0;

				Vector3 relativeVec = new Vector3(v_x, v_y, v_z);

				vertices.Add(vineSkeleton[node].startPoint + relativeVec);
			}
		}

		mesh.vertices = vertices.ToArray();

		// now, define how to triangles are to be drawn between these vertices

		for (int node = 0; node < vineSkeleton.Count; node++)
		{
			for (int faceNum = 0; faceNum < res; faceNum++)
			{
				//check if we're at a segment, or at the point
				if (node < vineSkeleton.Count - 1)
				{
					// this is a segment.
					// we will draw two triangles for each rectangular face created between this ring and the next ring

					int bottomLeft = (node * res) + faceNum + 1;
					int bottomRight = (node * res) + ((faceNum + 1) % res) + 1;
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
				else
				{
					// this is the point.
					// we will draw one triangle for each face between this ring and the tip

					int bottomLeft = (node * res) + faceNum + 1;
					int bottomRight = (node * res) + ((faceNum + 1) % res + 1);
					int top = 0;

					// add the triangle's vertices
					triangles.Add(bottomLeft);
					triangles.Add(bottomRight);
					triangles.Add(top);
				}
			}
		}

		mesh.triangles = triangles.ToArray();

	}
	

	private void addNewSegment()
	{
		
	}
}