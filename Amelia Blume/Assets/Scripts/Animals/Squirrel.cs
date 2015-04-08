using UnityEngine;
using System.Collections;

public class Squirrel : Animal {
	GameObject[] seeds;
	GameObject[] bushes;
	public GameObject targetSeed;
	public GameObject targetBush;
	public GameObject nextBush;
	GameObject player;
	public bool lookingForSeed = true;
	public bool lookingForBush = false;

	public float walkSpeed = 0.05f;

	public bool chasingPlayer = true;

	[HideInInspector]
	public int faceDirection;
	int rotationCooldown;
	int chargeUpCooldown;
	
	//locking character to axis
	public float lockedAxisValue;
	
	public float speed;
	
	public bool isFacingRight = false;


	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		seeds = GameObject.FindGameObjectsWithTag ("Seed");
		speed = walkSpeed;
		bushes = GameObject.FindGameObjectsWithTag ("Bush");
	}
	
	// Update is called once per frame
	void Update () {
		if (transform.rotation.eulerAngles.y >= 90 && transform.rotation.eulerAngles.y <= 270) {
			faceDirection = 1;
			isFacingRight = true;
		} else {
			faceDirection = -1;
			isFacingRight = false;
		}

		if (!lookingForSeed) 
		{
			DigSeeds ();
		}


		if (lookingForSeed) {
			FindSeeds ();
			if(!chasingPlayer)
				moveToBush();
			else
				moveToPlayer();
		} else {
			lookingForSeed = false;
			GetTargetSeed();
		}

		if (chasingPlayer) {
			if (player.transform.position.y - this.transform.position.y > 1 ||
				player.transform.position.y - this.transform.position.y < -1)
				lookingForBush = true;
			else
				lookingForBush = false;
		} else {
			if (targetSeed.transform.position.y - this.transform.position.y > 1 ||
			    targetSeed.transform.position.y - this.transform.position.y < -1)
				lookingForBush = true;
			else
				lookingForBush = false;
		}

	}

	void moveToBush()
	{
		//Do Stuff
		transform.position = Vector3.MoveTowards (transform.position, targetBush.transform.position, speed);
	}

	void moveToPlayer()
	{
		//Do Stuff
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, speed);
	}

	void FindBush()
	{
		int randomBush = Random.Range (0, bushes.Length);
		while (nextBush == bushes[randomBush]) {
			randomBush++;
			if(randomBush == bushes.Length)
				randomBush = 0;
		}
		nextBush = bushes [randomBush];
		//lookingForBush = false;
	}

	void FindSeeds()
	{
		seeds = GameObject.FindGameObjectsWithTag ("Seed");
		if (seeds.Length > 0) 
			lookingForSeed = false;
		 else
			chasingPlayer = true;
//		Debug.Log (seeds);
	}

	void DigSeeds()
	{
		if(targetSeed != null)
			transform.position = Vector3.MoveTowards (transform.position, targetSeed.transform.position, speed);
	}

	void GetTargetSeed()
	{
		int randomSeed = Random.Range (0, seeds.Length);
		targetSeed = seeds [randomSeed];
		chasingPlayer = false;
	}

	void findNextBush(Vector3 bushPosition)
	{
		//if chasing player find bush closest to her
		//else find bush closest to target seed
	}

	void TeleportToBush()
	{
		if (nextBush == null) {
			if (chasingPlayer)
				findNextBush (player.transform.position);
			else
				findNextBush (targetSeed.transform.position);
		}
		this.transform.position = nextBush.transform.position;
		targetBush = nextBush;
//		findNextBush ();
	}

	void OnCollisionEnter(Collision other)
	{
		Debug.Log ("collision");
		if (other.gameObject.tag == "Seed") {
			Destroy (other.gameObject);
			lookingForSeed = true;
			//GetTargetSeed();
		}


	}

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log ("trigger");
		if (other.gameObject.tag == "Bush") {
			if(other.gameObject == targetBush){
				Debug.Log ("Hit Target Bush");
				if(lookingForBush)
					TeleportToBush();
			}
		}
	}
}
