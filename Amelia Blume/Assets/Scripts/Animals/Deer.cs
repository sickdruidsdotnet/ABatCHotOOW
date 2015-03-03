using UnityEngine;
using System.Collections;

public class Deer : Animal
{

	Player player;
	Animator anim;

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
		sporeResistance = 10f;
		sporeLoc = new Vector3 (-1.5f, 1.5f, 0f);

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
    void Update()
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
		checkSeen ();

        if (isRestrained) {
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
		}
		else
        {

			MoveRight();
			checkRotate();

            //rotation management

            if (rotationCooldown > 0)
            {
                rotationCooldown--;
                transform.Rotate(0f, 3f, 0f);
                isCharging = false;
                isInChargeUp = false;
            }
            else
            {
                recentlyRotated = false;
				//unfreeze deer 
				rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionY;
				rigidbody.constraints &= ~RigidbodyConstraints.FreezePositionX;
				rigidbody.freezeRotation = true;
            }
            


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
        transform.position = new Vector3(transform.position.x, transform.position.y, lockedAxisValue);
    }

	//this will bounce the player and cause the deer to look towards them,
	//preventing the player from just running into the deer and being completely safe
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
				rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
				rigidbody.freezeRotation = true;
				other.GetComponent<ImpactReceiver> ().AddImpact (new Vector3(hitDirection * 4, 8f, 0f), 100f);
				other.GetComponent<PlayerController>().canControl = false;
				other.GetComponent<PlayerController>().stunTimer = 30;
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
			else if(!hit.transform != transform && !hit.collider.isTrigger)
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
				Ray vision = new Ray(new Vector3(transform.position.x + (2f * faceDirection),
				                                 transform.position.y + 1.5f,
				                                 transform.position.z),
				                     Vector3.right * faceDirection);
				RaycastHit visionHit;
				if (Physics.Raycast (vision, out visionHit, deerXSight)) {
					//draws the vision ray in editor
					/*Debug.DrawRay(new Vector3(transform.position.x + (2f * faceDirection),
					                          transform.position.y + 1.5f,
					                          transform.position.z),
					              Vector3.right * faceDirection,
					              Color.green, 1f);*/
					if(visionHit.transform.tag == "Player" || visionHit.transform.tag == "Blossom")
					{
						//Debug.Log("found player");
						isCharging = true;
						isInChargeUp = true;
						chargeUpCooldown = 60;
						speed = 0f;
						return;
					}
					/*else
						Debug.Log("found " + visionHit.transform.name);*/

				}
			}
		}
		else
			return;
	}

    void MoveRight()
    {
		transform.Translate (speed *-1 * sporeModifier, 0, 0);
		//animation["Walking"].enabled = true;
		anim.SetBool ("isRunning", true);
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
			//freeze the deer to prevent weird player interaction physics
			rigidbody.constraints = RigidbodyConstraints.FreezePositionY;
			rigidbody.constraints = RigidbodyConstraints.FreezePositionX;
			rigidbody.freezeRotation = true;
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
		//don't add the impact if player is about to die
		if (!(player.GetComponent<Player> ().GetHealth () - damageValue <= 0)) {
			player.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * 4, 8f, 0f), 100f);
		}
		player.GetComponent<PlayerController>().canControl = false;
		player.GetComponent<PlayerController>().stunTimer = 45;
		player.GetComponent<Player> ().ReduceHealth (damageValue);
		isCharging = false;

	}
}
