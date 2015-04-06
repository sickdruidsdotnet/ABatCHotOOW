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
	public Vector3 startPoint;
	public Vector3 direction;
	public float branchMaturity;
	public float lengthGoal;
	private float growthRate = 0.05f;
	private float growthStart;
	public bool isGrowing = false;
	private float maxSegLength = 0.2f;

	// "trunk" branch constructor
	public Branch(Vector3 start, Vector3 dir, float initialRadius = 0.02f, float maxLength = 1.0f, float length = 0)
	{
		// since there is no parent branch, we need a start point for this branch

		skeleton = new List<BranchNode>();

		parent = null;
		parentNode = -1;
		children = new List<Branch>();
		startPoint = start;
		direction = dir;
		lengthGoal = maxLength;
		growthStart = length;

		skeleton.Add(new BranchNode(initialRadius, length, startPoint, direction));
	}

	// standard branch constructor
	public Branch(Branch parentBranch, Vector3 dir, float initialRadius = 0.02f, int node = -1, float maxLength = 1.0f, float length = 0)
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
		startPoint = parent.skeleton[parentNode].getNodeEndPoint();
		direction = dir;
		lengthGoal = maxLength;
		growthStart = length;

		skeleton.Add(new BranchNode(initialRadius, length, startPoint, direction));

		branchMaturity = Mathf.Clamp01(getLength() / lengthGoal);
	}

	public void Update()
	{
		bool wasGrowing = isGrowing;
		// update vine skeleton structure (such as adding a new segment)
		if (getTotalLength() < lengthGoal)
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

		float newGrowth = (lengthGoal - growthStart) * growthRate * Time.deltaTime;
		int growIndex = skeleton.Count - 2;

		// should probably throw an error if newGrowth > maxSegLength
		if (newGrowth > maxSegLength)
		{
			Debug.Log("Whoops, newGrowth > maxSegLength in grow(). We should do something to handle this case.");
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
			addSegment(initialRadius, newGrowth, skeleton.Last().direction);
			//Debug.Log("Creating first non-tip segment");
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
				//Debug.Log("Segment overflow (" + newSegLength + "). segment " + growIndex + " maxed out at " + skeleton[growIndex].length + ", so a new node is created with length " + overflow);
			}
		}

		updateSkeleton(skeleton);
	}

	private void addSegment()
	{
		// steal from Vine
	}

	private void updateSkeleton()
	{
		// steal from Vine
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