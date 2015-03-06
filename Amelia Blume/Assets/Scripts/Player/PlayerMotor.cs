using UnityEngine;
using System.Collections;

public class PlayerMotor : BaseBehavior {
	
	public class ImpactArgs {
		public GameObject impactObject;
		public float impactSpeed;
		
		public ImpactArgs(GameObject obj, float speed) {
			impactObject = obj;
			impactSpeed = speed;
		}
	}
	
	
	[System.Serializable]
	public class MovementSettings {
		public float speed = 0f;
		public float walkSpeed = 4f;
		public float runSpeed = 8f;
		public float airControlRatio = 0.3f;
		
		public float sidestepSpeed = 3f;
		// I think this part is irrelevant, since we constrain the Z axis.
		public float sidestepWhileMovingRatio = 0.7f;
		public bool sidestepAtFullSpeed = false;
		
		public float turnToFaceCameraSpeed = 8f;
		
		public float jumpForce = 15f;
		public float dashForce = 15f;
		public float acceleration = 10f;
		// determines how quickly you accelerate towards 0 when trying to stop. Low number = sliding.
		public float stoppingPower = 15f;
				
		public float fallSpeed = 0f;
		public float baseFallSpeed = -5f;

		public float jumpForceRemaining = 0f;
		public float dashForceRemaining = 0f;

		public Vector3 momentum = Vector3.zero;
	}
	
	[System.Serializable]
	public class EnvironmentSettings {
		public float gravity = 20f;
		public float terminalVelocity = 20f;
		public GameObject ground = null;
		public bool grounded;
		public bool wasGrounded;
		public float altitude = 0f;
		
		public float maxCalculatedAltitude = 100f;
		
		public Vector3 lastGroundPosition = Vector3.zero;
		public Vector3 currentGroundPosition = Vector3.zero;
		public Vector3 groundMovement = Vector3.zero;
		
		public Quaternion lastGroundRotation = Quaternion.identity;
		public Quaternion currentGroundRotation = Quaternion.identity;
		public Quaternion groundRotation = Quaternion.identity;

	}
	
	[System.Serializable]
	public class SlideSettings {
		public Vector3 groundNormal = Vector3.up;
		public Vector3 localGroundNormal = Vector3.up;
		public float groundSlope = 0f;
		public Vector3 direction;
		
		public float slideAt = 45f;
		public float uncontrolledSlideAt = 60f;
		public float speed = 10f;		
		
		public bool slopedLeft = false;
		public bool slopedRight = false;
		public bool slopedUp = false;
		public bool slopedDown = false;
	}
	
	
	public MovementSettings movement = new MovementSettings();
	public EnvironmentSettings environment = new EnvironmentSettings();
	public SlideSettings slide = new SlideSettings();
	
	public Vector3 lastInput;
	public Vector3 lastAttemptedMovement;
	
	protected Vector3 pendingInput;
	
	protected LayerMask groundTestMask;
	
	
	public bool isGroundTooSteepToJump {
		get {
			return (slide.groundSlope >= slide.uncontrolledSlideAt);	
		}
	}
	
	protected void Awake() {
		groundTestMask = ~(1 << LayerMask.NameToLayer("Player"));
	}
		
	// this is the only function that actually moves the player. All other functions prepare
	// information that will be processed here, before applying the work vector.
	public void UpdateMotor(Vector3 movementInput) {
		environment.wasGrounded = environment.grounded;
		
		pendingInput = movementInput;
		
		//RotateTowardCameraDirection();
		UpdateGround();		
		ProcessMotion();
		
		environment.grounded = player.characterController.isGrounded;
		
		if (environment.grounded && !environment.wasGrounded) {
			player.Broadcast("OnPlayerHitGround", environment.ground);
		}
		if (!environment.grounded && environment.wasGrounded) {
			player.Broadcast("OnPlayerLeftGround", environment.ground);
		}
		
		lastInput = movementInput;
	}
	
	protected void ProcessMotion() {
		Vector3 workVector = pendingInput;
		
		float targetSpeed = 0f;
		float acceleration = 1f;
		
		if (player.isGrounded) {
			if (player.controller.running || player.controller.alwaysRun) {
				targetSpeed = movement.runSpeed;
				acceleration = movement.acceleration;
			} else if (pendingInput.x != 0 || pendingInput.z != 0) {
				targetSpeed = movement.walkSpeed;
				acceleration = movement.acceleration;
			} else {
				targetSpeed = 0;
				acceleration = movement.stoppingPower;
			}
			
			/*if (player.controller.isSideStepping) {
				if (!movement.sidestepAtFullSpeed) {
					targetSpeed = movement.sidestepSpeed;
				}
			}*/
		}
				
		movement.speed = Mathf.Lerp(movement.speed, targetSpeed, acceleration * Time.deltaTime);
		
		workVector.x *= movement.sidestepWhileMovingRatio;
		
		workVector = transform.TransformDirection(workVector);
		

		if (workVector.magnitude > 1) {
			workVector = workVector.normalized;
		}

		workVector *= movement.speed;

		if (player.isGrounded) {
			movement.momentum = workVector;
		} else {
			workVector = movement.momentum + workVector * movement.airControlRatio;
			if (workVector.magnitude > movement.momentum.magnitude) {
				workVector = workVector.normalized * movement.momentum.magnitude;	
			}
		}
		

		lastAttemptedMovement = workVector;
		
		workVector = ApplySlide(workVector);
		workVector = ApplyGravity(workVector);
		workVector = ApplyJumpPower(workVector);
		workVector = ApplyDashPower(workVector);
		workVector = ApplyGroundMovement(workVector);
		
		player.controller.CommitMove(workVector * Time.fixedDeltaTime);
	}
	
