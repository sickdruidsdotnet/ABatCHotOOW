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
		public float targetHeight = 5f;
		public float targetSpeed = 10f;
		public float targetTrackRadius = 5f;
		public float targetAngle = 0f;
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

		public Vector3 getNodeEndPoint()
		{
			return startPoint + getNodeRay();
		}
	}

	public List<VineNode> vineSkeleton;

	public VineSettings vineSettings = new VineSettings();

	// number of segments, not including the tip.
	private int numSegments = 1;
	private float initialRadius = 0.05f;
	private float initialSegLength = 0.3f;
	private float tipLength = 0.3f;
	private float maxSegLength = 0.3f;

	private Color vineColor = Color.green;

	private float growthRate = 0.05f;
	public float lengthGoal = 0;
	private float growthStart;

	public float length;
	
	private int numRings;
	private int numFaces;
	private int numTriangles;
	private int numTriVerts;
	private float ringRadians;

	public GameObject vineTarget;

	// debug draw values
	private float debugSphereSize = 0.01f;
	private Color debugColor = Color.red;

	private Mesh mesh;

	/* vertices has the following format:
	Vertex 0 is the tip.
	Vertices (VineNodeNum, resolution * VineNodeNum) correspond to the ring for each VineNode */
	private List<Vector3> vertices;
	private List<int> triangles;
	private List<Vector2> uvs;

	private Transform _transform; // cached transform to increase speeds
	private MeshRenderer meshRenderer;
	private Material vineMat;

	public bool pressedVineButton = false;
	public bool isGrowing = false;

	int debugCount = 0;


	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		vineMat = Resources.Load("Materials/VineGreen", typeof(Material)) as Material;
		meshRenderer.material = vineMat;

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

		growthStart = initialSegLength;

		// initialize our mesh's data structures
		vertices = new List<Vector3>();
		triangles = new List<int>();
		uvs = new List<Vector2>();
		vineSkeleton = new List<VineNode>();

		GetComponent<MeshFilter>().mesh = mesh = new Mesh();
		mesh.Clear();
		mesh.name = "Vine";

		createInitialVineSkeleton();

		// initialize vine target
		vineTarget = new GameObject("VineTarget");
		vineTarget.transform.position = new Vector3(getTotalLength() / 2, getTotalLength() / 2, 0) + _transform.position;

		// initialize random vine target movement properties
		vineSettings.targetTrackRadius = UnityEngine.Random.Range(2f, 10f);
		vineSettings.targetAngle = UnityEngine.Random.Range(0f, 360f);
		vineSettings.targetHeight = UnityEngine.Random.Range(5f, 15f);
		vineSettings.targetSpeed = UnityEngine.Random.Range(-0.5f, 0.5f);
	}

	void Update()
	{
		length = getTotalLength();

		bool wasGrowing = isGrowing;
		// update vine skeleton structure (such as adding a new segment)
		if (getTotalLength() < lengthGoal)
		{
			growVine();

			if (!wasGrowing)
			{
				isGrowing = true;
			}
		}
		else if (wasGrowing)
		{
			//printSkeletonInfo();
			isGrowing = false;
		}

		// debug vine growth testing
		/*
		if (Input.GetButtonDown("GrowVineDebug") && !pressedVineButton)
		{
			setGrowthInfo(getTotalLength() + 0.5f, 0.15f);
			//Debug.Log(Vector3.Cross(vineSkeleton.Last().getNodeEndPoint(), vineTarget.transform.position - _transform.position));
			//moveTowardsTarget();
			pressedVineButton = true;
		}
		else
		{
			pressedVineButton = false;
		}
		*/

		moveTarget();
		moveTowardsTarget();

	}

	// Debug drawing of the skeleton.
	// Uncheck the vine's Mesh Renderer in the Inspector to turn off mesh
	// for easier skeleton viewing.
	/*
	void OnDrawGizmos()
	{
		// Avoid NPE in the editor when the game isn't running,
		// (and therefore the object hasn't been initialized).
		if (_transform)
		{
			Vector3 mPos = _transform.position;

			if (vineSkeleton != null)
			{
				for (int node = 0; node < vineSkeleton.Count; node++)
				{
					Vector3 nStart = vineSkeleton[node].startPoint;
					Vector3 nEnd = vineSkeleton[node].getNodeEndPoint();

					// Draw segment endpoints with red spheres
					Gizmos.color = Color.red;
					Gizmos.DrawSphere(mPos + nStart, debugSphereSize);

					// Draw segments with lines. Alternate colors for each segment,
					// but keep the tip segment red (or it swaps colors weirdly)
					if (node == vineSkeleton.Count - 1)
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
				Gizmos.DrawSphere(mPos + vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay(), debugSphereSize);
			}
		}
			
	}
	*/

	/*
	SKELETON FUNCTIONS

	These are private functions which create and manipulate the vine's skeleton.
	This includes growing and moving the vine.
	*/

	private void createInitialVineSkeleton()
	{
		vineSkeleton.Add(new VineNode(initialRadius, initialSegLength, Vector3.zero, Vector3.up));
		//Debug.Log("Node 0 start: " + vineSkeleton[0].startPoint);



		createMesh();

	}

	private void growVine()
	{
		// Extend the length of the vine.
		// The segment before the tip ring will be extended. If it reaches its max length,
		// then a new segment will be added, and the overflow growth distance
		// will be its initial length.

		//Debug.Log("Entered growVine()");
		//Debug.Break();

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
			addSegment(initialRadius, newGrowth, vineSkeleton.Last().direction);
			//Debug.Log("Creating first non-tip segment");
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
				addSegment(initialRadius, overflow, vineSkeleton.Last().direction);
				//Debug.Log("Segment overflow (" + newSegLength + "). segment " + growIndex + " maxed out at " + vineSkeleton[growIndex].length + ", so a new node is created with length " + overflow);
			}
		}

		updateSkeleton(vineSkeleton);
	}

	private void updateSkeleton(List<VineNode> v)
	{
		// Iterate through all the nodes and make sure the start points correspond 
		// to the ends of the previous nodes.
		if (v.Count > 1)
		{
			for (int node = 1; node < v.Count; node++)
			{
				v[node].startPoint = v[node - 1].startPoint + v[node - 1].getNodeRay();
			}
		}
		
		updateMesh();
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

	private float vineDistToGoal(List<VineNode> v)
	{
		float dist = 0;
		Vector3 vineTip = _transform.position + v.Last().getNodeEndPoint();
		Vector3 target = vineTarget.transform.position;

		return Vector3.Distance(vineTip, target);
	}

	private void moveTowardsTarget()
	{
		
		float rotAmount = 1f;

		string debugOutput = "Initial distance to target: " + vineDistToGoal(vineSkeleton);

		// find the gradient for each node, and rotate by that amount.
		for (int node = 0; node < vineSkeleton.Count; node++)
		{
			debugOutput += "\n\tNode " + node + ":";

			Vector3 rotAxis = Vector3.Cross(vineSkeleton[node].getNodeEndPoint(), vineTarget.transform.position - _transform.position).normalized;

			Vector3 tipPoint = vineSkeleton.Last().getNodeEndPoint();
			Vector3 toTip = tipPoint - vineSkeleton[node].startPoint;
			Vector3 toTarget = (vineTarget.transform.position - _transform.position) - tipPoint;

			Vector3 movementVector = Vector3.Cross(toTip, rotAxis);

			float gradient = Vector3.Dot(movementVector, toTarget);
			Vector3 rotGrad = Quaternion.AngleAxis(-gradient, rotAxis) * vineSkeleton[node].direction;

			debugOutput += "\n\t\trotAxis: " + rotAxis.ToString("F8");
			debugOutput += "\n\t\ttipPoint: " + tipPoint.ToString("F8");
			debugOutput += "\n\t\ttoTip: " + toTip.ToString("F8");
			debugOutput += "\n\t\ttoTarget: " + toTarget.ToString("F8");
			debugOutput += "\n\t\tmovementVector: " + movementVector.ToString("F8");
			debugOutput += "\n\t\tgradient: " + gradient.ToString("F8");
			debugOutput += "\n\t\trotGrad: " + rotGrad.ToString("F8");


			// apply the rotation to the node
			vineSkeleton[node].direction = rotGrad;
			updateSkeleton(vineSkeleton);

		}

		debugOutput += "\nFinal distance to target: " + vineDistToGoal(vineSkeleton);
		//Debug.Log(debugOutput);

		updateMesh();
	}

	//move the target for the vine
	private void moveTarget()
	{
		//target should orbit above the vine's base
		vineSettings.targetAngle += (vineSettings.targetSpeed * Time.deltaTime) % 360f;
		float t_x = vineSettings.targetTrackRadius * Mathf.Cos(vineSettings.targetAngle);
		float t_z = vineSettings.targetTrackRadius * Mathf.Sin(vineSettings.targetAngle);
		float t_y = vineSettings.targetHeight;

		vineTarget.transform.position = new Vector3(t_x, t_y, t_z) + _transform.position;
	}

	/*
	MESH FUNCTIONS

	These are private functions which manage the mesh constructed around the skeleton.
	*/

	// createMesh() is only called once, at the beginning.
	// vertices[] and triangles[] are cleared and started from scratch.
	private void createMesh()
	{

		// maybe do some error handling here to make sure that there is at least one segment!

		int res = vineSettings.resolution;

		// just in case, clear things that should already be empty
		mesh.Clear();
		vertices.Clear();
		uvs.Clear();
		triangles.Clear();

		// push the tip vertex
		vertices.Add(vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay());
		//Debug.Log("tip: " + vertices[0]);

		// push vertices for each ring

		for (int node = 0; node < vineSkeleton.Count; node++)
		{
			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = vineSkeleton[node].radius * Mathf.Cos(angle);
				float v_z = vineSkeleton[node].radius * Mathf.Sin(angle);
				float v_y = 0;

				// I'm 99% sure that the "+ _transform.position" part should not be there... test this some time.
				Vector3 relativeVec = new Vector3(v_x, v_y, v_z) + _transform.position;

				vertices.Add(vineSkeleton[node].startPoint + relativeVec);
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
				uvs.Add(new Vector2(0,0));
			}
		}

		// also, we need to move the tip point to it's new location.
		Vector3 newTip = vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay();
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

	// updateMesh() is called whenever existing mesh vertices need to be transformed.
	// vertices[] is cleared and started from scratch, but triangles[] is left untouched.
	private void updateMesh()
	{
		float res = vineSettings.resolution;
		// when the vine moves, pretty much all vertices are subject to change.
		// it's easier just to clear it and start from scratch.
		vertices.Clear();

		string debugString = "Node rotation info";

		// push the tip vertex
		vertices.Add(vineSkeleton.Last().startPoint + vineSkeleton.Last().getNodeRay());

		// push vertices for each ring

		for (int node = 0; node < vineSkeleton.Count; node++)
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
				bisectAxis = Vector3.Cross(vineSkeleton[node-1].direction, vineSkeleton[node].direction).normalized;
				bisectAngle = Vector3.Angle(vineSkeleton[node-1].direction, vineSkeleton[node].direction) / 2f;
				bottomAngle = Vector3.Angle(vineSkeleton[node-1].direction, Vector3.up);
				prevSegAxis = Vector3.Cross(vineSkeleton[node-1].direction, Vector3.up);

				// debugString += "\n\t\taVec: " + vineSkeleton[node-1].direction.ToString("F8");
				// debugString += "\n\t\tbVec: " + vineSkeleton[node].direction.ToString("F8");
			}

			// debugString += "\n\t\trotAxis: " + bisectAxis.ToString("F8");
			// debugString += "\n\t\tnodeAngle: " + bisectAngle;

			

			for (int ringVert = 0; ringVert < res; ringVert++)
			{
				float angle = ringVert * ringRadians * -1;
				float v_x = vineSkeleton[node].radius * Mathf.Cos(angle);
				float v_z = vineSkeleton[node].radius * Mathf.Sin(angle);
				float v_y = 0;

				// ring is first initialized to fall on the Vecotr3.up plane.
				// now, rotate the rings to fall on the vineSkeleton[node-1].direction plane.

				Vector3 relativeVec = new Vector3(v_x, v_y, v_z);

				// debugString += "\n\t\t\tringVert before rotation: " + relativeVec.ToString("F8");

				relativeVec = Quaternion.AngleAxis(-bottomAngle, prevSegAxis) * relativeVec;

				// debugString += "\n\t\t\tringVert after rotation: " + relativeVec.ToString("F8");

				// from here, we want to rotate the ring halfway to the vineSkeleton[node].direction plane

				relativeVec = Quaternion.AngleAxis(bisectAngle, bisectAxis) * relativeVec;

				vertices.Add(vineSkeleton[node].startPoint + relativeVec);
			}
		}

		mesh.vertices = vertices.ToArray();
		//Debug.Log(debugString);
		//Debug.Break();
	}

	/*
	PUBLIC FUNCTIONS

	These functions either:

	- GET information about this vine to be used outside of the Vine class, or
	- SET values in the Vine class that internal private functions will use to change the vine.
	- PRINT debug info about the Vine

	These functions should never directly manipulate the mesh or vertices.
	*/
	public void setGrowthInfo(float goal, float rate)
	{
		lengthGoal = goal;
		growthRate = rate;
	}

	public float getTotalLength()
	{
		if (vineSkeleton != null)
		{
			float len = 0f;

			for (int node = 0; node < vineSkeleton.Count; node++)
			{
				len += vineSkeleton[node].length;
			}

			return len;
		}
		else
		{
			Debug.Log("Can not get length of vine before it's been created!");

			return 0f;
		}
			
	}

	public void printSkeletonInfo()
	{
		string vineInfo = "Number of nodes: " + vineSkeleton.Count
						   + "\nVine Length: " + getTotalLength();

		string nodeInfo = "";

		for (int node = 0; node < vineSkeleton.Count; node++)
		{
			nodeInfo += "Node #" + node + "\n";
			nodeInfo += "\tStart: " + vineSkeleton[node].startPoint + "\n";
			nodeInfo += "\tEnd: " + vineSkeleton[node].getNodeEndPoint() + "\n";
		}

		Debug.Log(vineInfo + "\n" + nodeInfo);
	}
}