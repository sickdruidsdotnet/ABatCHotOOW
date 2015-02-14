using UnityEngine;
using System.Collections;

public class Deer : Animal
{

	Player player;

    //how much damage this will do to the player
    public int damageValue;

    //make some variables editable in the editor for debugging
    public float chargeSpeed = 2.5f;
    public float walkSpeed = 1.25f;

    //again, how we do states will change, but for now I'm doing bools
    public bool isCharging;
    public bool isInChargeUp;

	public float deerYSight = 3;
	public float deerXSight = 5;

    bool recentlyRotated;
    bool recentlyChargedUp;

    public int faceDirection;
    int rotationCooldown;
    int chargeUpCooldown;

    //locking character to axis
    public float lockedAxisValue;

    public float speed;
    public float maxVelocityChange;

    // Use this for initialization
    void Start()
    {

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

    }

    // Update is called once per frame
    void Update()
    {
		//keep faceDirection Up to Date
		if (transform.rotation.eulerAngles.y >= 0 && transform.rotation.eulerAngles.y <= 180)
		{
			faceDirection = 1;
		}
		else {
			faceDirection = -1;
		}


		//function to check if the player is in sight
		checkSeen ();

        if (!isRestrained)
        {
			//update face direction


            MoveRight();

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

        //locking needs to happen last
        transform.position = new Vector3(transform.position.x, transform.position.y, lockedAxisValue);
    }

    //this will be used for rotating when hitting walls mainly
    void OnCollisionStay(Collision collision)
    {
		//Debug.Log ("Colliding with " + collision.gameObject.tag);
        if (collision.gameObject.tag == "Wall" || collision.gameObject.name == "Deer")
		{
            beginRotate();
		}
		if(collision.gameObject.tag == "Player")
		{
			if(isCharging){
				HitPlayer(collision.gameObject);
			}
            beginRotate();
		}

    }

	void checkSeen()
	{

		float yDistance = Mathf.Abs(transform.position.y - player.transform.position.y);
		float xDistance = Mathf.Abs(transform.position.x - player.transform.position.x);
		
		
		// Check to see if player is in the seeing range
		if (yDistance <= deerYSight && xDistance <= deerXSight)
		{
			//check the deer should be charging
			if ( isInfected && !isRestrained && !(isCharging) && !recentlyRotated) {

				//check if deer is facing the right way to see the player
				if (transform.position.x - player.transform.position.x < 0 && faceDirection == -1)
				{
					//Debug.Log("found player to the right");
					isCharging = true;
					isInChargeUp = true;
					chargeUpCooldown = 60;
					return;
				}
				else if (transform.position.x - player.transform.position.x > 0 && faceDirection == 1)
				{
					//Debug.Log("found player to the left");
					isCharging = true;
					isInChargeUp = true;
					chargeUpCooldown = 60;
					return;
				}
			}
		}
		else
			return;
	}

    void MoveRight()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(speed * -1f , 0, 0);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

		//Debug.Log ("Target Velocity: " + targetVelocity);

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        //rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
		transform.Translate (-0.1f, 0, 0);
    }

    public void beginRotate()
    { 
        if (!(recentlyRotated) && !(isInChargeUp) && !(recentlyChargedUp))
        {
	            recentlyRotated = true;
				isCharging = false;
	            rotationCooldown = 60;
				speed = walkSpeed;
		}
    }

	public void HitPlayer(GameObject player)
	{
		player.GetComponent<Player> ().ReduceHealth (damageValue);
		player.GetComponent<PlayerController>().canControl = false;
		player.GetComponent<PlayerController>().stunTimer = 45;

		int hitDirection;

		if (transform.position.x - player.transform.position.x >= 0)
			hitDirection = -1;
		else
			hitDirection = 1;
		//prevent the player's force vector from affecting the deer
		Physics.IgnoreCollision(player.collider, collider);
		player.GetComponent<ImpactReceiver> ().AddImpact (new Vector3(hitDirection * 4, 8f, 0f), 100f);

	}
}