	protected Vector3 ApplyJumpPower(Vector3 workVector) {
		if (player.isCollidingAbove) {
			movement.jumpForceRemaining = 0;
		}
		
		if (movement.jumpForceRemaining > 0) {
			workVector.y += movement.jumpForceRemaining;
			movement.jumpForceRemaining -= environment.gravity * Time.fixedDeltaTime;
		}
		
		return workVector;
	}
	
	protected Vector3 ApplyDashPower(Vector3 workVector) {
		
		if (movement.dashForceRemaining > 0 && player.isFacingRight) {
				workVector.x += movement.dashForceRemaining;
				movement.dashForceRemaining -= environment.gravity * Time.fixedDeltaTime;
		}

		if (movement.dashForceRemaining < 0 && !player.isFacingRight) {
				workVector.x += movement.dashForceRemaining;
				movement.dashForceRemaining += environment.gravity * Time.fixedDeltaTime;
		}
		
		
		return workVector;
	}

	protected Vector3 ApplyGravity(Vector3 workVector) {
		if (environment.grounded) {
			if (!environment.wasGrounded) {
				player.Broadcast("OnGroundImpact", new ImpactArgs(environment.ground, movement.fallSpeed));		
			}
			movement.fallSpeed = 0;
			workVector.y = movement.baseFallSpeed;	
		} else {
			if (workVector.y > -environment.terminalVelocity) {
				if (movement.fallSpeed > movement.baseFallSpeed) {
					movement.fallSpeed = movement.baseFallSpeed;
				}
				movement.fallSpeed -= environment.gravity * Time.fixedDeltaTime;
				workVector.y += movement.fallSpeed;
			}
		}
		
		
		return workVector;
	}
	
	protected Vector3 ApplySlide(Vector3 workVector) {
		if (!player.isGrounded) {
			return workVector;
		}
		
		slide.direction = Vector3.zero;		
		
		RaycastHit hit;
		
		if (Physics.Raycast(transform.position + Vector3.up, Vector3.down, out hit, 1.5f)) {
			slide.groundNormal = hit.normal;
		}
		
		slide.localGroundNormal = player.transform.worldToLocalMatrix * slide.groundNormal;
		slide.groundSlope = Vector3.Angle(Vector3.up, slide.localGroundNormal);
		
		slide.direction = slide.groundNormal / 2f;
		
		slide.direction.y += movement.baseFallSpeed;

		slide.slopedUp = (slide.localGroundNormal.z < -0.1);
		slide.slopedDown = (slide.localGroundNormal.z > 0.1);
		
		slide.slopedLeft = (slide.localGroundNormal.x < -0.1);	
		slide.slopedRight = (slide.localGroundNormal.x > 0.1);

		
		if (slide.groundSlope >= slide.uncontrolledSlideAt) {
			if (!player.controller.slidingFast) {
				player.Broadcast("OnBeginUncontrolledSlide");
				player.controller.slidingFast = true;
			}
			if (!player.controller.sliding) {
				player.Broadcast("OnBeginSlide");
				player.controller.sliding = true;
			}
			workVector = slide.direction * slide.speed;
		} else if (slide.groundSlope >= slide.slideAt) {
			if (!player.controller.sliding) {
				player.Broadcast("OnBeginSlide");	
				player.controller.sliding = true;
			}
			workVector += slide.direction * slide.speed;
		} else {
			if (player.controller.sliding) {
				player.Broadcast("OnEndSlide");
				player.controller.sliding = false;
			}
			if (player.controller.slidingFast) {
				player.Broadcast("OnEndUncontrolledSlide");	
				player.controller.slidingFast = false;
			}
		}			
			
		
		return workVector;
	}
	
	protected Vector3 ApplyGroundMovement(Vector3 workVector) {
		if (player.isGrounded) {
			workVector += environment.groundMovement;
		}
		
		return workVector;
	}
	
