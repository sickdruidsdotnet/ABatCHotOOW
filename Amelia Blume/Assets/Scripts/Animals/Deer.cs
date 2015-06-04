using UnityEngine;
using System.Collections;

public class Deer : Animal
{

	Player player;
	public Animator anim;

    //how much damage this will do to the player
    public int damageValue;

    //make some variables editable in the editor for debugging
    public float chargeSpeed = 0.1f;
    public float walkSpeed = 0.05f;

    //again, how we do states will change, but for now I'm doing bools
    public bool isCharging;
    public bool isInChargeUp;

	public float deerXSight = 5;

    bool recentlyRotated;
    bool recentlyChargedUp;

	int lockCounter;

	[HideInInspector]
	public int faceDirection;
    int rotationCooldown;
    int chargeUpCooldown;

    //locking character to axis
    public float lockedAxisValue;

    public float speed;

	public bool isFacingRight = false;

    // Use this for initialization
    void Start()
    {

		anim = GetComponentsInChildren<Animator> ()[0];
		animalType = "Deer";
		strength = 2f;
		sporeResistance = 10f;
		sporeLoc = new Vector3 (-1.5f, 1.5f, 0f);

		if (!isInfected) {
			BroadcastMessage ("clearInfection");
		}

		//get the player to easily work with
		GameObject playerObject = GameObject.FindWithTag ("Player");
		if (playerObject == null) {
			Debug.LogError("Deer Error: cannot locate player");
			//If you're seeing this error, you may have tagged the player incorrectly
		}
		else{
			player = playerObject.GetComponent <Player>();
		}


        isCharging = false;
        isInChargeUp = false;
		recentlyRotated = false;
		recentlyChargedUp = false;

		//animations:
		anim.SetBool ("isWalking", true);
		anim.SetBool ("isRunning", false);
		anim.SetBool ("chargingUp", false);
		anim.SetBool ("isRestrained", false);

        //get the value where the animal should be locked to
        lockedAxisValue = this.transform.position.z;

        speed = walkSpeed;

        //this will change to check to see how it's positioned in the editor later
        faceDirection = -1;

        //lock the axis to where it's been placed in the editor
        lockedAxisValue = this.transform.position.z;
		anim = this.GetComponent<Animator>();
		speed = walkSpeed;

		lockCounter = -1;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
		//keep faceDirection Up to Date
		if (transform.rotation.eulerAngles.y >= 90 && transform.rotation.eulerAngles.y <= 270)
		{
			faceDirection = 1;
			isFacingRight = true;
			sporeLoc = new Vector3 (1.5f, 1.5f, 0f);
		}
		else {
			faceDirection = -1;
			isFacingRight = false;
			sporeLoc = new Vector3 (-1.5f, 1.5f, 0f);
		}


		//function to check if the player is in sight
		if (isInfected) {
			checkSeen ();
		} else {
			//do some check to make sure it behaves neutrally
			isCharging = false;
			isInChargeUp = false;
			speed = walkSpeed;
			anim.SetBool ("isRunning", false);
			anim.SetBool ("chargingUp", false);
			anim.SetBool ("isRestrained", false);
			anim.SetBool ("isWalking", true);
			if(optimizer != null && !optimizer.rendered)
				Destroy(gameObject);
		}

        if (isRestrained) {
			anim.SetBool("isWalking", false);
			anim.SetBool ("isRunning", false);
			anim.SetBool ("chargingUp", false);
			anim.SetBool ("isRestrained", true);
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
		}
		else
        {
			MoveRight();
			checkRotate();

            //rotation management
			HandleRotation();

            //charging
            if (isCharging)
            {
                //AI that will slow down for warning, then suddenly charge
                if (isInChargeUp && chargeUpCooldown > 0)
                {
                    chargeUpCooldown--;
                    speed = 0f;
                }
                else
                {
                    if (isInChargeUp)
					{
                        isInChargeUp = false;
						anim.SetBool ("chargingUp", false);
						anim.SetBool ("isRunning", true);
					}

                    speed = chargeSpeed;
                }
            }
        }

		//prevent the player's force from affecting deer after ramming
		if (lockCounter >= 0) {
			lockCounter--;
			if(lockCounter == 0)
			{
				rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
				rigidbody.freezeRotation = true;
			}
		}


        //locking needs to happen last
        if(isInfected)
			transform.position = new Vector3(transform.position.x, transform.position.y, lockedAxisValue);
		else
			transform.position = new Vector3(transform.position.x, transform.position.y, 4);
		//ensure z rotation doesn't exceed reasonable amounts
		float angle = transform.rotation.eulerAngles.z;
		//if rotated greater than 35 degrees
		if (angle > 35f && angle < 325f) {
			if (angle > 35f && angle <= 180f) {
				angle = 35f;
			} else if (angle < 325f && angle >= 180f) {
				angle = 325f;
			}
			angle = angle / 360f;
			transform.rotation = new Quaternion (transform.rotation.x, transform.rotation.y,
		                                    angle, transform.rotation.w);
		}

    }

