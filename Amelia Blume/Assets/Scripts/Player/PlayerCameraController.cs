using UnityEngine;
using System.Collections;

public class PlayerCameraController : BaseBehavior {
	
	[System.Serializable]
	public struct ClipPanePoints {
		public Vector3 upperLeft;
		public Vector3 upperRight;
		public Vector3 lowerLeft;
		public Vector3 lowerRight;
	}
	[System.Serializable]
	public class OcclusionCheckResults {
		public bool wasOccluded;
		public bool stayedOccluded;
		
		public bool occludedLeft;
		public bool occludedRight;
		public bool occludedTop;
		public bool occludedBottom;
		public bool occludedCenter;
		
		public float nearestDistance;
	
		public bool SaveIfNearest(float potential) {
			if (potential < nearestDistance || nearestDistance == -1) {
				nearestDistance = potential;	
				return true;
			}
			return false;	
		}
		public void Reset() {
			nearestDistance = -1;	
			occludedBottom = false;
			occludedTop = false;
			occludedLeft = false;
			occludedRight = false;
			occludedCenter = false;
		}
	}
	[System.Serializable]
	public class DistanceSettings {
		public float current = 5f;
		public float minimum = 2f;
		public float maximum = 10f;
		
		public float normalSmoothTime = 0.05f;
		public float resumeSmoothTime = 1f;
		
		public bool allowUserToZoom = true;
		
		[HideInInspector]
		public float initial;
		[HideInInspector]
		public float goal;
		[HideInInspector]
		public float preOcclusion;
	}
	[System.Serializable]
	public class MouseSettings {
		public float sensitivityX = 5f;
		public float sensitivityY = 5f;
		public float sensitivityWheel = 5f;
		
		public float smoothX = 0.05f;
		public float smoothY = 0.1f;

		public float yMinimum = -40f;
		public float yMaximum = 80f;
		
		public float x;
		public float y;
	}
	[System.Serializable]
	public class OcclusionSettings {
		public float distanceStep = 0.5f;
		public float minimumDistance = 0.25f;
		public int maximumSolverSteps = 10;		
		public float collisionPushForce = 4f;
		
		public OcclusionCheckResults state;
	}

	public Transform target;
	public Camera controlledCamera;
	public float inputDeadZone = 0.02f;
	public LayerMask cameraCollisionMask;
	
	public bool isMouseLooking = false;
	public bool alwaysMouseLook = false;
	
	public float manualRotationSpeedHorizontal = 1f;
	public float manualRotationSpeedVertical = 1f;
	
	public DistanceSettings distance = new DistanceSettings();
	public MouseSettings mouse = new MouseSettings();
	public OcclusionSettings occlusion = new OcclusionSettings();
	
	protected Vector3 goalPosition;
	
	protected float speedX;
	protected float speedY;
	protected float speedZ;
	protected float smoothSpeed;
	protected float smoothDuration;


	
	
	protected void Start() {
		distance.current = Mathf.Clamp(distance.current, distance.minimum, distance.maximum);
		distance.initial = distance.current;
		Reset();
	}

	protected void LateUpdate() {
		int occlusionCheckCount = 0;
		float xTweak = 0;
		float yTweak = 0;
		
		if (target == null) {
			return;	
		}	
		
		HandleInput();
		
		do {
			CalculateGoalPosition();
			CheckForOcclusion(occlusionCheckCount++);
		} while (occlusion.state.wasOccluded);
		
		if (occlusion.state.stayedOccluded) {
			
			occlusionCheckCount = 0;
			
			if (occlusion.state.occludedLeft) {
				xTweak = -mouse.sensitivityX * occlusion.collisionPushForce * Time.deltaTime;
			}
			if (occlusion.state.occludedRight) {
				xTweak = mouse.sensitivityX * occlusion.collisionPushForce * Time.deltaTime;
			}
			
			do {
				mouse.x += xTweak;
				mouse.y += yTweak;
				
				CalculateGoalPosition();
				CheckForOcclusion(occlusionCheckCount++);
			} while (occlusion.state.wasOccluded);
		}
		
		UpdatePosition();
	}
	
	public void RotateView(float horizontal, float vertical) {
		mouse.x += horizontal * manualRotationSpeedHorizontal;
		mouse.y -= vertical * manualRotationSpeedVertical;;
		
		mouse.y = ClampAngle(mouse.y, mouse.yMinimum, mouse.yMaximum);			
	}
	
	protected void HandleInput() {
		if (alwaysMouseLook || Input.GetMouseButton(1)) {
			Screen.lockCursor = true;
			mouse.x += Input.GetAxis ("Mouse X") * mouse.sensitivityX;
			mouse.y -= Input.GetAxis ("Mouse Y") * mouse.sensitivityY;
			isMouseLooking = true;
		} else {
			Screen.lockCursor = false;	
			isMouseLooking = false;
		}
		
		mouse.y = ClampAngle(mouse.y, mouse.yMinimum, mouse.yMaximum);
		
		if (distance.allowUserToZoom) {
			float scroll = Input.GetAxis("Mouse ScrollWheel");
			
			if (Mathf.Abs(scroll) > inputDeadZone) {
				distance.goal = Mathf.Clamp(distance.current - scroll * mouse.sensitivityWheel, distance.minimum, distance.maximum);
				distance.preOcclusion = distance.goal;
				smoothDuration = distance.normalSmoothTime;
				
			}
		}
	}
	
	protected float ClampAngle(float angle, float min, float max) {
		while (angle < -360) {
			angle += 360;	
		}
		
		while (angle > 360) {
			angle -= 360;	
		}
		
		return Mathf.Clamp(angle, min, max);
	}
	
