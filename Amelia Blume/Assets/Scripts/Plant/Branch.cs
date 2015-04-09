using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Don't extend MonoBehavious because this script will never be attached to a GameObject.
// It's simply an Object that will be used by another script.
public class Branch
{
	public class BranchNode
	{
		public float radius;
		public float length;
		public Vector3 startPoint;
		public Vector3 direction;

		public BranchNode(float rad, float magnitude, Vector3 start, Vector3 normalizedRay)
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

	public List<BranchNode> skeleton;

	public Branch parent;
	public int parentNode;
	public List<Branch> children;
	private int depth;
	public Vector3 startPoint;
	public Vector3 direction;
	public float branchMaturity;
	public float lengthGoal;
	private float growthRate = 0.05f;
	private float growthStart = 0;
	public bool isGrowing = false;
	private float initialRadius = 0.02f;
	public float chanceToBranch = 0.25f;
	public int randomBranchingFactor = 3;
	public float randomBranchAngleFactor = 30f;

	private TreePlant_Procedural.TreeSettings treeSettings;

	// "trunk" branch constructor
	public Branch(Vector3 start, Vector3 dir, TreePlant_Procedural.TreeSettings ts)
	{
		// since there is no parent branch, we need a start point for this branch

		skeleton = new List<BranchNode>();

		parent = null;
		parentNode = -1;
		children = new List<Branch>();
		depth = 0;
		startPoint = start;
		direction = dir;
		treeSettings = ts;
		lengthGoal = treeSettings.branchMaxLength / (depth + 1);

		skeleton.Add(new BranchNode(treeSettings.branchMinWidth, 0, startPoint, direction));
	}

	// standard branch constructor
	public Branch(Branch parentBranch, Vector3 dir, int node = -1, TreePlant_Procedural.TreeSettings ts)
	{
		// parentBranch determines what branch this branch will... branch... off of.
		// node determines where along the parent branch this branch will protrude from
		// we determine the actual start point from the parent branch's node

		skeleton = new List<BranchNode>();

		parent = parentBranch;
		parentNode = node;

		// -1 is code for "the end of the branch"
		if (parentNode == -1)
		{
			int count = parent.skeleton.Count;
			if (count > 1)
			{
				parentNode = count - 2;
			}
			else
			{
				parentNode = 0;
			}
		}

		children = new List<Branch>();
		depth = parent.getDepth() + 1;
		startPoint = parent.skeleton[parentNode].getNodeEndPoint();
		direction = dir;
		treeSettings = ts;
		lengthGoal = treeSettings.branchMaxLength / (depth + 1);

		skeleton.Add(new BranchNode(treeSettings.branchMinWidth, 0, startPoint, direction));

		branchMaturity = Mathf.Clamp01(getLength() / lengthGoal);
	}

	public void Update()
	{
		bool wasGrowing = isGrowing;
		// update vine skeleton structure (such as adding a new segment)
		if (getLength() < lengthGoal)
		{
			grow();
			isGrowing = true;
		}
		else if (wasGrowing)
		{
			isGrowing = false;
		}
	}

