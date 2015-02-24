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
		public float length;
		public Vector3 startPoint;
		public Vector3 direction;

		public VineNode(float rad, float magnitude, Vector3 start, Vector3 normalizedRay)
		{
			radius = rad;
			length = magnitude;
			startPoint = start;
			direction = normalizedRay;
		}

		public Vector3 getNodeRay()
		{
			return direction * length;
		}
	}

	public List<VineNode> vineSkeleton;

	public VineSettings vineSettings = new VineSettings();

	// number of segments, not including the tip.
	private int numSegments = 1;
	private float initialRadius = 0.1f;
	private float initialSegLength = 0.4f;
	private float tipLength = 0.5f;
	private float maxSegLength = 0.3f;

	private float growthRate = 0.05f;
	private float lengthGoal;
	private float growthStart;
	
	private int numRings;
	private int numFaces;
	private int numTriangles;
	private int numTriVerts;
	private float ringRadians;

	// debug draw values
	private float debugSphereSize = 1f;
	private Color debugColor = Color.red;

	private Mesh mesh;

	/* vertices has the following format:
	Vertex 0 is the tip.
	Vertices (VineNodeNum, resolution * VineNodeNum) correspond to the ring for each VineNode */
	private List<Vector3> vertices;
	private List<int> triangles;

	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;

	public bool pressedVineButton = false;

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

		lengthGoal = growthStart = initialSegLength;

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
		// update vine skeleton structure (such as adding a new segment)
		if (getTotalLength() < lengthGoal)
		{
			growVine();
		}

		// debug vine growth testing
		if (Input.GetButtonDown("GrowVineDebug") && !pressedVineButton)
		{
			setGrowthInfo(5.0f, 0.01f);
			pressedVineButton = true;
		}
		else
		{
			pressedVineButton = false;
		}

	}

	// debug drawing of the skeleton
	void OnDrawGizmos()
	{
		if (vineSkeleton != null)
		{
			Gizmos.color = debugColor;
			for (int node = 0; node < vineSkeleton.Count; node++)
			{
				Gizmos.DrawSphere(vineSkeleton[node].startPoint, debugSphereSize);
				Gizmos.DrawLine(vineSkeleton[node].startPoint, vineSkeleton[node].getNodeRay());
			}

			Gizmos.DrawSphere(vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay(), debugSphereSize);
		}
	}

	private void createInitialVineSkeleton()
	{
		vineSkeleton.Add(new VineNode(initialRadius, initialSegLength, Vector3.zero, Vector3.up));
		Debug.Log("Node 0 start: " + vineSkeleton[0].startPoint);

		createMesh();

	}

	private void growVine()
	{
		// Extend the length of the vine.
		// The segment before the tip ring will be extended. If it reaches its max length,
		// then a new segment will be added, and the overflow growth distance
		// will be its initial length.

		float newGrowth = (lengthGoal - growthStart) * growthRate * Time.deltaTime;
		int growIndex = vineSkeleton.Count - 2;

		// should probably throw an error if newGrowth > maxSegLength
		if (newGrowth > maxSegLength)
		{
			Debug.Log("Whoops, newGrowth > maxSegLength in growVine(). We should do something to handle this case.");
		}

		// trim the new growth if our vine is overshooting the total length goal
		if (getTotalLength() + newGrowth > lengthGoal)
		{
			newGrowth = lengthGoal - getTotalLength();
			growthStart = lengthGoal;
		}

		// if the only segment is the tip segment, then we need to start fresh on a new one.
		if (vineSkeleton.Count == 1)
		{
			addSegment(initialRadius, newGrowth, Vector3.up);
			Debug.Log("Creating first non-tip segment");
		}
		else
		{
			// we're not editing the very last segment, but the one right before it.
			// this is to maintain constant tip length. Otherwise growth will look funky.
			float currentLength = vineSkeleton[growIndex].length;
			float newSegLength = currentLength + newGrowth;

			if (newSegLength < maxSegLength)
			{
				vineSkeleton[growIndex].length = newSegLength;
			}
			else
			{
				vineSkeleton[growIndex].length = maxSegLength;
				float overflow = newSegLength - maxSegLength;
				addSegment(initialRadius, overflow, Vector3.up);
				Debug.Log("Segment overflow (" + newSegLength + "). segment " + growIndex + " maxed out at " + vineSkeleton[growIndex].length + ", so a new node is created with length " + overflow);
			}
		}

		updateSkeleton();
	}

	// createMesh() is only called once, at the beginning.
	// vertices[] and triangles[] are cleared and started from scratch.
	private void createMesh()
	{

		// maybe do some error handling here to make sure that there is at least one segment!

		int res = vineSettings.resolution;

		// just in case, clear things that should already be empty
		mesh.Clear();
		vertices.Clear();
		triangles.Clear();

		// push the tip vertex
		vertices.Add(vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay());
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

	// expandMesh() is called whenever a new segment is added.
	// vertices[] is added to, and triangles[] is manipulated.
	private void expandMesh()
	{
		int res = vineSettings.resolution;

		// no need to clear mesh, vertices, or triangles.
		// we're just going to append to vertices,
		// and modify the tail end of triangles

		// first, add the new vertices.
		// usually we're just adding one segment at a time,
		// but we should be able to handle any number of new segments

		int prevSegCount = (mesh.vertices.Length - 1) / res;
		int newSegCount = vineSkeleton.Count;

		for (int node = prevSegCount; node < newSegCount; node++)
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

		// also, we need to move the tip point to it's new location.
		Vector3 newTip = vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay();
		vertices[0] = newTip;

		mesh.vertices = vertices.ToArray();

		// next, we need to fix the triangles array.
		// the segment connected to the tip should now be connected to a newly created segment.
		// so, we'll delete the triangle information for that segment.

		// purge the triangles forming the tip,
		int numPointsToPurge = 3 * res;
		// but preserve the faces forming the previous segments
		int oldTipSegIndex = 3 * 2 * res * (prevSegCount - 1);

		triangles.RemoveRange(oldTipSegIndex, numPointsToPurge);

		for (int node = prevSegCount - 1; node < newSegCount; node++)
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

	
	private void updateSkeleton()
	{
		// Iterate through all the nodes and make sure the start points correspond 
		// to the ends of the previous nodes.
		if (vineSkeleton.Count > 1)
		{
			for (int node = 1; node < vineSkeleton.Count; node++)
			{
				vineSkeleton[node].startPoint = vineSkeleton[node - 1].startPoint + vineSkeleton[node - 1].getNodeRay();
			}
		}
		
		updateMesh();
	}

	// updateMesh() is called whenever existing mesh vertices need to be transformed.
	// vertices[] is cleared and started from scratch, but triangles[] is left untouched.
	private void updateMesh()
	{
		float res = vineSettings.resolution;
		// when the vine moves, pretty much all vertices are subject to change.
		// it's easier just to clear it and start from scratch.
		vertices.Clear();

		// push the tip vertex
		vertices.Add(vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay());

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
	}

	private void addSegment(float rad, float magnitude, Vector3 direction)
	{
		// since the tip is always of uniform length, we are actually adding a new tip,
		// and shrinking the previous end segment. It can now grow to its full length,
		// and then the process will start again.

		if (magnitude > maxSegLength)
		{
			Debug.Log("Whoops, magnitude > maxSegLength in addSegment(). We should do something to handle this case.");
		}

		vineSkeleton.Last().length = magnitude;
		VineNode newNode = new VineNode(rad, tipLength, vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay(), direction);
		vineSkeleton.Add(newNode);

		expandMesh();
	}

	/*

	PUBLIC FUNCTIONS

	These functions either:

	- GET information about this vine to be used outside of the Vine class, or
	- SET values in the Vine class that internal private functions will use to change the vine.

	These functions should never directly manipulate the mesh or vertices.
	*/
	public void setGrowthInfo(float goal, float rate)
	{
		lengthGoal = goal;
		growthRate = rate;
	}

	public float getTotalLength()
	{
		float len = 0f;

		for (int node = 0; node < vineSkeleton.Count; node++)
		{
			len += vineSkeleton[node].length;
		}

		return len;
	}
}