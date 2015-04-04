using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class TreePlant_Procedural : MonoBehaviour
{
	public List<TreeNode> skeleton;

	public int resolution = 8;
	public float initialRadius = 0.02f;
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


	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;

	private Mesh mesh;
	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;

	public class TreeNode {

		public TreeNode parent;
		public List<TreeNode> children;
		public float radius;
		public float length;
		public Vector3 startPoint;
		public Vector3 direction;

		public StalkNode(TreeNode parentNode, float rad, float magnitude, Vector3 normalizedRay)
		{
			// constructor
			parent = parentNode;
			children = new List<TreeNode>();
			radius = rad;
			length = magnitude;
			startPoint = Vector3.zero;
			direction = normalizedRay;

			if (parent != null)
			{
				startPoint = parent.getNodeEndPoint();
			}
		}

		public TreeNode getParent()
		{
			// getter
			return parent;
		}

		public void setParent(TreeNode parentNode)
		{
			// setter
			parent = parentNode;
		}

		public List<TreeNode> getChildren()
		{
			// getter
			return children;
		}

		public void addChild(float rad, float magnitude, Vector3 normalizedRay)
		{
			StalkNode newChild = new StalkNode(this, rad, magnitude, normalizedRay);
			children.Add(newChild);
		}

		public Vector3 getNodeRay()
		{
			// getter
			return direction * length;
		}

		public Vector3 getNodeEndPoint()
		{
			// getter
			return startPoint + getNodeRay();
		}
	}

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();

		_transform = transform;

		ringRadians = 2 * Mathf.PI / resolution;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		skeleton = new List<TreeNode>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "TreeStructure";

		createInitialTreeSkeleton();
	}

	void Update()
	{
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
	}

	private void createInitialTreeSkeleton()
	{
		skeleton.Add(new TreeNode(null, initialRadius, 0, Vector3.up));
		createMesh();
	}

	private void growTree()
	{}

	public float getTotalLength()
	{}
}