using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : BaseBehavior {
	
	
	public float inputDeadZone = 0.05f;
	
	public bool strafeLock = false;
	public bool strafing = false;
		
	public bool running = false;
	public bool alwaysRun = false;
	
	public bool sliding = false;
	public bool slidingFast = false;
	
	public bool isSideStepping = false;
	
	public bool isTurning = false;
	public float turnDirection = 0f;

	protected Vector3 pendingMovementInput;
	public CollisionFlags collisionFlags;
	

	
	protected void Update() {
		
		if (Camera.main == null) {
			return;		
		}
		
		HandleMovementInput();
		HandleActionInput();
		
	}
	
	protected void FixedUpdate() {
		player.motor.UpdateMotor(pendingMovementInput);		
		
		if (player.isGrounded) {
			player.transform.rotation *= player.motor.environment.groundRotation;
		}
	}
	
	public void CommitMove(Vector3 finalMovement) {
		collisionFlags = player.characterController.Move(finalMovement);
	}

	
	protected void HandleMovementInput() {
		float vertical = 0f;
		float horizontal = 0f;
		
		bool wasRunning = running;
		Vector3 lastInput = pendingMovementInput;
			
		running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
		strafing = Input.GetMouseButton(1);
				
		vertical  = Input.GetAxis("Vertical");
		if (Mathf.Abs(vertical) < inputDeadZone) {
			vertical = 0f;
		}
		
		isTurning = false;
		horizontal  = Input.GetAxis("Horizontal");
		if (strafing || strafeLock) {				
			if (Mathf.Abs(horizontal) < inputDeadZone) {
				horizontal = 0f;
			}
		} else if (Mathf.Abs(horizontal) > inputDeadZone) {
			isTurning = true;
			turnDirection = horizontal;
			player.cameraController.RotateView(horizontal, 0f);	
			player.motor.RotateTowardCameraDirection();
			horizontal = 0;
		}
		
		isSideStepping = (horizontal != 0 && Mathf.Abs(vertical) < inputDeadZone);
		
		lastInput = pendingMovementInput;
		pendingMovementInput = new Vector3(horizontal, 0, vertical);
		
		if (pendingMovementInput.magnitude == 0) {
			running = false;
			if (wasRunning) {
				player.Broadcast("OnStopRunning");	
			}
			if (lastInput.magnitude > 0) {
				player.Broadcast("OnStopMoving");	
			}
		} else {
			if (running && !wasRunning) {
				player.Broadcast("OnStartRunning");
			}
			if (wasRunning && !running) {
				player.Broadcast("OnStopRunning");	
			}
			if (lastInput.magnitude == 0) {
				player.Broadcast("OnStartMoving");	
			}
		}

	}
	
	protected void HandleActionInput() {
		if (Input.GetButtonDown("Jump")) {
			Jump();
		}
	}
	
	protected void Jump() {
		player.Broadcast("OnJumpRequest");
		player.motor.Jump();
	}
}
