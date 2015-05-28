using UnityEngine;
using System.Collections;
using System;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : BaseBehavior {
	//public bool Cutscene = false;
	public InputHandler playerInput;
	
	public float inputDeadZone = 0.05f;
    
    public float lockedAxisValue;
		
	public bool running = false;
	public bool alwaysRun = false;
	public bool isRunning = false;

	public bool isDashing = false;
	public bool isJumping = false;
	public bool isAirDashing = false;
	public bool isStunned = false;
	public bool isPlanting = false;
	public bool isSunLighting = false;

	public float watering = 0.0f;
	
    //do we want sliding? could be cool...
	public bool sliding = false;
	public bool slidingFast = false;

    public bool isFacingRight;
    public int faceDirection;

	public bool invulnerable = false;
	public int invulCounter;

	public bool canControl;
	public int stunTimer;

	public bool canConvert = false;
	public GameObject conversionTarget;

	public bool isTurning = false;
	public float turnDirection = 0f;

	public Transform blossomParent;
	public GameObject blossomPrefab;	
	public GameObject[] blossoms;
	public Vector3[] blossomPositions;
	public Quaternion[] blossomRotations;

	public GameObject[] activeSeeds;

	protected Vector3 pendingMovementInput;
	public CollisionFlags collisionFlags;
	
    void Start()
    {
//		Debug.Log (Input.GetJoystickNames()[0]);
		//get the input handler and reference that instead
		GameObject playerInputObj = GameObject.FindGameObjectWithTag ("Input Handler");
		if (playerInputObj != null) {
			playerInput = playerInputObj.GetComponent<InputHandler> ();
		}
    	// initialize Amelia's health blossoms
		blossoms = GameObject.FindGameObjectsWithTag("Blossom");
		blossomPositions = new Vector3[10];
		blossomRotations = new Quaternion[10];
		int i = 0;
		foreach (GameObject child in blossoms) 
		{
			blossomMover tempBlossom = blossoms[i].GetComponent<blossomMover>();
			if(tempBlossom != null)
			{
				blossoms[i].name = blossoms[i].name + " " + i;
				blossomPositions[i] = blossoms[i].transform.localPosition;
				blossomRotations[i] = blossoms[i].transform.localRotation;
				i++;
			}
		}

        //get the value to lock the player to (this is a sidescroller after all). Should ideally be 0 in the editor
        lockedAxisValue = this.transform.position.z;

        //set the angle to face the proper directions, then assign isFacingRight
        if (transform.rotation.eulerAngles.y >= 0 && transform.rotation.eulerAngles.y <= 180)
        {
            Vector3 _direction = ((new Vector3(this.transform.position.x + 1f, transform.position.y, lockedAxisValue))
                                    - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 50f);

            // TODO we should find a better solution for this. Always flipping these values together is not good practice.
            // can we consolidate to one variable? I recognize the advantages of both, but two seems bad. --Derk
            faceDirection = 1;
            isFacingRight = true;
        }
        else
        {
            Vector3 _direction = ((new Vector3(this.transform.position.x - 1f, transform.position.y, lockedAxisValue))
                                    - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 50f);
            isFacingRight = false;
            faceDirection = -1;
        }

		canControl = true;
		stunTimer = 0;
		activeSeeds = new GameObject[3];
    }
	
	// Update calls sporadically, as often as it can. Recieve input here, but don't apply it yet
	protected void Update() {
		if (playerInput == null) {
			playerInput = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();	
		}

		if (Camera.main == null) {
			return;		
		}

		//check to see if blossoms are up-to-date
		checkHealth ();

		if (playerInput.jumpUp || !canControl)
			StopJump();

		// these functions should not directly move the player. They only handle input, and 
		// send information to the motor, which will move the player in UpdateMotor().
		HandleStun ();
		HandleMovementInput();
		HandleActionInput();

		if (transform.rotation.eulerAngles.y >= 0 && transform.rotation.eulerAngles.y <= 180) {
			isFacingRight = true;
		}
		else
		{
			isFacingRight = false;
		}

		if (player.isGrounded && player.airDashed)
			player.airDashed = false;
		if (!player.isGrounded && player.isDashing)
			isAirDashing = true;

		if (player.isDashing && (Math.Abs (Convert.ToDouble (player.dashStartX - player.transform.position.x)) >= 6.0 || player.isCollidingSides)) {
			isDashing = false;
			player.isDashing = false;
			isAirDashing = false;
		}

        //locking needs to happen last
        transform.position = new Vector3(transform.position.x, transform.position.y, lockedAxisValue);
	}
	
	// FixedUpdate is called every 0.02 seconds (or something).
	// this is where we apply movement to make sure everything is fluid and consistent across time.
	protected void FixedUpdate() {
		player.motor.UpdateMotor(pendingMovementInput);		
		
		if (player.isGrounded) {
			player.transform.rotation *= player.motor.environment.groundRotation;
		}

		if (invulnerable) {
			HandleInvulnerability();
		}
	}
	
	public void CommitMove(Vector3 finalMovement) {
		collisionFlags = player.characterController.Move(finalMovement);
	}

	
	protected void HandleMovementInput() {

		//float vertical = 0f;
		float horizontal = 0f;
		
		bool wasRunning = running;
		Vector3 lastInput = pendingMovementInput;

        //Debug.Log("Horizontal input is: " + horizontal);
		horizontal  = playerInput.xMove;

        if (Mathf.Abs(horizontal) < inputDeadZone || player.CUTSCENE)
        {
            horizontal = 0f;
		}
		else{
			isRunning = true;
			//now we do some checks and corrections depending on how the player is facing
			if (horizontal < 0 && isFacingRight) //facing right, pushing left
			{
				//Debug.Log("Turning Left");
				Vector3 _direction = ((new Vector3(this.transform.position.x - 1, transform.position.y, lockedAxisValue))
				                      - transform.position).normalized;
				Quaternion _lookRotation = Quaternion.LookRotation(_direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 500f);
				isFacingRight = false;
				faceDirection = -1;
			}
			else if (horizontal > 0 && !isFacingRight) //facing left, pushing right
			{
				//Debug.Log("Turning Right");
				Vector3 _direction = ((new Vector3(this.transform.position.x + 1, transform.position.y, lockedAxisValue))
				                      - transform.position).normalized;
				Quaternion _lookRotation = Quaternion.LookRotation(_direction);
				transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 500f);
				isFacingRight = true;
				faceDirection = 1;
			}
		}
		
		isTurning = false;
        //if we ever want to use vertical, it works like horizontal:
        //vertical = Input.GetAxis("Vertical");

		// TODO can this be removed? we say this at the top of the function, and nothing's changed.
		lastInput = pendingMovementInput;
		
        pendingMovementInput = new Vector3(0, 0, faceDirection * horizontal);
        //original vector was (vertical, 0, horizontal), just for if we want to edit in vertical later

		if (!canControl || player.isSunning() || player.isConverting() || player.GetDead ()) {
			pendingMovementInput = Vector3.zero;
		}
		
		if (pendingMovementInput.magnitude == 0) {
			running = false;
			isRunning = false;
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

		if (playerInput.primaryInput == "Keyboard") {
			if(playerInput.firstSeedDown && player.vineUnlocked)
				player.SetCurrentSeed (Player.SeedType.VineSeed);
			else if(playerInput.secondSeedDown && player.treeUnlocked)
				player.SetCurrentSeed (Player.SeedType.TreeSeed);
			else if(playerInput.thirdSeedDown && player.fluerUnlocked)
				player.SetCurrentSeed (Player.SeedType.FlowerSeed);
		} else {
			float horizontal2 = playerInput.xSelect;
			float vertical2 = playerInput.ySelect;
			if (new Vector2 (horizontal2, vertical2).magnitude >= 0.5f) {
				float angle = Vector2.Angle (Vector2.up * -1f, new Vector2 (horizontal2, vertical2));
				if (player.treeUnlocked && angle >= 60 && angle < 180 && horizontal2 > 0) {
					player.SetCurrentSeed (Player.SeedType.TreeSeed);
				} else if (player.fluerUnlocked && angle >= 60 && angle < 180 && horizontal2 < 0) {
					player.SetCurrentSeed (Player.SeedType.FlowerSeed);
				} else if (player.vineUnlocked && angle < 60) {
					player.SetCurrentSeed (Player.SeedType.VineSeed);
				}
			}
		}

		if (!canControl) {
			//Debug.Log (canControl);
			if(player.GetDead()){
				if(playerInput.jumpDown){
					player.SetSpawn(true);
				}
			}else{
				return;
			}
		}

		if (!player.isSunning() && !player.isConverting() && !player.GetDead()) {
			if (playerInput.jumpDown) {
				if(player.GetReadSign())
					ReadSign();
				else{
					if(!player.CUTSCENE)
					Jump ();
				}
			}
			if (playerInput.throwSeedDown && !player.CUTSCENE) {
				ThrowSeed ();
			}
			if (playerInput.dashDown && !player.CUTSCENE) {
				Dash ();
			}
		}
		if (playerInput.sunDown && !player.CUTSCENE) {
			if(canConvert){
				AnimalConvert();
				player.SetConverting(true);
			}
			else{
				Sun();
				player.SetSunning(true);
			}
		}
		if (playerInput.sunUp) {
			isSunLighting = false;
		}
	}
	
	protected void Jump() {
		player.Broadcast("OnJumpRequest");
		player.motor.Jump();
		isJumping = true;
		isRunning = false;
	}

	protected void StopJump() {
		player.Broadcast("OnStopJumpRequest");
		player.motor.StopJump();
		isJumping = false;
	}

	protected void ThrowSeed() {
		player.Broadcast("OnThrowSeedRequest");
		player.motor.ThrowSeed();
		isPlanting = true;
		
	}

	protected void Dash() {
		player.Broadcast("OnDashRequest");
		player.motor.Dash();
		isDashing = true;
		isRunning = false;
	}

	protected void Sun() {
		player.Broadcast("OnSunRequest");
		player.motor.Sun();
		isSunLighting = true;
	}

	protected void AnimalConvert() {
		player.Broadcast("OnConvertRequest");
		player.motor.Convert();
	}

	public void ReadSign(){
		player.motor.ReadSign();
		player.Broadcast ("OnReadSignRequest");
		}

	public void HandleStun()
	{
		if (isStunned) {
			if(stunTimer <= 0)
			{
				canControl = true;
				isStunned = false;
			}
			else
			{
				stunTimer--;
			}
		}
	}

	public void HandleInvulnerability()
	{
		invulCounter--;
		if (invulCounter <= 0) {
			invulnerable = false;
		}

	}

	//essentially check if the blossoms need to be detached/added
	public void checkHealth()
	{
		int currentHealth = transform.GetComponent<Player> ().GetHealth ();
		int currTens = currentHealth / 10;
		currTens -= 1;
		//checking if lost health
		if (currTens < 9 && currTens >= 0) {
			for(int i = 9; i > currTens; i--)
			{
				if(blossoms[i] != null){
					blossoms[i].GetComponent<blossomMover>().detach();
					blossoms[i].name = blossoms[i].name + " (detached)";
					blossoms[i] = null;
				}
			}
		}

		//checking if gained health back
		if (currTens <= 10 && currTens >=0) {
			int i;
			for ( i = 0; (i < currTens) && (blossoms[currTens-i] == null); i++)
     		{
				GameObject newBlossom = (GameObject)Instantiate(blossomPrefab);
				blossoms[currTens-i] = newBlossom;
				blossoms[currTens-i].transform.parent = blossomParent;
				blossoms[currTens-i].name = blossoms[currTens-i].name + " " + (currTens-i);
				blossoms[currTens-i].transform.localPosition = blossomPositions[currTens-i];
				//rotation varies depending on which direction the player, but not locally...?
				blossoms[currTens-i].transform.localRotation = blossomRotations[currTens-i];
			}
		}
		//Debug.Log ("Curr: " + currentHealth + " tens: " + currTens + " prev: " + prevHealth +  prevTens
	}

	public void updateActiveSeeds(int slot, GameObject seed)
	{
		if (activeSeeds [slot] != null) {
			Destroy (activeSeeds [slot]);
		}
		activeSeeds [slot] = seed;

	}

	public void damagePlayer(int damageValue, int hitDirection = 0)
	{
		if (!invulnerable) {
			if(hitDirection == 0)
			{
				hitDirection = faceDirection;
			}
			if (!(gameObject.GetComponent<Player> ().GetHealth () - damageValue <= 0)) {
				gameObject.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * 4, 8f, 0f), 100f);
			}
			canControl = false;
			isStunned = true;
			invulnerable = true;
			stunTimer = 45;
			invulCounter = 75;
			gameObject.GetComponent<Player> ().ReduceHealth (damageValue);
		}
	}
}