	protected void UpdateGround() {
		GameObject newGround;
		RaycastHit hit;
		float rayOffset = 0.5f;
		
		if (Physics.Raycast(player.transform.position + rayOffset * Vector3.up, -Vector3.up, out hit, environment.maxCalculatedAltitude, groundTestMask)) {
			newGround = hit.collider.gameObject;
			if (newGround != environment.ground) {
				if (environment.ground == null) {
					player.Broadcast("OnLeaveMaxAltitude");	
				}
				environment.ground = newGround;				
				environment.lastGroundPosition = environment.ground.transform.position;
				environment.currentGroundPosition = environment.lastGroundPosition;
				environment.groundMovement = Vector3.zero;
				
				environment.lastGroundRotation = environment.ground.transform.rotation;
				environment.currentGroundRotation = environment.lastGroundRotation;
				environment.groundRotation = Quaternion.identity;
				
			} else {
				environment.lastGroundPosition = environment.currentGroundPosition;
				environment.currentGroundPosition = environment.ground.transform.position;
				
				environment.lastGroundRotation = environment.currentGroundRotation;
				environment.currentGroundRotation = environment.ground.transform.rotation;
				
				if (environment.ground.rigidbody != null) {
					environment.groundMovement = environment.ground.rigidbody.velocity;	
				} else {
					environment.groundMovement = (environment.currentGroundPosition - environment.lastGroundPosition) / Time.fixedDeltaTime;
				}
				
				environment.groundRotation = 
					environment.currentGroundRotation * Quaternion.Inverse(environment.lastGroundRotation);
					
			}
			environment.altitude = hit.distance - rayOffset;
			if (environment.grounded) {
				environment.altitude = 0;	
			}
		} else {
			if (environment.ground != null) {
				player.Broadcast("OnReachMaxAltitude");	
			}
			environment.ground = null;
			environment.currentGroundPosition = Vector3.zero;
			environment.lastGroundPosition = Vector3.zero;
			environment.altitude = environment.maxCalculatedAltitude;
		}
	}
	
	public void Jump() {
		if (player.canJump) {
			player.Broadcast("OnJump");
			movement.jumpForceRemaining = movement.jumpForce;
		} else {
			player.Broadcast("OnJumpDenied");
		}
	}

	public void Dash() {
		if (player.canDash) {
			player.Broadcast("OnDash");
			if(player.isFacingRight)
				movement.dashForceRemaining = movement.dashForce;
			else {
				movement.dashForceRemaining = movement.dashForce * -1;
			}
		} else {
			player.Broadcast("OnDashDenied");
		}
	}

	public void ThrowSeed() {
		if(player.canThrowSeed) {
			player.Broadcast("OnThrowSeed");
			// spawn a seed
			Vector3 loc = new Vector3(transform.GetComponent<PlayerController>().faceDirection * 0.5f, 1, 0);
			loc += transform.position;
			//GameObject newSeed = Instantiate(Resources.Load("VineSeed")) as GameObject;
			GameObject newSeed;
			Player.SeedType currSeed = player.getCurrentSeedType();
			switch(currSeed){
			case Player.SeedType.VineSeed:
				newSeed = Instantiate(Resources.Load("VineSeed")) as GameObject;
				break;
				
			case Player.SeedType.TreeSeed:
				newSeed = Instantiate(Resources.Load("TreeSeed")) as GameObject;
				break;
				
			case Player.SeedType.FlowerSeed:
				newSeed = Instantiate(Resources.Load("FlowerSeed")) as GameObject;
				break;
				
			case Player.SeedType.FernSeed:
				newSeed = Instantiate(Resources.Load("FernSeed")) as GameObject;
				break;

			default:
				newSeed = Instantiate(Resources.Load("VineSeed")) as GameObject;
				break;
			}

			newSeed.transform.position = loc;
			newSeed.rigidbody.velocity = new Vector3(0,-3,0);
			Debug.Log("called ThrowSeed");
		}
		else{
			player.Broadcast("OnThrowSeedDenied");
		}
	}

	public void Sun() {
		if (player.canSun) {
			player.Broadcast("OnSun");
			//SunStuff
			GameObject sun = (GameObject) Resources.Load("Sun");
			sun.transform.position = new Vector3(this.transform.position.x, this.transform.position.y+2, this.transform.position.z);
			Instantiate(sun);
		}
		
	}
	
	public void RotateTowardCameraDirection() {
		if (pendingInput.x != 0 || pendingInput.z != 0 || player.controller.isTurning) {
			Quaternion targetRot = Quaternion.Euler(
				transform.eulerAngles.x,
				Camera.main.transform.eulerAngles.y,
				transform.eulerAngles.z
			);
			
			transform.rotation = Quaternion.Lerp(
				transform.rotation,
				targetRot,
				Time.fixedDeltaTime * movement.turnToFaceCameraSpeed
			);
		}
	}
}
