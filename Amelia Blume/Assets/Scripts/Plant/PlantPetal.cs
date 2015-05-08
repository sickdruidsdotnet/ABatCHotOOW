using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using System;

/*
Class Description
*/

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class PlantPetal : MonoBehaviour
{
	public class PetalNode {
		public float width;
		public float length;
		public Vector3 startPoint;
		public Vector3 direction;

		public PetalNode(float wid, float magnitude, Vector3 start, Vector3 normalizedRay)
		{
			width = wid;
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

		public Vector3 getUpVector()
		{
			return Vector3.Cross(Vector3.forward, direction);
		}
	}

	public List<PetalNode> skeleton;

	public int stalkNode;
	public float growthAngle;

	public int resolution = 4; // DO NOT CHANGE THIS, IT WILL BREAK THE PETAL AND DO BAD THINGS.
	public float initialWidth = 0.1f;
	public float initialSegLength = 0f;
	public float maxSegLength = 0.05f;
	public float petalThickness = 0.01f;
	public float petalWidth = 0.08f;

	public bool bloomTrigger = false;
	public bool isBlooming = false;
	public float curlStart = 0f;
	public float curlBloom = 0f;
	public float bloomRate = 1.0f;

	public int colorHue = 200;
	public float colorSat = 0.9f;
	public float colorVal = 0.9f;

	public float length;

	public bool isGrowing = false;
	public float growthRate = 0.6f;
	public float lengthGoal;
	public float growthStart;


	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;

	private Mesh mesh;
	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;
	private Material petalMat;

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		petalMat = Resources.Load("Materials/LurePlantMagenta", typeof(Material)) as Material;
		meshRenderer.material = petalMat;

		float r, g, b;
		GetComponent<ColorConverter>().HsvToRgb(colorHue, colorSat, colorVal, out r, out g, out b);
		meshRenderer.material.color = new Color(r,g,b);

		growthStart = initialSegLength;

		lengthGoal = 0.2f;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		skeleton = new List<PetalNode>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "PlantPetal";

		createInitialPetalSkeleton();
	}

	public void setPetalInfo(int node, float angle, float cStart, float cBloom, float rate)
	{
		stalkNode = node;
		growthAngle = angle;
		curlStart = cStart;
		curlBloom = cBloom;
		bloomRate = rate;
	}

	void Update()
	{
		length = getTotalLength();

		bool wasGrowing = isGrowing;
		// update skeleton structure (such as adding a new segment)
		if (getTotalLength() < lengthGoal)
		{
			growPetal();

			if (!wasGrowing)
			{
				isGrowing = true;
			}
		}
		else if (wasGrowing)
		{
			isGrowing = false;
			//printSkeletonInfo();
			//updateMesh(true);
		}

		if (bloomTrigger)
		{
			bool wasBlooming = isBlooming;
			// check if we still need to bloom more.
			// this is harder than length, since it depends all node angles
			// make a function to handle this
			if (!isDoneBlooming())
			{
				bloomPetal();

				if (!wasBlooming)
				{
					isBlooming = true;
				}
			}
			else if (wasBlooming)
			{
				isBlooming = false;
				//printSkeletonInfo();
			}
		}

			

	}

	private void createInitialPetalSkeleton()
	{
		skeleton.Add(new PetalNode(initialWidth, 0, Vector3.zero, curlNode(Vector3.right, curlStart)));
		createMesh();
	}

	private void growPetal()
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
			Debug.Log("Whoops, newGrowth > maxSegLength in growPetal(). We should do something to handle this case.");
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
			addSegment(initialWidth, newGrowth, curlNode(skeleton.Last().direction, curlStart));
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
				addSegment(initialWidth, overflow, curlNode(skeleton.Last().direction, curlStart));
			}
		}

		updateSkeleton(skeleton);
	}

	private void bloomPetal()
	{
		string debugString = "Curling petal:";
		float amountToCurl = (curlBloom - curlStart) * bloomRate * Time.deltaTime;

		for (int node = 0; node < skeleton.Count; node++)
		{
			debugString += "\n\tNode " + node + ":";

			Vector3 curDirection = skeleton[node].direction;
			debugString += "\n\t\tcurDirection: " + curDirection.ToString("F4");

			Vector3 newDirection = curlNode(curDirection, amountToCurl * (node + 1));
			debugString += "\n\t\tnewDirection: " + newDirection.ToString("F4");

			debugString += "\n\t\tAmount Curled: " + Vector3.Angle(curDirection, newDirection);

			skeleton[node].direction = newDirection;
		}
		//Debug.Log(debugString);
		//printSkeletonInfo();
		updateSkeleton(skeleton);
		//Debug.Break();
	}

	private Vector3 curlNode(Vector3 inVec, float angle)
	{
		inVec = Quaternion.AngleAxis(angle, Vector3.forward) * inVec;

		return inVec;
	}

	private void updateSkeleton(List<PetalNode> s)
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
		//Debug.Log("tip: " + vertices[0]);

		// push vertices for each ring

		for (int node = 0; node < skeleton.Count; node++)
		{
			// define and push the four corners of this petal "ring"
			// for reference, we are "looking" in the direction of the node's direction vec

			Vector3 topLeft = skeleton[node].startPoint + new Vector3(0f, petalThickness / 2f, skeleton[node].width / 2f);
			Vector3 topRight = skeleton[node].startPoint + new Vector3(0f, petalThickness / 2f, -skeleton[node].width / 2f);
			Vector3 bottomRight = skeleton[node].startPoint + new Vector3(0f, -petalThickness / 2f, -skeleton[node].width / 2f);
			Vector3 bottomLeft = skeleton[node].startPoint + new Vector3(0f, -petalThickness / 2f, skeleton[node].width / 2f);

			vertices.Add(topLeft);
			vertices.Add(topRight);
			vertices.Add(bottomRight);
			vertices.Add(bottomLeft);

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
					triangles.Add(topLeft);
					triangles.Add(topRight);
					triangles.Add(bottomLeft);

					// add second triangle's vertices
					triangles.Add(topRight);
					triangles.Add(bottomRight);
					triangles.Add(bottomLeft);
				}
				else
				{
					// this is the point.
					// we will draw one triangle for each face between this ring and the tip

					int bottomLeft = (node * res) + faceNum + 1;
					int bottomRight = (node * res) + ((faceNum + 1) % res + 1);
					int top = 0;

					// add the triangle's vertices
					triangles.Add(top);
					triangles.Add(bottomRight);
					triangles.Add(bottomLeft);
				}
			}
		}

		mesh.triangles = triangles.ToArray();

		updateMesh();
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

			// define and push the four corners of this petal "ring"
			// for reference, we are "looking" in the direction of the node's direction vec

			Vector3 topLeft = skeleton[node].startPoint + new Vector3(0f, petalThickness / 2f, skeleton[node].width / 2f);
			Vector3 topRight = skeleton[node].startPoint + new Vector3(0f, petalThickness / 2f, -skeleton[node].width / 2f);
			Vector3 bottomRight = skeleton[node].startPoint + new Vector3(0f, -petalThickness / 2f, -skeleton[node].width / 2f);
			Vector3 bottomLeft = skeleton[node].startPoint + new Vector3(0f, -petalThickness / 2f, skeleton[node].width / 2f);

			vertices.Add(topLeft);
			uvs.Add(new Vector2(0,0));

			vertices.Add(topRight);
			uvs.Add(new Vector2(0,0));

			vertices.Add(bottomRight);
			uvs.Add(new Vector2(0,0));

			vertices.Add(bottomLeft);
			uvs.Add(new Vector2(0,0));
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
					triangles.Add(topLeft);
					triangles.Add(topRight);
					triangles.Add(bottomLeft);

					// add second triangle's vertices
					triangles.Add(topRight);
					triangles.Add(bottomRight);
					triangles.Add(bottomLeft);
				}
				else
				{
					// this is the point.
					// we will draw one triangle for each face between this ring and the tip

					int bottomLeft = (node * res) + faceNum + 1;
					int bottomRight = (node * res) + ((faceNum + 1) % res + 1);
					int top = 0;

					// add the triangle's vertices
					triangles.Add(top);
					triangles.Add(bottomRight);
					triangles.Add(bottomLeft);
				}
			}
		}

		mesh.triangles = triangles.ToArray();
	}

	// updateMesh() is called whenever existing mesh vertices need to be transformed.
	// vertices[] is cleared and started from scratch, but triangles[] is left untouched.
	private void updateMesh(bool printDebug = false)
	{
		// float res = resolution;
		// when the stalk moves, pretty much all vertices are subject to change.
		// it's easier just to clear it and start from scratch.
		vertices.Clear();

		string debugString = "Node rotation info";

		// push the tip vertex
		vertices.Add(skeleton.Last().startPoint + skeleton.Last().getNodeRay());

		// push vertices for each ring

		for (int node = 0; node < skeleton.Count; node++)
		{
			debugString += "\n\tNode " + node;

			// determine the width for this segment

			float nodeLoc = getNodeLocation(node);
			skeleton[node].width = petalWidthFunction(nodeLoc) * petalWidth;

			debugString += "\n\t\tnodeLoc: " + nodeLoc;
			debugString += "\n\t\twidth: " + skeleton[node].width;


			// determine ring rotation based on the angle between the nodes it connects.
			// but skip the first ring, it should fall on the local Vector3.up plane

			

			Vector3 bisectAxis = Vector3.up;
			Vector3 prevSegAxis = Vector3.up;
			float bottomAngle = 0f;
			float bisectAngle = 0f;

			if (node > 0)
			{
				bisectAxis = Vector3.Cross(skeleton[node-1].direction, skeleton[node].direction).normalized;
				bisectAngle = Vector3.Angle(skeleton[node-1].direction, skeleton[node].direction) / 2f;
				bottomAngle = Vector3.Angle(skeleton[node-1].direction, Vector3.right);
				prevSegAxis = Vector3.Cross(skeleton[node-1].direction, Vector3.up);

				debugString += "\n\t\tbisectAxis: " + bisectAxis;
				debugString += "\n\t\tbisectAngle: " + bisectAngle;
				debugString += "\n\t\tbottomAngle: " + bottomAngle;
				debugString += "\n\t\tprevSegAxis: " + prevSegAxis;

			}

			//create the "ring" around the origin

			Vector3 topLeft = new Vector3(0f, petalThickness / 2f, skeleton[node].width / 2f);
			Vector3 topRight = new Vector3(0f, petalThickness / 2f, -skeleton[node].width / 2f);
			Vector3 bottomRight = new Vector3(0f, -petalThickness / 2f, -skeleton[node].width / 2f);
			Vector3 bottomLeft = new Vector3(0f, -petalThickness / 2f, skeleton[node].width / 2f);

			// now rotate it properly.
			// First to the normal of the previous node's direction...

			topLeft = Quaternion.AngleAxis(bottomAngle, prevSegAxis) * topLeft;
			topRight = Quaternion.AngleAxis(bottomAngle, prevSegAxis) * topRight;
			bottomRight = Quaternion.AngleAxis(bottomAngle, prevSegAxis) * bottomRight;
			bottomLeft = Quaternion.AngleAxis(bottomAngle, prevSegAxis) * bottomLeft;

			// ...and now halfway to the normal of this node's direction. Split the difference.

			topLeft = Quaternion.AngleAxis(bisectAngle, bisectAxis) * topLeft;
			topRight = Quaternion.AngleAxis(bisectAngle, bisectAxis) * topRight;
			bottomRight = Quaternion.AngleAxis(bisectAngle, bisectAxis) * bottomRight;
			bottomLeft = Quaternion.AngleAxis(bisectAngle, bisectAxis) * bottomLeft;

			// Now add the ring to vertices after moving it to the proper position

			vertices.Add(skeleton[node].startPoint + topLeft);
			vertices.Add(skeleton[node].startPoint + topRight);
			vertices.Add(skeleton[node].startPoint + bottomRight);
			vertices.Add(skeleton[node].startPoint + bottomLeft);
		}

		mesh.vertices = vertices.ToArray();
		if (printDebug)
		{
			Debug.Log(debugString);
		}
		
		//Debug.Break();
	}

	private void addSegment(float wid, float magnitude, Vector3 direction)
	{
		// since the tip is always of uniform length, we are actually adding a new tip,
		// and shrinking the previous end segment. It can now grow to its full length,
		// and then the process will start again.

		if (magnitude > maxSegLength)
		{
			Debug.Log("Whoops, magnitude > maxSegLength in addSegment(). We should do something to handle this case.");
		}

		skeleton.Last().length = magnitude;
		PetalNode newNode = new PetalNode(wid, 0, skeleton.Last().startPoint + skeleton.Last().getNodeRay(), direction);
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

	private float getNodeLocation(int node)
	{
		// returns a value between 0 and 1 to represent the node's loaction along the petal's length.
		// 0 is the base, 1 is the tip.
		// this function is mainly used to determine the node's width.
		float petalLength = getTotalLength();
		float location = 0f;
		float nodeDistance = 0f;

		for (int n = 0; n < node; n++)
		{
			nodeDistance += skeleton[n].length;
		}

		location = nodeDistance / petalLength;

		return location;

	}

	public float petalWidthFunction(float x)
	{

		// x is a number between 0 and 1 defining it's position along the petal


		// Third Order Polynomial defining the petal's shape
		float y = 5.333f * (x*x*x) -11.886f * (x*x) + 5.952f * (x) + 0.614f;

		// since this defines only one side of the petal, mulitply by two to get full width

		return y;
	}

	public void setGrowthInfo(float goal, float rate)
	{
		lengthGoal = goal;
		growthRate = rate;
	}

	public void printSkeletonInfo()
	{
		string stalkInfo = "Number of petal nodes: " + skeleton.Count
						   + "\nPetal Length: " + getTotalLength();

		string nodeInfo = "";

		for (int node = 0; node < skeleton.Count; node++)
		{
			Vector3 prevNodeDirection = Vector3.right;
			Vector3 curNodeDirection = skeleton[node].direction;
			float angle;
			if (node > 0)
			{
				prevNodeDirection = skeleton[node - 1].direction;
			}

			angle = Vector3.Angle(prevNodeDirection, curNodeDirection);

			nodeInfo += "Node #" + node + "\n";
			nodeInfo += "\tStart: " + skeleton[node].startPoint + "\n";
			nodeInfo += "\tEnd: " + skeleton[node].getNodeEndPoint() + "\n";
			nodeInfo += "\tDirection: " + skeleton[node].direction + "\n";
			nodeInfo += "\tAngle: " + angle + "\n";
			nodeInfo += "\tLength: " + skeleton[node].length + "\n";
			nodeInfo += "\tWidth: " + skeleton[node].width + "\n";
		}

		Debug.Log(stalkInfo + "\n" + nodeInfo);
	}

	private bool isDoneBlooming()
	{
		// first, error check to make sure all nodes have the same rotation
		List<float> angles = new List<float>();
		for (int node = 0; node < skeleton.Count; node++)
		{
			Vector3 prevNodeDirection = Vector3.right;
			Vector3 curNodeDirection = skeleton[node].direction;
			float angle;
			if (node > 0)
			{
				prevNodeDirection = skeleton[node - 1].direction;
			}

			angle = Vector3.Angle(prevNodeDirection, curNodeDirection);

			angles.Add(angle);
		}

		// check to see if any elements on the list are not identical to the others
		// http://stackoverflow.com/questions/5307172/check-if-all-items-are-the-same-in-a-list
		// due to floating point errors, account for slight differences
		// http://stackoverflow.com/questions/3420812/how-do-i-find-if-two-variables-are-approximately-equals
		/*
		float tolerance = 0.0000001f;
		if (!angles.All(a => Math.Abs(a - angles.First()) < tolerance))
		{
			string debugString = "Angles are not identical. Could be a floating point error though.";
			
			for (int a = 0; a < angles.Count; a++)
			{
				debugString += "\n\tAngle " + a + ": " + angles[a].ToString("F20");
			}
			Debug.Log(debugString);
			Debug.Break();
		}
		*/

		// finally, check if we're done blooming
		// theoretically, we could be blooming in either direction.
		Vector3 goalDirection = curlNode(Vector3.right, curlBloom);
		Vector3 curDirection = skeleton[0].direction;
		Vector3 nextDirection = curlNode(skeleton[0].direction, (curlBloom - curlStart) * bloomRate * Time.deltaTime);
		float thisAngle = Vector3.Angle(curDirection, goalDirection);
		float nextAngle = Vector3.Angle(nextDirection, goalDirection);

		if (thisAngle < nextAngle)
		{
			return true;
		}
		else
		{
			return false;
		}
		/*
		if (curlStart < curlBloom)
		{
			if (angles[0] < curlBloom)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		else
		{
			if (angles[0] > curlBloom)
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		*/


	}

}
