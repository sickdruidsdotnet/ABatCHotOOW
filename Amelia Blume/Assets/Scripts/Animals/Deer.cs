using UnityEngine;
using System.Collections;

public class Deer : Animal
{

    //how much damage this will do
    public int damageValue;

    //again, how we do states will change, but for now I'm doing bools
    public bool isCharging;
    public bool isInChargeUp;
    public bool isFacingRight;
    public bool isGrounded;
    bool recentlyRotated;
    bool recentlyChargedUp;

    int faceDirection;
    int rotationCooldown;
    int chargeUpCooldown;

    //locking character to axis
    public float lockedAxisValue;

    public float speed;
    public float maxVelocityChange;
    float distToGround;

    // Use this for initialization
    void Start()
    {

        isCharging = false;
        isInChargeUp = false;

        //whatever axis that it shouldn't be traveling in. I think it's Z in Unity
        lockedAxisValue = this.transform.position.z;

        speed = 1f;
        //this will change to check to see how it's positioned in the editor later
        faceDirection = -1;

        distToGround = collider.bounds.extents.y;

        //lock the axis to where it's been placed in the editor
        lockedAxisValue = this.transform.position.z;

    }

    // Update is called once per frame
    void Update()
    {
        isGrounded = Physics.Raycast(transform.position, -Vector3.up, distToGround + 0.1f);
        /*if (isGrounded)
        {
            rigidbody.useGravity = false;
        }
        else
        {
            rigidbody.useGravity = true;
        }*/
        MoveRight();

        //rotation management
        if (rotationCooldown > 0)
        {
            rotationCooldown--;
            Vector3 quickRotate = transform.rotation.eulerAngles;
            quickRotate.y += 3.0f;
            transform.rotation = new Quaternion(quickRotate.z / 360f, quickRotate.y / 360f,
                                                                    quickRotate.z / 360f, 0);
            isCharging = false;
            isInChargeUp = false;
        }
        else
            recentlyRotated = false;



        //locking needs to happen last
        transform.position = new Vector3(transform.position.x, transform.position.y, lockedAxisValue);
    }

    //this will be used for rotating when hitting walls mainly
   /* void OnCollisionEnter(Collision collision)
    {
        if (!(recentlyRotated) && !(isInChargeUp) && !(recentlyChargedUp))
        {
            faceDirection *= -1;
            recentlyRotated = true;
            rotationCooldown = 60;
        }

    }*/

    void MoveRight()
    {
        // Calculate how fast we should be moving
        Vector3 targetVelocity = new Vector3(speed * faceDirection, 0, 0);
        targetVelocity = transform.TransformDirection(targetVelocity);
        targetVelocity *= speed;

        // Apply a force that attempts to reach our target velocity
        Vector3 velocity = rigidbody.velocity;
        Vector3 velocityChange = (targetVelocity - velocity);
        velocityChange.x = Mathf.Clamp(velocityChange.x, -maxVelocityChange, maxVelocityChange);
        velocityChange.z = Mathf.Clamp(velocityChange.z, -maxVelocityChange, maxVelocityChange);
        velocityChange.y = 0;
        rigidbody.AddForce(velocityChange, ForceMode.VelocityChange);
    }

        //controller.SimpleMove(new Vector3(speed * faceDirection, 0, 0));
        /*transform.position = new Vector3(transform.position.x + (speed * faceDirection),
                                          transform.position.y,
                                          lockedAxisValue);*/
}
