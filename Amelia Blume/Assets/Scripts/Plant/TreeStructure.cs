using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

class TreeStructure : MonoBehaviour
{
	public Branch trunk;

	public int resolution = 6;
	public static float initialRadius = 0.02f;
	public float maxRadius = 0.02f;
	public float initialSegLength = 0f;
	public float maxSegLength = 0.1f;
	public float crookedFactor = 10f;

	private float ringRadians;

	public float length;

	public bool isGrowing = false;
	public bool debugDoneGrowing = false;
	public float growthRate = 0.1f;
	public float lengthGoal = 1f;
	public float growthStart;

	// debug draw values
	private float debugSphereSize = 0.01f;
	private Color debugColor = Color.red;

	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;

	private Mesh mesh;
	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();

		_transform = transform;

		ringRadians = 2 * Mathf.PI / resolution;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "TreeStructure";

		createInitialTreeSkeleton();
	}

	void Update()
	{
		/*
		length = getTotalLength();

		bool wasGrowing = isGrowing;
		// update skeleton structure (such as adding a new segment)
		if (getTotalLength() < lengthGoal)
		{
			growTree();

			if (!wasGrowing)
			{
				isGrowing = true;
			}
		}
		else if (wasGrowing)
		{
			isGrowing = false;
			debugDoneGrowing = true;
			//printSkeletonInfo();
		}
		*/

		// update each Branch
		updateBranches(trunk);
		updateTreeSkeleton(trunk);

	}

	// recursively call Update() on all branches. This is not automatic since they are not Script Components
	private void updateBranches(Branch b)
	{
		b.Update();

		foreach (Branch c in b.getChildren())
		{
			updateBranches(c);
		}
	}

	private void createInitialTreeSkeleton()
	{
		trunk = new Branch(Vector3.zero, Vector3.up, initialRad : 0.02f, maxLength : 1.0f);

		//createMesh();
	}

	private void growTree()
	{}

	private void updateTreeSkeleton(Branch b)
	{
		if (b.getParent() != null)
		{
			// reposition the branch so it is still attached to the correct parent node.
			b.startPoint = b.getParent().skeleton[b.parentNode].getNodeEndPoint();
		}

		foreach(Branch c in b.getChildren())
			{
				updateTreeSkeleton(c);
			}
	}

	/*
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
		//Debug.Log("tip: " + vertices[0]);

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
				//Debug.Log("ringVert: " + vertices[ringVert + 1 + (node * res)]);
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
	*/

	/*
	public float getTotalLength()
	{}
	*/

	void OnDrawGizmos()
	{
		// Avoid NPE in the editor when the game isn't running,
		// (and therefore the object hasn't been initialized).
		if (_transform)
		{
			Vector3 mPos = _transform.position;

			if (trunk != null)
			{
				// recursively draw tree
				drawTreeSkeletonGizmos(trunk, mPos);
			}
		}
			
	}

	void drawTreeSkeletonGizmos(Branch b, Vector3 mPos)
	{
		// recursive draw function

		for (int node = 0; node < b.skeleton.Count; node++)
		{
			Vector3 nStart = b.skeleton[node].startPoint;
			Vector3 nEnd = b.skeleton[node].getNodeEndPoint();

			// Draw segment endpoints with red spheres
			Gizmos.color = Color.red;
			Gizmos.DrawSphere(mPos + nStart, debugSphereSize);

			// Draw segments with lines. Alternate colors for each segment,
			// but keep the tip segment red (or it swaps colors weirdly)
			if (node == b.skeleton.Count - 1)
			{
				Gizmos.color = Color.red;
			}
			else if (node % 2 == 0)
			{
				Gizmos.color = Color.blue;
			}
			else
			{
				Gizmos.color = Color.white;
			}
			Gizmos.DrawLine(mPos + nStart, mPos + nEnd);
		}

		// Draw the tip as another segment endpoint
		Gizmos.color = Color.red;
		Gizmos.DrawSphere(mPos + b.skeleton.Last().getNodeEndPoint(), debugSphereSize);

		foreach (Branch childBranch in b.children)
		{
			drawTreeSkeletonGizmos(childBranch, mPos);
		}
	}
}