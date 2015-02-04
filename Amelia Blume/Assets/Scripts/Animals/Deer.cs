using UnityEngine;
using System.Collections;

public class Deer : Animal
{

    //how much damage this will do to the player
    public int damageValue;

    //make some variables editable in the editor for debugging
    public float chargeSpeed = 2.5f;
    public float walkSpeed = 1.25f;

    //again, how we do states will change, but for now I'm doing bools
    public bool isCharging;
    public bool isInChargeUp;
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

        if (!isRestrained)
        {
            MoveRight();

            //rotation management
            if (rotationCooldown > 0)
            {
                rotationCooldown--;
                transform.Rotate(0f, 0f, 3f);
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

        if (!(recentlyRotated) && !(isInChargeUp) && !(recentlyChargedUp))
        {
            if (collision.gameObject.tag == "Wall" || collision.gameObject.name == "Deer")
			{
	            recentlyRotated = true;
				isCharging = false;
	            rotationCooldown = 60;
				speed = walkSpeed;
			}
			if(collision.gameObject.tag == "Player")
			{
				if(isCharging){
				//calling player reaction to damage
				}
				recentlyRotated = true;
				isCharging = false;
				rotationCooldown = 60;
				speed = walkSpeed;
			}
        }

    }

	void OnTriggerStay(Collider collider)
	{
		if ( isInfected && !isRestrained && !(isCharging) && !recentlyRotated && collider.tag == "Player") {
			//Debug.Log("found player");
			isCharging = true;
			isInChargeUp = true;
			chargeUpCooldown = 60;
		}
	}

    void MoveRight()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(speed * faceDirection, 0, 0);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

		//Debug.Log ("Target Velocity: " + targetVelocity);

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }
}
