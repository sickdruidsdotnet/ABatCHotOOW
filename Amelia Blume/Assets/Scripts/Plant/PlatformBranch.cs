using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

// Don't extend MonoBehavious because this script will never be attached to a GameObject.
// It's simply an Object that will be used by another script.
public class PlatformBranch : Branch
{
	
	public PlatformBranch(TreePlant_Procedural.TreeSettings ts, Vector3 start, Vector3 dir, float tms)
	{
		// since there is no parent branch, we need a start point for this branch

		skeleton = new List<BranchNode>();

		parent = null;
		parentNode = -1;
		children = new List<Branch>();
		depth = 0;
		treeSettings = ts;
		startPoint = start;
		direction = dir;
		trajectory = direction;
		maxNodeAngle = 5f;
		lengthGoal = treeSettings.platformMaxLength;
		thickness = 0;
		maxDepth = treeSettings.platformMaxDepth;

		// only do this for the trunk
		widthGoal = treeSettings.treeMaxWidth;

		skeleton.Add(new BranchNode(thickness, 0, startPoint, direction));

		treeMaturityStart = tms;
	}

	public PlatformBranch(TreePlant_Procedural.TreeSettings ts, Branch parentBranch, Vector3 dir, float tms, int node = -1)
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
		treeSettings = ts;
		startPoint = parent.skeleton[parentNode].startPoint;
		direction = dir;

		// determine trajectory
		Vector3 crossWith = Vector3.up;
		if (crossWith == treeSettings.treeTrajectory)
		{
			crossWith = Vector3.right;
		}
		Vector3 axis = Vector3.Cross(direction, treeSettings.treeTrajectory);
		axis = Quaternion.AngleAxis(Random.Range(0, 360f), treeSettings.treeTrajectory) * axis;
		float rotAmount = Random.Range(0, treeSettings.branchTrajectoryNoise);

		trajectory = Quaternion.AngleAxis(rotAmount, axis) * treeSettings.treeTrajectory;

		// determine maxNodeAngle
		float variedWeight = Random.Range(ts.branchTrajectoryWeight - ts.branchTrajectoryWeight * ts.branchTrajectoryWeightVariation,
		                                  ts.branchTrajectoryWeight + ts.branchTrajectoryWeight * ts.branchTrajectoryWeightVariation);
		maxNodeAngle = ts.branchNodeMaxAngle * variedWeight;

		// determine lengthGoal
		float plr = parent.getLengthRemaining(parentNode);
		lengthGoal = Random.Range(0.5f * treeSettings.platformMaxChildLength, 1.2f * treeSettings.platformMaxChildLength);
		thickness = treeSettings.branchMaxWidth / (depth + 1);
		maxDepth = treeSettings.platformMaxDepth;

		skeleton.Add(new BranchNode(thickness, 0, startPoint, direction));

		// branchMaturity = Mathf.Clamp01(getLength() / lengthGoal);
		treeMaturityStart = tms;
	}

	public override void UpdateBranch(float treeMaturity)
	{
		currentTreeMaturity = treeMaturity;
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

		updateSkeleton(skeleton);
	}
	
	protected override void addSegment(float rad, float magnitude, Vector3 direction)
	{
		// since the tip is always of uniform length, we are actually adding a new tip,
		// and shrinking the previous end segment. It can now grow to its full length,
		// and then the process will start again.

		if (magnitude > treeSettings.branchSegLength)
		{
			Debug.Log("Whoops, magnitude > treeSettings.branchSegLength in PlatformBranch::addSegment(). We should do something to handle this case.");
		}
		float tipLength = skeleton.Last().length;
		skeleton.Last().length = magnitude;
		BranchNode newNode = new BranchNode(rad, tipLength, skeleton.Last().startPoint + skeleton.Last().getNodeRay(), direction);
		skeleton.Add(newNode);

		if (Random.Range(0, 1f) < treeSettings.maxNodeChanceToBranch
			&& depth < (maxDepth - 1)
			&& skeleton.Count > 2)
		{
			growRandomChildren(skeleton.Count - 2);
		}

		//expandMesh();
	}

	protected override void growRandomChildren(int parentBranchNode)
	{
		int numChildren = Random.Range(1, treeSettings.maxNumNodeBranches);
		float angleStart = Random.Range(0, 360);
		float branchAngle = Random.Range(treeSettings.minBranchAngle, treeSettings.maxBranchAngle);

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

	

	public override void addChild(Vector3 dir, int node = -1)
	{
		PlatformBranch newChild = new PlatformBranch(treeSettings, (Branch)this, dir, currentTreeMaturity, node);
		children.Add(newChild);
	}

	protected override void updateSkeleton(List<BranchNode> skel)
	{
		/*
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
		*/

		// iterate through all nodes in the skeleton
		for (int n = 0; n < skel.Count; n++)
		{
			////////////////////////////////////////////////////////////////////////////
			// Make sure all start points correspond to the end of the previous nodes //
			////////////////////////////////////////////////////////////////////////////

			// node 0 starts at the branch's start point
			if (n == 0) {skel[0].startPoint = startPoint;}
			else if (skel.Count > 1)
			{
				skel[n].startPoint = skel[n-1].getNodeEndPoint();
			}

			/////////////////////////////////////////////////////////////////////////////
			//         Update each node's radius to match the branch shape        ///////
			/////////////////////////////////////////////////////////////////////////////

			skel[n].radius = branchWidthFunction(getNodeLocation(n)) * getBranchThickness();

		}
	}

	public override float getBranchThickness()
	{
		// this will be the thickness at the base of the branch
		return Mathf.Min(thickness, treeSettings.platformMaxThickness);
	}
}