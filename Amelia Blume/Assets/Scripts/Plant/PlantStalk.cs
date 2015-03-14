using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/*
Class Description
*/

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlantStalk : MonoBehaviour
{
	public class StalkNode {
		public float radius;
		public float length;
		public Vector3 startPoint;
		public Vector3 direction;

		public StalkNode(float rad, float magnitude, Vector3 start, Vector3 normalizedRay)
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

		public Vector3 getNodeEndPoint()
		{
			return startPoint + getNodeRay();
		}
	}

	public List<StalkNode> skeleton;

	private int resolution = 8;
	private float initialRadius = 0.05f;
	private float initialSegLength = 0.3f;
	private float maxSegLength = 0.3f;


	private float ringRadians;

	public float length;

	public bool isGrowing = false;
	private float growthRate = 0.5f;
	public float lengthGoal;
	private float growthStart;


	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;

	private Mesh mesh;
	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;
	private Material stalkMat;

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		stalkMat = Resources.Load("Materials/VineGreen", typeof(Material)) as Material;
		meshRenderer.material = stalkMat;

		_transform = transform;

		ringRadians = 2 * Mathf.PI / resolution;

		growthStart = initialSegLength;

		lengthGoal = 1f;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		skeleton = new List<StalkNode>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "PlantStalk";

		createInitialStalkSkeleton();
		printSkeletonInfo();
	}

	void Update()
	{
		length = getTotalLength();

		bool wasGrowing = isGrowing;
		// update skeleton structure (such as adding a new segment)
		if (getTotalLength() < lengthGoal)
		{
			growStalk();

			if (!wasGrowing)
			{
				isGrowing = true;
			}
		}
		else if (wasGrowing)
		{
			isGrowing = false;
		}

	}

	private void createInitialStalkSkeleton()
	{
		skeleton.Add(new StalkNode(initialRadius, 0, Vector3.zero, Vector3.up));
		createMesh();
	}

	private void growStalk()
	{
		// Extend the length of the stalk.
		// The segment before the tip ring will be extended. If it reaches its max length,
		// then a new segment will be added, and the overflow growth distance
		// will be its initial length.

		float newGrowth = (lengthGoal - growthStart) * growthRate * Time.deltaTime;
		int growIndex = skeleton.Count - 2;

		// should probably throw an error if newGrowth > maxSegLength
		if (newGrowth > maxSegLength)
		{
			Debug.Log("Whoops, newGrowth > maxSegLength in growStalk(). We should do something to handle this case.");
		}

		// trim the new growth if our stalk is overshooting the total length goal
		if (getTotalLength() + newGrowth > lengthGoal)
		{
			newGrowth = lengthGoal - getTotalLength();
			growthStart = lengthGoal;
		}

		// if the only segment is the tip segment, then we need to start fresh on a new one.
		if (skeleton.Count == 1)
		{
			addSegment(initialRadius, newGrowth, skeleton.Last().direction);
		}
		else
		{
			// we're not editing the very last segment, but the one right before it.
			// this is to maintain constant tip length. Otherwise growth will look funky.
			float currentLength = skeleton[growIndex].length;
			float newSegLength = currentLength + newGrowth;

			if (newSegLength < maxSegLength)
			{
				skeleton[growIndex].length = newSegLength;
			}
			else
			{
				skeleton[growIndex].length = maxSegLength;
				float overflow = newSegLength - maxSegLength;
				addSegment(initialRadius, overflow, skeleton.Last().direction);
			}
		}

		updateSkeleton(skeleton);
	}

	private void updateSkeleton(List<StalkNode> s)
	{
		// Iterate through all the nodes and make sure the start points correspond 
		// to the ends of the previous nodes.
		if (s.Count > 1)
		{
			for (int node = 1; node < s.Count; node++)
			{
				s[node].startPoint = s[node - 1].startPoint + s[node - 1].getNodeRay();
			}
		}
		
		updateMesh();
	}

	// createMesh() is only called once, at the beginning.
	// vertices[] and triangles[] are cleared and started from scratch.
	private void createMesh()
	{

		// maybe do some error handling here to make sure that there is at least one segment!

		int res = resolution;

		// just in case, clear things that should already be empty
		mesh.Clear();
		vertices.Clear();
		uvs.Clear();
		triangles.Clear();

		// push the tip vertex
		vertices.Add(skeleton.Last().startPoint + skeleton.Last().getNodeRay());
		Debug.Log("tip: " + vertices[0]);

		// push vertices for each ring

		for (int node = 0; node < skeleton.Count; node++)
		{
			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = skeleton[node].radius * Mathf.Cos(angle);
				float v_z = skeleton[node].radius * Mathf.Sin(angle);
				float v_y = 0;

				Vector3 relativeVec = new Vector3(v_x, v_y, v_z);

				vertices.Add(skeleton[node].startPoint + relativeVec);
				Debug.Log("ringVert: " + vertices[ringVert + 1 + (node * res)]);
			}
		}

		mesh.vertices = vertices.ToArray();

		// next, define the uv coordinates for the vertices that were created

		for (int vert = 0; vert < vertices.Count; vert++)
		{
			uvs.Add(new Vector2(0,0));
		}

		mesh.uv = uvs.ToArray();

		// now, define how to triangles are to be drawn between these vertices

		for (int node = 0; node < skeleton.Count; node++)
		{
			for (int faceNum = 0; faceNum < res; faceNum++)
			{
				//check if we're at a segment, or at the point
				if (node < skeleton.Count - 1)
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
		int res = resolution;

		// no need to clear mesh, vertices, or triangles.
		// we're just going to append to vertices,
		// and modify the tail end of triangles

		// first, add the new vertices.
		// usually we're just adding one segment at a time,
		// but we should be able to handle any number of new segments

		int prevSegCount = (mesh.vertices.Length - 1) / res;
		int newSegCount = skeleton.Count;

		for (int node = prevSegCount; node < newSegCount; node++)
		{
			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = skeleton[node].radius * Mathf.Cos(angle);
				float v_z = skeleton[node].radius * Mathf.Sin(angle);
				float v_y = 0;

				Vector3 relativeVec = new Vector3(v_x, v_y, v_z);

				vertices.Add(skeleton[node].startPoint + relativeVec);
				uvs.Add(new Vector2(0,0));
			}
		}

		// also, we need to move the tip point to it's new location.
		Vector3 newTip = skeleton.Last().startPoint + skeleton.Last().getNodeRay();
		vertices[0] = newTip;

		mesh.vertices = vertices.ToArray();
		mesh.uv = uvs.ToArray();

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
				if (node < skeleton.Count - 1)
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

	// updateMesh() is called whenever existing mesh vertices need to be transformed.
	// vertices[] is cleared and started from scratch, but triangles[] is left untouched.
	private void updateMesh()
	{
		float res = resolution;
		// when the stalk moves, pretty much all vertices are subject to change.
		// it's easier just to clear it and start from scratch.
		vertices.Clear();

		string debugString = "Node rotation info";

		// push the tip vertex
		vertices.Add(skeleton.Last().startPoint + skeleton.Last().getNodeRay());

		// push vertices for each ring

		for (int node = 0; node < skeleton.Count; node++)
		{
			// determine ring rotation based on the angle between the nodes it connects.
			// but skip the first ring, it should fall on the local Vector3.up plane

			debugString += "\n\tNode " + node;

			Vector3 bisectAxis = Vector3.up;
			Vector3 prevSegAxis = Vector3.up;
			float bottomAngle = 0f;
			float bisectAngle = 0f;

			if (node > 0)
			{
				bisectAxis = Vector3.Cross(skeleton[node-1].direction, skeleton[node].direction).normalized;
				bisectAngle = Vector3.Angle(skeleton[node-1].direction, skeleton[node].direction) / 2f;
				bottomAngle = Vector3.Angle(skeleton[node-1].direction, Vector3.up);
				prevSegAxis = Vector3.Cross(skeleton[node-1].direction, Vector3.up);
			}			

			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = skeleton[node].radius * Mathf.Cos(angle);
				float v_z = skeleton[node].radius * Mathf.Sin(angle);
				float v_y = 0;

				// ring is first initialized to fall on the Vecotr3.up plane.
				// now, rotate the rings to fall on the skeleton[node-1].direction plane.

				Vector3 relativeVec = new Vector3(v_x, v_y, v_z);

				// debugString += "\n\t\t\tringVert before rotation: " + relativeVec.ToString("F8");

				relativeVec = Quaternion.AngleAxis(-bottomAngle, prevSegAxis) * relativeVec;

				// debugString += "\n\t\t\tringVert after rotation: " + relativeVec.ToString("F8");

				// from here, we want to rotate the ring halfway to the skeleton[node].direction plane

				relativeVec = Quaternion.AngleAxis(bisectAngle, bisectAxis) * relativeVec;

				vertices.Add(skeleton[node].startPoint + relativeVec);
			}
		}

		mesh.vertices = vertices.ToArray();
		//Debug.Log(debugString);
		//Debug.Break();
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

		skeleton.Last().length = magnitude;
		StalkNode newNode = new StalkNode(rad, 0, skeleton.Last().startPoint + skeleton.Last().getNodeRay(), direction);
		skeleton.Add(newNode);

		expandMesh();
	}

	public float getTotalLength()
	{
		if (skeleton != null)
		{
			float len = 0f;

			for (int node = 0; node < skeleton.Count; node++)
			{
				len += skeleton[node].length;
			}
			return len;
		}
		else
		{
			Debug.Log("Can not get length of plant stalk before it's been created!");

			return 0f;
		}
			
	}

	public void setGrowthInfo(float goal, float rate)
	{
		lengthGoal = goal;
		growthRate = rate;
	}

	public void printSkeletonInfo()
	{
		string stalkInfo = "Number of nodes: " + skeleton.Count
						   + "\nStalk Length: " + getTotalLength();

		string nodeInfo = "";

		for (int node = 0; node < skeleton.Count; node++)
		{
			nodeInfo += "Node #" + node + "\n";
			nodeInfo += "\tStart: " + skeleton[node].startPoint + "\n";
			nodeInfo += "\tEnd: " + skeleton[node].getNodeEndPoint() + "\n";
		}

		Debug.Log(stalkInfo + "\n" + nodeInfo);
	}

}
