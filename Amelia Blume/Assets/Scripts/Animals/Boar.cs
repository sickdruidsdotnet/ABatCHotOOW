using UnityEngine;
using System.Collections;

public class Boar : Animal
{
	Player player;
	//Animator anim;
	
	//how much damage this will do to the player
	public int damageValue;
	
	//make some variables editable in the editor for debugging
	public float chargeSpeed = 0.08f;
	public float walkSpeed = 0.03f;
	
	//again, how we do states will change, but for now I'm doing bools
	public bool isCharging;
	public bool isInChargeUp;
	
	public float boarXSight = 10;

	bool recentlyRotated;
	bool recentlyChargedUp;
	public bool hasLeaped;

	public int rampageCount = 3;
	int lockCounter;
	
	[HideInInspector]
	public int faceDirection;
	int rotationCooldown;
	int chargeUpCooldown;
	
	//locking character to axis
	public float lockedAxisValue;
	
	public float speed;
	
	public bool isFacingRight = false;
	
	//audio variables
	public AudioClip spotPlayer1;
	private AudioSource source;

	// Use this for initialization
	void Start()
	{
		source = GetComponent<AudioSource>();

		Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider>(), collider, true);
		Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController>(), collider, true);


		animalType = "Boar";
		strength = 7f;
		sporeResistance = 10f;
		sporeLoc = new Vector3 (-3.3f, 0.5f, 0f);
		
		//get the player to easily work with
		GameObject playerObject = GameObject.FindWithTag ("Player");
		if (playerObject == null) {
			Debug.LogError("Boar Error: cannot locate player");
			//If you're seeing this error, you may have tagged the player incorrectly
		}
		else{
			player = playerObject.GetComponent <Player>();
		}
		
		
		isCharging = false;
		isInChargeUp = false;
		recentlyRotated = false;
		recentlyChargedUp = false;
		
		//get the value where the animal should be locked to
		lockedAxisValue = this.transform.position.z;
		
		speed = walkSpeed;
		
		//this will change to check to see how it's positioned in the editor later
		faceDirection = -1;
		
		//lock the axis to where it's been placed in the editor
		lockedAxisValue = this.transform.position.z;
		//anim = this.GetComponent<Animator>();
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
			rampageCount = 3;
			isInChargeUp = false;
			speed = walkSpeed;
		}

		if (isRestrained) {
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
		}
		else
		{
			MoveRight();
			checkRotate();
			HandleRotation();
			//rotation management
			
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
						isInChargeUp = false;
					
					speed = chargeSpeed;

					//check if leap it should leap
					if(!hasLeaped && Mathf.Abs(player.transform.position.x - transform.position.x) >= 3.0 && 
					   player.transform.position.y > transform.position.y)
					{
						//make sure it's only leaping when facing the player
						if((player.transform.position.x <= transform.position.x && !isFacingRight) || 
						   (player.transform.position.x >= transform.position.x && isFacingRight)){
							leap ();
						}
					}
				}

			}
		}
		
		//prevent the player's force from affecting boar after ramming
		if (lockCounter >= 0) {
			lockCounter--;
			if(lockCounter == 0)
			{
				rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
			}
		}

		//checking if it has hit the ground via raycasting
		if(hasLeaped){
			//check to see if the velocity has been 0 for two frames
			if(rigidbody.velocity.y == 0)
				hasLeaped = false;
		}

		//locking needs to happen last
		//if it's not infected it should be behind in the background
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
	
	//this will bounce the player and cause the boar to look towards them,
	//preventing the player from just running into the boar and being completely safe
	void OnTriggerEnter(Collider other)
	{
		if (isRestrained || !isInfected)
			return;
		
		if (other.tag == "Player") {
			//make sure the player isn't in stun before bouncing to prevent exponential force addition
			if(!isCharging && (other.GetComponent<PlayerController>().stunTimer <= 0 || other.GetComponent<PlayerController>().canControl == true))
			{	
				int hitDirection;
				
				if (transform.position.x - other.transform.position.x >= 0)
					hitDirection = -1;
				else
					hitDirection = 1;
				lockCounter = 60;
				rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
				rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
				other.GetComponent<ImpactReceiver> ().AddImpact (new Vector3(hitDirection * 4, 8f, 0f), 100f);
				other.GetComponent<PlayerController>().canControl = false;
				other.GetComponent<PlayerController>().stunTimer = 30;
			}
			//rotate the boar to face the player if that's not already the case
			if(((transform.position.x - other.transform.position.x >= 0) && isFacingRight) ||
			   ((transform.position.x - other.transform.position.x < 0) && !isFacingRight))
			{
				beginRotate();
			}
			else if(isCharging && !isInChargeUp)
			{
				beginRotate();
				HitPlayer(other.transform.gameObject);
			}
		}
	}
	
	
	//uses raycasting to see if it needs to turn around or not, bypassing tags
	void checkRotate()
	{

		Ray nearVision = new Ray(new Vector3(transform.position.x + (2.3f * faceDirection),
		                                     transform.position.y + 0.5f,
		                                     transform.position.z),
		                         Vector3.right * faceDirection);
		RaycastHit[] visionHits;
		Debug.DrawRay(new Vector3(transform.position.x + (2.3f * faceDirection),
		                          transform.position.y + 0.5f,
		                          transform.position.z),
		              Vector3.right * faceDirection,
		              Color.blue, 0);
		visionHits = Physics.RaycastAll (nearVision, 0.5f);
		foreach( RaycastHit hit in visionHits) {
			if(hit.transform.tag == "Player")
			{
				if(isCharging){
					HitPlayer(hit.transform.gameObject);
				}
				beginRotate();
			}
			//ignore itself. this is also where you would ignore other objects
			else if(!hit.transform != transform && !hit.collider.isTrigger)
				beginRotate();
			
		}
	}
	
	
	//checks if the player is in the boar's sight using raycasting
	void checkSeen()
	{
		float xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
		
		// Check to see if player is in the seeing range
		if (xDistance <= boarXSight)
		{
			
			//check the boar should be charging
			if ( isInfected && !isRestrained && !(isCharging) && !recentlyRotated) {
				Vector3 sightPos = new Vector3(transform.position.x + (3f * faceDirection),
				                               transform.position.y + 0.5f,
				                               transform.position.z);
				Vector3 playerHead = new Vector3(player.transform.position.x,
				                                 player.transform.position.y + 1.5f,
				                                 player.transform.position.z);
				Ray visionFeet = new Ray(sightPos, player.transform.position - sightPos);
				Ray visionHead = new Ray(sightPos, playerHead - sightPos);
				RaycastHit visionHit;
				RaycastHit visionHeadHit;
				//check the feet
				if (Physics.Raycast (visionFeet, out visionHit, 2f * boarXSight)) {
					//draws the vision ray in editor
					Debug.DrawRay(sightPos, player.transform.position - sightPos, Color.green);
					//check to make sure angle isn't too great to see
					//Vector3 rayVector = vision.direction;
					float angle = Vector3.Angle(visionFeet.direction, Vector3.right * faceDirection);
					if(Mathf.Abs(angle) <= 45){
						if(visionHit.transform.tag == "Player" || visionHit.transform.tag == "Blossom")
						{
							//Debug.Log("found player");
							//play audio
							source.PlayOneShot(spotPlayer1, 1F);

							isCharging = true;
							isInChargeUp = true;
							chargeUpCooldown = 60;
							speed = 0f;
							return;
						}
					}
				}
				//check the head
				if (Physics.Raycast (visionHead, out visionHeadHit, 2f * boarXSight)) {
					//draws the vision ray in editor
					Debug.DrawRay(sightPos, playerHead - sightPos, Color.green);
					//check to make sure angle isn't too great to see
					//Vector3 rayVector = vision.direction;
					float angle = Vector3.Angle(visionHead.direction, Vector3.right * faceDirection);
					if(Mathf.Abs(angle) <= 45){
						if(visionHeadHit.transform != null && visionHeadHit.transform.tag != null && 
						   (visionHeadHit.transform.tag == "Player" || visionHeadHit.transform.tag == "Blossom"))
						{
							//Debug.Log("found player");
							//play audio
							source.PlayOneShot(spotPlayer1, 1F);
							
							isCharging = true;
							isInChargeUp = true;
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
		if (!isBeingLured) {
			if(recentlyRotated){
				transform.Translate ((walkSpeed * -1 * sporeModifier), 0, 0);
			}
			else{
				transform.Translate ((speed * -1 * sporeModifier), 0, 0);
			}
		}
		else {
			if(target.x > transform.position.x && !isFacingRight)
				beginRotate();
			transform.position = Vector3.MoveTowards (transform.position, target, speed*sporeModifier);
		}
		//animation["Walking"].enabled = true;
		//anim.SetBool ("isRunning", true);
	}
	//starts the boar turning around
	public void beginRotate()
	{ 

		if (!(recentlyRotated) && !(isInChargeUp) && !(recentlyChargedUp))
		{
			recentlyRotated = true;
			if(isCharging)
			{
				if(rampageCount > 0)
				{
					rampageCount--;
				}
				else{
					isCharging = false;
					rampageCount = 3;
					speed = walkSpeed;
				}

			}
			rotationCooldown = 60;
			//freeze the boar to prevent weird player interaction physics
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
			rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		}
	}

	//TODO change this when we get boar animations. THere's no need to physically rotate over time if animation's are good
	void HandleRotation ()
	{
		if (rotationCooldown > 0)
		{
			rotationCooldown--;
			transform.Rotate(0f, 3f, 0f);
			if(rotationCooldown <= 0){
				recentlyRotated = false;
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
				rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
				rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
				
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
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		//don't add the impact if player is about to die
		if (!(player.GetComponent<Player> ().GetHealth () - damageValue <= 0)) {
			player.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * 4, 8f, 0f), 100f);
		}
		player.GetComponent<PlayerController>().canControl = false;
		player.GetComponent<PlayerController>().stunTimer = 45;
		player.GetComponent<Player> ().ReduceHealth (damageValue);
		isCharging = false;
		isInChargeUp = false;
		rampageCount = 3;
		speed = walkSpeed;
		
	}

	void OnCollisionStay()
	{
		//make sure it's touching the gorund before refreshing its leap
		if (GetComponentInChildren<EdgeCheckerScript> ().count > 0) {
			hasLeaped = false;
			
			//for turning around after passing the player while rampaging, only when grounded
			//checks if the boar is to the left/right of the player and is charging the same direction
			if( isCharging && ((player.transform.position.x > transform.position.x && !isFacingRight) || 
			   (player.transform.position.x < transform.position.x && isFacingRight))){
				beginRotate();
			}
		}
	}

	//the boar leaps if the player is higher than the boar while it's charging and within and X radius
	public void leap()
	{
		hasLeaped = true;
		rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + 1f, rigidbody.velocity.z);

	}

}
