using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : BaseBehavior {
	
	
	public float inputDeadZone = 0.05f;
    
    public float lockedAxisValue;
		
	public bool running = false;
	public bool alwaysRun = false;
	
    //do we want sliding? could be cool...
	public bool sliding = false;
	public bool slidingFast = false;

    public bool isFacingRight;
    int faceDirection;

	public bool canControl;
	public int stunTimer;

	public bool isTurning = false;
	public float turnDirection = 0f;

	public GameObject blossomPrefab;	
	public GameObject[] blossoms;
	public Vector3[] blossomPositions;
	public Quaternion[] blossomRotations;

	protected Vector3 pendingMovementInput;
	public CollisionFlags collisionFlags;
	
    void Start()
    {

		blossoms = new GameObject[10];
		blossomPositions = new Vector3[10];
		blossomRotations = new Quaternion[10];
		int i = 0;
		foreach (Transform child in transform) 
		{
			blossomMover tempBlossom = child.GetComponent<blossomMover>();
			if(tempBlossom != null)
			{
				blossoms[i] = child.gameObject;
				blossoms[i].name = blossoms[i].name + " " + i;
				blossomPositions[i] = blossoms[i].transform.localPosition;
				blossomRotations[i] = blossoms[i].transform.localRotation;
				i++;
			}
		}

        //get the value to lock the player to. Should Iddeally be 0 in the editor
        lockedAxisValue = this.transform.position.z;

        //set the angle to face the proper directions, then assign isFacingRight
        if (transform.rotation.eulerAngles.y >= 0 && transform.rotation.eulerAngles.y <= 180)
        {
            Vector3 _direction = ((new Vector3(this.transform.position.x + 1f, transform.position.y, lockedAxisValue))
                                    - transform.position).normalized;
            Quaternion _lookRotation = Quaternion.LookRotation(_direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, _lookRotation, Time.deltaTime * 50f);
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

    }
	
	protected void Update() {

		if (Camera.main == null) {
			return;		
		}

		//check to see if blossoms are up-to-date
		checkHealth ();

		//debug, remove this when we get it properly detaching via health drops
		if (Input.GetKey ("1")) {
			for(int i = 0; i < 10; i++)
				blossoms[i].GetComponent<blossomMover>().detach ();
		}

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

        //locking needs to happen last
        transform.position = new Vector3(transform.position.x, transform.position.y, lockedAxisValue);
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

		//float vertical = 0f;
		float horizontal = 0f;
		
		bool wasRunning = running;
		Vector3 lastInput = pendingMovementInput;
			
		running = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
				
		horizontal  = Input.GetAxis("Horizontal");
        //Debug.Log("Horizontal input is: " + horizontal);
        if (Mathf.Abs(horizontal) < inputDeadZone)
        {
            horizontal = 0f;
		}
		else{
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
		
		lastInput = pendingMovementInput;
        pendingMovementInput = new Vector3(0, 0, faceDirection * horizontal);
        //original vector was (vertical, 0, horizontal), just for if we want to edit in vertical later

		if (!canControl) {
			pendingMovementInput = Vector3.zero;
		}
		
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

		if(!canControl)
			return;

		if (Input.GetButtonDown("Jump")) {
			Jump();
		}
		if (Input.GetButtonDown("ThrowSeed")) {
			ThrowSeed();
		}
	}
	
	protected void Jump() {
		player.Broadcast("OnJumpRequest");
		player.motor.Jump();
	}

	protected void ThrowSeed() {
		player.Broadcast("OnThrowSeedRequest");
		player.motor.ThrowSeed();
	}

	public void HandleStun()
	{
		if (!canControl) {
			if(stunTimer <= 0)
			{
				canControl = true;
			}
			else
			{
				stunTimer--;
			}
		}
	}

	//essentially check if the blossoms need to be detached/added
	void checkHealth()
	{
		int currentHealth = transform.GetComponent<Player> ().GetHealth ();
		int currTens = currentHealth / 10;
		//checking if lost health
		if (currTens < 9 && blossoms[currTens+1] != null) {
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
		if (currTens != 10 && blossoms [currTens] == null) {
			int i;
			for ( i = 0; (i < currTens) && (blossoms[currTens-i] == null); i++)
     		{
				GameObject newBlossom = (GameObject)Instantiate(blossomPrefab);
				blossoms[currTens-i] = newBlossom;
				blossoms[currTens-i].transform.parent = transform;
				blossoms[currTens-i].name = blossoms[currTens-i].name + " " + (currTens-i);
				blossoms[currTens-i].transform.localPosition = blossomPositions[currTens-i];
				//rotation varies depending on which direction the player, but not locally...?
				blossoms[currTens-i].transform.localRotation = blossomRotations[currTens-i];
			}
		}
		//Debug.Log ("Curr: " + currentHealth + " tens: " + currTens + " prev: " + prevHealth +  prevTens
	}
}