	//this will bounce the player and cause the deer to look towards them,
	//preventing the player from just running into the deer and being completely safe
	void OnTriggerEnter(Collider other)
	{
		if (isRestrained || !isInfected)
			return;

		if (other.tag == "Player") {
			//make sure the player isn't in stun before bouncing to prevent exponential force addition
			if(!isCharging && (other.GetComponent<PlayerController>().isStunned == false && other.GetComponent<PlayerController>().invulnerable == false))
			{	
				int hitDirection;
				
				if (transform.position.x - other.transform.position.x >= 0)
					hitDirection = -1;
				else
					hitDirection = 1;
				lockCounter = 60;
				rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
				rigidbody.freezeRotation = true;
				player.GetComponent<PlayerController>().damagePlayer(0, hitDirection);
			}
			//rotate the deer to face the player if that's not already the case
			if(((transform.position.x - other.transform.position.x >= 0) && isFacingRight) ||
				((transform.position.x - other.transform.position.x < 0) && !isFacingRight))
			{
				beginRotate();
			}
			else if(isCharging && !isInChargeUp)
			{
				HitPlayer(other.transform.gameObject);
				beginRotate();
			}
		}
	}


	//uses raycasting to see if it needs to turn around or not, bypassing tags
	void checkRotate()
	{
		Ray nearVision = new Ray(new Vector3(transform.position.x + (1.3f * faceDirection),
		                                 transform.position.y + 1f,
		                                 transform.position.z),
		                     Vector3.right * faceDirection);
		RaycastHit[] visionHits;
		Debug.DrawRay(new Vector3(transform.position.x + (1.3f * faceDirection),
		                          transform.position.y + 1f,
		                          transform.position.z),
		              Vector3.right * faceDirection,
					              Color.red, 0);
		visionHits = Physics.RaycastAll (nearVision, 0.5f);
		foreach( RaycastHit hit in visionHits) {
			if(hit.transform.tag == "Player")
			{
				if(isCharging){
					HitPlayer(hit.transform.gameObject);
					beginRotate();
				}
			}
			//ignore itself. this is also where you would ignore other objects
			else if(hit.transform != transform && !hit.collider.isTrigger)
				beginRotate();
		
		}
	}


	//checks if the player is in the deer's sight using raycasting
	void checkSeen()
	{
		float xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);

