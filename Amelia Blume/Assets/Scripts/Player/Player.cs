using UnityEngine;
using System.Collections;
using System;

/// <summary>
/// Primary player controller class. Provides a few basic
/// utility functions for all related components and readonly
/// properties for commonly-used values and for cached
/// component references to avoid redundant GetComponent calls.
/// 
/// External interactions with Player should be provided by
/// adding methods to this object, which in turn interact with
/// the other components, to provide a single point of interaction.
/// </summary>
[RequireComponent(typeof(PlayerController))]
//[RequireComponent(typeof(PlayerCameraController))]
[RequireComponent(typeof(PlayerMotor))]
public class Player : BaseBehavior {
	
#region Component Accessors

	/*
	 * Public properties providing access to various components.
	 * On first lookup, the result is cached to speed up all
	 * future calls.
	 */

	public enum SeedType
	{
		VineSeed,
		FernSeed,
		TreeSeed,
		FlowerSeed,
	};

	public int health;
	public bool airDashed = false;
	public bool isDashing = false;
	public float dashStartX = 0F;
	public float dashedAtTime = 0F;
	private GameObject spawner;
	protected PlayerController cachedPlayerController;
	private GameObject fruit;
	private bool canGrow = false;
	private bool sunning = false;
	private bool converting = false;
	public SeedType currentSeed = SeedType.VineSeed;
	private bool canReadSign = false;
	private GameObject currentSign;

	public bool vineUnlocked = false;
	public bool treeUnlocked = false;
	public bool fluerUnlocked = false;
	public bool fernUnlocked = false;

	

	public PlayerController controller {
		get {
			if (cachedPlayerController == null) {
				cachedPlayerController = GetComponent<PlayerController>();
			}
			return cachedPlayerController;
		}
	}
	
	protected PlayerMotor cachedPlayerMotor;
	public PlayerMotor motor {
		get {
			if (cachedPlayerMotor == null) {
				cachedPlayerMotor = GetComponent<PlayerMotor>();	
			}
			return cachedPlayerMotor;
		}
	}
	
	protected PlayerCameraController cachedPlayerCameraController;
	public PlayerCameraController cameraController {
		get {
			if (cachedPlayerCameraController == null) {
				cachedPlayerCameraController = GetComponent<PlayerCameraController>();	
			}
			return cachedPlayerCameraController;
		}
	}
	
	protected CharacterController cachedCharacterController;
	public CharacterController characterController {
		get {
			if (cachedCharacterController == null) {
				cachedCharacterController = GetComponent<CharacterController>();
			}
			return cachedCharacterController;
		}
	}
	
	protected PlayerPushHandler cachedPlayerPushHandler;
	public PlayerPushHandler pushHandler {
		get {
			if (cachedPlayerPushHandler == null) {
				cachedPlayerPushHandler = GetComponent<PlayerPushHandler>();
			}
			return cachedPlayerPushHandler;
		}
	}
	
	protected PlayerAnimationHandler cachedPlayerAnimationHandler;
	public PlayerAnimationHandler animationHandler {
		get {
			if (cachedPlayerAnimationHandler == null) {
				cachedPlayerAnimationHandler = GetComponent<PlayerAnimationHandler>();
			}
			return cachedPlayerAnimationHandler;
		}
	}
	
#endregion
	
#region Utility properties
	
	/*
	 * Convenience properties for potentially useful values.
	 * All properties in this region simply retrieve a
	 * value from another component or, in a few cases,
	 * combine multiple related lookups (e.g. canJump).
	 */
	
	public float height {
		get {
			return characterController.height;
		}
	}

	public Vector3 velocity {
		get {
			return characterController.velocity;	
		}
	}
	public Vector3 attemptedVelocity {
		get {
			return motor.lastAttemptedMovement;	
		}
	}

	public bool isSliding {
		get {
			return controller.sliding;	
		}
	}
	public bool isSlidingFast {
		get {
			return controller.slidingFast;	
		}
	}
	
	public bool isRunning {
		get {
			return controller.running || controller.alwaysRun;	
		}
	}
	
	public bool isGrounded {
		get {
			return characterController.isGrounded;	
		}
	}
	public bool isCollidingSides {
		get {
			return (controller.collisionFlags & CollisionFlags.Sides) != 0;	
		}
	}
	public bool isCollidingAbove {
		get {
			return (controller.collisionFlags & CollisionFlags.Above) != 0;	
		}
	}
	
	public bool isFacingRight {
		get {
			return controller.isFacingRight;	
		}
	}