	private void grow()
	{
		// Extend the length of the branch.
		// The segment before the tip ring will be extended. If it reaches its max length,
		// then a new segment will be added, and the overflow growth distance
		// will be its initial length.

		float newGrowth = (lengthGoal - growthStart) * treeSettings.branchGrowthRate * Time.deltaTime;
		int growIndex = skeleton.Count - 2;

		// should probably throw an error if newGrowth > treeSettings.branchSegLength
		if (newGrowth > treeSettings.branchSegLength)
		{
			Debug.Log("Whoops, newGrowth > treeSettings.branchSegLength in grow(). We should do something to handle this case.");
		}

		// trim the new growth if our vine is overshooting the total length goal
		if (getLength() + newGrowth > lengthGoal)
		{
			newGrowth = lengthGoal - getLength();
			growthStart = lengthGoal;
		}

		// if the only segment is the tip segment, then we need to start fresh on a new one.
		if (skeleton.Count == 1)
		{
			addSegment(treeSettings.branchMinWidth, newGrowth, skeleton.Last().direction);
			//Debug.Log("Creating first non-tip segment");
		}
		else
		{
			// we're not editing the very last segment, but the one right before it.
			// this is to maintain constant tip length. Otherwise growth will look funky.
			float currentLength = skeleton[growIndex].length;
			float newSegLength = currentLength + newGrowth;

			if (newSegLength < treeSettings.branchSegLength)
			{
				skeleton[growIndex].length = newSegLength;
			}
			else
			{
				skeleton[growIndex].length = treeSettings.branchSegLength;
				float overflow = newSegLength - treeSettings.branchSegLength;
				addSegment(initialRadius, overflow, skeleton.Last().direction);
				//Debug.Log("Segment overflow (" + newSegLength + "). segment " + growIndex + " maxed out at " + skeleton[growIndex].length + ", so a new node is created with length " + overflow);
			}
		}

		updateSkeleton(skeleton);
	}

	private void addSegment(float rad, float magnitude, Vector3 direction)
	{
		// since the tip is always of uniform length, we are actually adding a new tip,
		// and shrinking the previous end segment. It can now grow to its full length,
		// and then the process will start again.

		if (magnitude > treeSettings.branchSegLength)
		{
			Debug.Log("Whoops, magnitude > treeSettings.branchSegLength in addSegment(). We should do something to handle this case.");
		}
		float tipLength = skeleton.Last().length;
		skeleton.Last().length = magnitude;
		BranchNode newNode = new BranchNode(rad, tipLength, skeleton.Last().startPoint + skeleton.Last().getNodeRay(), direction);
		skeleton.Add(newNode);

		if (Random.Range(0, 1f) < treeSettings.maxNodeChanceToBranch && depth < 3)
		{
			growRandomChildren(skeleton.Count - 2);
		}

		//expandMesh();
	}

	private void growRandomChildren(int parentBranchNode)
	{
		int numChildren = Random.Range(1, treeSettings.maxNumNodeBranches);
		float angleStart = Random.Range(0, 360);
		float branchAngle = Random.Range(5f, treeSettings.maxBranchAngle);

		for (int b = 0; b < numChildren; b++)
		{
			Vector3 direction = skeleton[parentBranchNode].direction;
			Vector3 crossWith = Vector3.up;
			if (crossWith == direction)
			{
				crossWith = Vector3.right;
			}
			Vector3 axis = Vector3.Cross(direction, crossWith);
			axis = Quaternion.AngleAxis((360f * (float)b / (float)numChildren) + angleStart, direction) * axis;

			direction = Quaternion.AngleAxis(branchAngle, axis) * direction;

			addChild(dir : direction);
		}
	}

	private void updateSkeleton(List<BranchNode> b)
	{
		// Iterate through all the nodes and make sure the start points correspond 
		// to the ends of the previous nodes.
		b[0].startPoint = startPoint;

		if (b.Count > 1)
		{
			for (int node = 1; node < b.Count; node++)
			{
				b[node].startPoint = b[node - 1].startPoint + b[node - 1].getNodeRay();
			}
		}
		
		//updateMesh();
	}

	public Branch getParent()
	{
		// getter
		return parent;
	}

	public void setParent(Branch parentNode)
	{
		// setter
		parent = parentNode;
	}

	public List<Branch> getChildren()
	{
		// getter
		return children;
	}

	public void addChild(Vector3 dir, int node = -1, float length = 0)
	{
		Branch newChild = new Branch(this, dir, node : node, length : length);
		children.Add(newChild);
	}

	public int getDepth()
	{
		return depth;
	}

	public float getLength()
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
			Debug.Log("Can not get length of branch before it's been created!");

			return 0f;
		}
	}
}