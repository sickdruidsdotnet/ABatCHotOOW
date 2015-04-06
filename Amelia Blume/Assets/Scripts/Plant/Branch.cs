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
	public float lengthGoal;
	public float branchMaturity;

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

		skeleton.Add(new BranchNode(initialRadius, length, startPoint, direction));

		branchMaturity = Mathf.Clamp01(getLength() / lengthGoal);
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