		// Check to see if player is in the seeing range
		if (xDistance <= deerXSight)
		{

			//check the deer should be charging
			if ( isInfected && !isRestrained && !(isCharging) && !recentlyRotated) {
				Vector3 sightPos = new Vector3(transform.position.x + (1.7f * faceDirection),
				                               transform.position.y + 1.5f,
				                               transform.position.z);
				Vector3 playerHead = new Vector3(player.transform.position.x,
				                                 player.transform.position.y + 1.5f,
				                                 player.transform.position.z);
				//let's check the middle too, just to be sure
				Vector3 playerCore = new Vector3(( player.transform.position.x + playerHead.x)/2,
				                                 ( player.transform.position.y + playerHead.y)/2,
				                                 ( player.transform.position.z + playerHead.z)/2);

				Ray visionFeet = new Ray(sightPos, player.transform.position - sightPos);
				Ray visionHead = new Ray(sightPos, playerHead - sightPos);
				Ray visionCore = new Ray(sightPos, playerCore - sightPos);

				RaycastHit visionHit;
				RaycastHit visionHeadHit;
				RaycastHit visionCoreHit;
				if (Physics.Raycast (visionFeet, out visionHit, 2f * deerXSight)) {
					//draws the vision ray in editor
					Debug.DrawRay(sightPos, player.transform.position - sightPos, Color.green);
					//check to make sure angle isn't too great to see
					//Vector3 rayVector = vision.direction;
					float angle = Vector3.Angle(visionFeet.direction, Vector3.right * faceDirection);
					if(Mathf.Abs(angle) <= 35){
						if(visionHit.transform.tag == "Player" || visionHit.transform.tag == "Blossom")
						{
							//Debug.Log("found player");

							isCharging = true;
							isInChargeUp = true;
							anim.SetBool ("isWalking", false);
							anim.SetBool ("isRunning", false);
							anim.SetBool ("chargingUp", true);
							chargeUpCooldown = 60;
							speed = 0f;
							return;
						}
					}
				}
				//check the head
				if (Physics.Raycast (visionHead, out visionHeadHit, 2f * deerXSight)) {
					//draws the vision ray in editor
					Debug.DrawRay(sightPos, playerHead - sightPos, Color.green);
					//check to make sure angle isn't too great to see
					//Vector3 rayVector = vision.direction;
					float angle = Vector3.Angle(visionHead.direction, Vector3.right * faceDirection);
					if(Mathf.Abs(angle) <= 35){
						if(visionHeadHit.transform != null && visionHeadHit.transform.tag != null && 
						   (visionHeadHit.transform.tag == "Player" || visionHeadHit.transform.tag == "Blossom"))
						{
							//Debug.Log("found player");
							//play audio

							isCharging = true;
							isInChargeUp = true;
							anim.SetBool ("isWalking", false);
							anim.SetBool ("isRunning", false);
							anim.SetBool ("chargingUp", true);
							chargeUpCooldown = 60;
							speed = 0f;
							return;
						}
					}
				}
				//check the core
				if (Physics.Raycast (visionCore, out visionCoreHit, 2f * deerXSight)) {
					//draws the vision ray in editor
					Debug.DrawRay(sightPos, playerCore - sightPos, Color.green);
					//check to make sure angle isn't too great to see
					//Vector3 rayVector = vision.direction;
					float angle = Vector3.Angle(visionCore.direction, Vector3.right * faceDirection);
					if(Mathf.Abs(angle) <= 35){
						if(visionCoreHit.transform != null && visionCoreHit.transform.tag != null && 
						   (visionCoreHit.transform.tag == "Player" || visionCoreHit.transform.tag == "Blossom"))
						{
							//Debug.Log("found player");
							//play audio
							
							isCharging = true;
							isInChargeUp = true;
							anim.SetBool ("isWalking", false);
							anim.SetBool ("isRunning", false);
							anim.SetBool ("chargingUp", true);
							chargeUpCooldown = 60;
							speed = 0f;
							return;
						}
					}
				}
			}
		}
		else
			return;
	}

    void MoveRight()
    {

		//checkRotate ();
		if (!isBeingLured)
			transform.Translate (speed * -1 * sporeModifier, 0, 0);
		else {
			if(target.x > transform.position.x && !isFacingRight)
				beginRotate();
			transform.position = Vector3.MoveTowards (transform.position, target, speed*sporeModifier);
		}


		transform.Translate (speed *-1 * sporeModifier, 0, 0);
    }
	//starts the deer turning around
    public void beginRotate()
    { 
        if (!(recentlyRotated) && !(isInChargeUp) && !(recentlyChargedUp))
        {
            recentlyRotated = true;
			isCharging = false;
            rotationCooldown = 60;
			speed = walkSpeed;
			anim.SetBool ("isRunning", false);
			anim.SetBool ("chargingUp", false);
			anim.SetBool ("isWalking", true);
			//freeze the deer to prevent weird player interaction physics
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
			rigidbody.freezeRotation = true;
		}
    }

	//TODO change this when we get boar animations. THere's no need to physically rotate over time if animation's are good
	void HandleRotation ()
	{
		if (rotationCooldown > 0)
		{
			speed = 0;
			rotationCooldown--;
			transform.Rotate(0f, 3f, 0f);
			if(rotationCooldown <= 0){
				recentlyRotated = false;
				speed = walkSpeed;
				//make sure it's perfectly in profile
				//set the angle to face the proper directions, then assign isFacingRight
				if (transform.rotation.eulerAngles.y > 90 && transform.rotation.eulerAngles.y <= 270)
				{
					transform.rotation = new Quaternion(0f, 180f, transform.rotation.z, transform.rotation.w);
					
					// TODO we should find a better solution for this. Always flipping these values together is not good practice.
					// can we consolidate to one variable? I recognize the advantages of both, but two seems bad. --Derk
					faceDirection = 1;
					isFacingRight = true;
				}
				else
				{
					transform.rotation = new Quaternion(0f, 0f, transform.rotation.z, transform.rotation.w);
					isFacingRight = false;
					faceDirection = -1;
				}
				//unfreeze boar 
				rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
				rigidbody.freezeRotation = true;
				
			}
		}
	}

	//deals the impact and damage to the player
	public void HitPlayer(GameObject player)
	{

		int hitDirection;

		if (transform.position.x - player.transform.position.x >= 0)
			hitDirection = -1;
		else
			hitDirection = 1;
		lockCounter = 60;
		rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
		rigidbody.freezeRotation = true;
		player.GetComponent<PlayerController> ().damagePlayer (damageValue, hitDirection);
		isCharging = false;
		anim.SetBool ("isRunning", false);
		anim.SetBool ("chargingUp", false);
		anim.SetBool ("isWalking", true);

	}

	public void BrokeFree()
	{
		anim.SetBool ("isRestrained", false);
		if(isCharging)
			anim.SetBool("isRunning", true);
		else if(isInChargeUp)
			anim.SetBool("chargingUp", true);
		else
			anim.SetBool ("isWalking", true);
	}


}