	protected ClipPanePoints GetNearClipPane(Vector3 pos) {
		ClipPanePoints pane = new ClipPanePoints();	
		
		if (controlledCamera == null) {
			return pane;	
		}
		
		Transform cam = controlledCamera.transform;
		float halfFOV = Mathf.Deg2Rad * Camera.main.fieldOfView / 2;
		float cameraDistance = controlledCamera.nearClipPlane;
		float height = cameraDistance * Mathf.Tan (halfFOV);
		float width = height * controlledCamera.aspect;
		
		Vector3 xOffset = cam.right * width;
		Vector3 yOffset = cam.up * height;
		Vector3 zOffset = cam.forward * cameraDistance;
		
		// point = cameraPos + xOffset + yOffset + zOffset
		pane.lowerRight = pos + (xOffset) + (-yOffset) + zOffset;
		pane.lowerLeft = pos + (-xOffset) + (-yOffset) + zOffset;
		pane.upperRight = pos + (xOffset) + (yOffset) + zOffset;
		pane.upperLeft = pos + (-xOffset) + (yOffset) + zOffset;
			
		return pane;
	}
	
	protected Vector3 CalculateOrbitalPosition(float rotationX, float rotationY, float distance) {
		return target.position + Quaternion.Euler(rotationX, rotationY,0) * new Vector3(0, 0, -distance);
	}
	
	protected void CalculateGoalPosition() {

		ResetGoalDistance();
		
		distance.current = Mathf.SmoothDamp(distance.current, distance.goal, ref smoothSpeed, smoothDuration);
		
		goalPosition = CalculateOrbitalPosition(mouse.y, mouse.x, distance.current);
	}
	

	protected void CheckForOcclusion(int count) {		
		occlusion.state = CheckCameraPoints(target.position, goalPosition);
		
		OcclusionCheckResults state = occlusion.state;
		
		state.wasOccluded = false;
		state.stayedOccluded = false;

		
		if (state.nearestDistance != -1f) {
			if (count < occlusion.maximumSolverSteps) {
				state.wasOccluded = true;
				distance.current -= occlusion.distanceStep;

				if (distance.current < occlusion.minimumDistance) {
					distance.current = occlusion.minimumDistance;	
				}
			} else {
				state.stayedOccluded = true;
				distance.current = state.nearestDistance - controlledCamera.nearClipPlane;
				if (distance.current < distance.minimum) {
					distance.current = distance.minimum;
				}
			}
			
			distance.goal = distance.current;
			smoothDuration = distance.resumeSmoothTime;
		}
	}
	
	protected OcclusionCheckResults CheckCameraPoints(Vector3 from, Vector3 to) {
		OcclusionCheckResults result = new OcclusionCheckResults();
		
		result.Reset();
		
		RaycastHit hit;
		
		ClipPanePoints pane = GetNearClipPane(to);
				
		if (Physics.Linecast(from, pane.upperLeft, out hit, cameraCollisionMask) && !hit.collider.tag.Equals("Player")) {
			result.occludedLeft = true;
			result.occludedTop = true;
			result.SaveIfNearest(hit.distance);
		}

		if (Physics.Linecast(from, pane.upperRight, out hit, cameraCollisionMask) && !hit.collider.tag.Equals("Player")) {
			result.occludedRight = true;
			result.occludedTop = true;
			result.SaveIfNearest(hit.distance);
		}

		if (Physics.Linecast(from, pane.lowerRight, out hit, cameraCollisionMask) && !hit.collider.tag.Equals("Player")) {
			result.occludedRight = true;
			result.occludedBottom = true;
			result.SaveIfNearest(hit.distance);
		}

		if (Physics.Linecast(from, pane.lowerLeft, out hit, cameraCollisionMask) && !hit.collider.tag.Equals("Player")) {
			result.occludedLeft = true;
			result.occludedBottom = true;
			result.SaveIfNearest(hit.distance);
		}

		if (Physics.Linecast(from, to + controlledCamera.transform.forward * -controlledCamera.nearClipPlane, out hit, cameraCollisionMask) && !hit.collider.tag.Equals("Player")) {
			result.occludedCenter = true;
			result.SaveIfNearest(hit.distance);
		}
		
		return result;
	}
	
	protected void ResetGoalDistance() {
		OcclusionCheckResults results;
		
		if (distance.goal < distance.preOcclusion) {
			Vector3 pos = CalculateOrbitalPosition (mouse.y, mouse.x, distance.preOcclusion);
			
			results = CheckCameraPoints(target.position, pos);
			
			if (results.nearestDistance == -1 || results.nearestDistance > distance.preOcclusion) {
				distance.goal = distance.preOcclusion;
			}
		}
	}
	
	protected void UpdatePosition() {
		Vector3 cameraPos = controlledCamera.transform.position;
		
		float posX = Mathf.SmoothDamp(cameraPos.x, goalPosition.x, ref mouse.smoothX, speedX);
		float posY = Mathf.SmoothDamp(cameraPos.y, goalPosition.y, ref mouse.smoothY, speedY);
		float posZ = Mathf.SmoothDamp(cameraPos.z, goalPosition.z, ref mouse.smoothX, speedZ);
		
		controlledCamera.transform.position = new Vector3(posX, posY, posZ);
		
		controlledCamera.transform.LookAt(target);
	}
	
	public void Reset() {
		mouse.x = 0;
		mouse.y = 10;
		distance.current = distance.initial;
		distance.goal = distance.current;
		distance.preOcclusion = distance.current;
	}
}
