using UnityEngine;
using System.Collections;

public class Boar : Animal
{
	Player player;
	public Animator anim;
	
	//how much damage this will do to the player
	public int damageValue = 100;
	
	//make some variables editable in the editor for debugging
	public float baseSpeed;
	public float speed;
	float idealDistance = 10f;

	public bool hasLeaped;

	public bool isFacingRight = false;
	[HideInInspector]
	public int faceDirection;
	
	//locking character to axis
	public float lockedAxisValue;
	
	//audio variables
	public AudioClip spotPlayer1;
	private AudioSource source;

	int lockCounter = -1;

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
		
		//get the value where the animal should be locked to
		lockedAxisValue = transform.position.z;
		
		speed = baseSpeed;

		//this will change to check to see how it's positioned in the editor later
		faceDirection = -1;

		anim = GetComponentInChildren<Animator> ();
		anim.SetBool ("IsCharging", true);
		anim.SetBool ("IsWalking", false);
		anim.SetBool ("IsRestrained", false);

		source.PlayOneShot (spotPlayer1, 3F);
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
			if (isRestrained) {
				rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
				rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
				if(!anim.GetBool("IsRestrained")){
					anim.SetBool ("IsRestrained", true);
					anim.SetBool ("IsCharging", false);
					anim.SetBool ("IsWalking", false);
				}
			}
			else
			{
				if (!anim.GetBool("IsCharging")){
					anim.SetBool ("IsCharging", true);
					anim.SetBool ("IsWalking", false);
					anim.SetBool ("IsRestrained", false);
				}
				MoveRight();
			}
		} else {
			MoveRight();
			if (!anim.GetBool("IsWalking")){
				anim.SetBool ("IsWalking", true);
				anim.SetBool ("IsCharging", false);
				anim.SetBool ("IsRestrained", false);
			}
			//walk away from amelia stuff, probably to the left
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

		if (isSpored) {
			strength = 3;
		}
	}
	
	//this will bounce the player and cause the boar to look towards them,
	//preventing the player from just running into the boar and being completely safe
	void OnTriggerEnter(Collider other)
	{
		if (isRestrained || !isInfected)
			return;
		
		if (other.tag == "Player") {

			HitPlayer(other.transform.gameObject);

		}
	}

	void OnTriggerStay(Collider other){
		if (isRestrained || !isInfected)
			return;
		
		if (other.tag == "Player") {
			
			HitPlayer(other.transform.gameObject);
			
		}
	}
	

	void MoveRight()
	{
		if (isInfected) {
			//special charging stuff would go here
			//move slightly slower when closer to the player for more tension
			float xDistance = Mathf.Abs (player.transform.position.x - transform.position.x);
			// ideal distance is ~10 units away. This should be tweaked as needed, but that's basically a whole boar
			// away from the boar's outer hitbox
			float dif = xDistance - idealDistance;
			//if xdif is negative, then the player is too close, else the player is too far away
			//these are magic numbers, adjust as needed. Clamping so it never hits speed of 0 or supersonic
			speed = Mathf.Clamp ((baseSpeed * sporeModifier) + (dif * 0.013f), 0.07f, 0.25f);

			transform.Translate ((speed * -1 * sporeModifier), 0, 0);
			//animation["Walking"].enabled = true;
			//anim.SetBool ("isRunning", true);
		} else {
			transform.Translate (-0.03f, 0, 0);
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

		player.GetComponent<PlayerController> ().damagePlayer (damageValue, hitDirection);
		lockCounter = 60;
		rigidbody.constraints = RigidbodyConstraints.FreezeRotation;
		
	}

	//this is for leaping logic
	/*void OnCollisionStay()
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
	}*/

	//the boar leaps if the player is higher than the boar while it's charging and within and X radius
	public void leap()
	{
		hasLeaped = true;
		rigidbody.velocity = new Vector3(rigidbody.velocity.x, rigidbody.velocity.y + 1f, rigidbody.velocity.z);

	}

}