	public float groundSlope {
		get {
			return motor.slide.groundSlope;	
		}
	}
	public Vector3 groundNormal {
		get {
			return motor.slide.groundNormal;	
		}
	}
	public Vector3 localGroundNormal {
		get {
			return motor.slide.localGroundNormal;	
		}
	}
	
	public bool canJump {
		get {
			return isGrounded && !isSlidingFast && !motor.isGroundTooSteepToJump;	
		}
	}

	public bool canThrowSeed {
		get {
			//need to make sure they have seeds unlocked
			//player will always get vine first
			if(!vineUnlocked)
				return false;

			return true;
		}
	}

	public bool canSun {
		get {
			return true;
		}
	}

	public bool canConvert {
		get {
			return cachedPlayerController.canConvert;
		}
	}

	public bool canDash {
		get {
			if (!isGrounded && !airDashed){
				airDashed = true;
				return true;
			}
			else if(!isGrounded && airDashed)
				return false;

			else if(isGrounded && Time.time - dashedAtTime >= 1.0F)
			{
				return true;
			}
			else
				return false;
		}
	}


#endregion
	
	
#region Internal methods
	
	protected void Start() {
		Broadcast("OnSpawn");
		health = 100;
	}

#endregion
	

#region Public methods
	
	// Method for external objects to push the character
	public void Push(Vector3 pushForce) {
		characterController.Move(pushForce);
	}
	
	
	/*
	 * Utility shorthand methods for broadcasting events
	 * to all components of the Player without requiring
	 * receivers.
	 */
	
	public void Broadcast(string eventName) {
		BroadcastMessage(eventName, SendMessageOptions.DontRequireReceiver);	
	}
	public void Broadcast(string eventName, object args) {
		BroadcastMessage(eventName, args, SendMessageOptions.DontRequireReceiver);	
	}

	void Update()
	{

		//ReduceHealth(1);



		//ReduceHealth(1);
		if (canGrow && this.transform.localScale.x < 1) {
			this.transform.localScale = new Vector3 (this.transform.localScale.x + .1f, this.
			                                        transform.localScale.y + .1f, this.transform.localScale.z + .1f);
		} else
			SetCanGrow (false);

	}

	public void ReduceHealth(int subtract)
	{
		health -= subtract;
		if (GetHealth() <= 0)
			Kill ();
	}

	public void SetHealth(int newHealth)
	{
		health = newHealth;
	}

	public int GetHealth()
	{
		return health;
	}

	public bool CheckCanGrow()
	{
		return canGrow;
	}

	public void SetCanGrow(bool value){
		canGrow = value;
	}

	public bool isSunning()
	{
		return sunning;
	}

	public void SetSunning(bool value)
	{
		sunning = value;
	}

	public bool isConverting()
	{
		return converting;
	}
	
	public void SetConverting(bool value)
	{
		converting = value;
	}

	public SeedType getCurrentSeedType()
	{
		return currentSeed;
	}

	public void SetCurrentSeed(SeedType seed)
	{
		currentSeed = seed;
	}

	public void SetReadSign(bool status){
		canReadSign = status;
	}

	public void SetCurrentSign(GameObject sign){
		currentSign = sign;
	}

	public GameObject GetCurrentSign(){
		return currentSign;
	}

	public bool GetReadSign(){
		return  canReadSign;
	}

	//returns direction the player is currently facing as an int. 1=right, -1=left
	//we don't call it derkrection
	public int GetDirection()
	{
		if (cachedPlayerController.isFacingRight)
			return 1;
		else
			return -1;
	}


	void Kill()
	{
		Debug.Log ("Killed Called");
		spawner = GameObject.FindGameObjectWithTag ("Spawner");
		Vector3 fruitPosition = new Vector3(spawner.transform.position.x,spawner.transform.position.y+4f, 0);
		fruit = (GameObject)Resources.Load ("RespawnFruit");
		fruit.transform.position = fruitPosition;
		this.transform.position = new Vector3(spawner.transform.position.x,spawner.transform.position.y + 4f, 0);
		SetHealth (100);
		//make sure to set the blossoms before scaling
		transform.GetComponent<PlayerController> ().checkHealth ();

		SideScrollerCameraController controller = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<SideScrollerCameraController>();
		//controller.MoveToPlayer(spawner.transform.position.x, spawner.transform.position.y + 4f);
		controller.MoveToPosition (spawner.transform.position.x, spawner.transform.position.y + 4f, true);
		Instantiate (fruit);
		this.transform.localScale = new Vector3 (0.1f, 0.1f, 0.1f);
	}



	

#endregion
	
}
