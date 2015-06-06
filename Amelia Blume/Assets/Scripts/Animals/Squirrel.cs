using UnityEngine;
using System.Collections;

public class Squirrel : Animal {
	GameObject[] seeds;
	GameObject[] bushes;
	Seed currentSeed;
	public GameObject targetSeed;
	public GameObject targetBush; //From Bush
	public GameObject nextBush; //To Bush
	GameObject player;
	public bool lookingForSeed = true;
	public bool lookingForBush = false;

	public float walkSpeed = 0.05f;

	public bool chasingPlayer = true;
	//int seedHealth = 120;

	//[HideInInspector]
	public int faceDirection;
	int rotationCooldown;
	int chargeUpCooldown;
	int hitCoolDown = -1;
	int damageValue = 1;
	
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
		FindBush ();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), false);
		CheckRotation ();
		if (targetSeed == null) {
			GetTargetSeed();
			lookingForSeed = true;
		} else {
			lookingForSeed = false;
		}

		if (!lookingForSeed) 
		{
			DigSeeds ();
		}


		if (lookingForSeed) {
			GetTargetSeed ();
			if(!chasingPlayer)
				moveToBush();
			else
				moveToPlayer();
		} else {
			lookingForSeed = false;
			GetTargetSeed();
		}

		if (chasingPlayer) {
			if (Mathf.Abs (player.transform.position.y - this.transform.position.y) > 3) {
				lookingForBush = true;
			} else {
				lookingForBush = false;
			}
		} else {
			if (Mathf.Abs (targetSeed.transform.position.y - this.transform.position.y) > 3) {
				lookingForBush = true;
			} else {
				lookingForBush = false;
			}
		}

		if (lookingForBush) {
			//Debug.Log (Mathf.Abs (player.transform.position.y - this.transform.position.y));
			if(chasingPlayer)
				findNextBush(player.transform.position);
			else
				findNextBush(targetSeed.transform.position);
		}

	}

	void moveToBush()
	{
		//Do Stuff
		FindBush ();
		if (targetBush.transform.position.x > this.transform.position.x) {
			isFacingRight = true;
		} else {
			isFacingRight = false;
		}
		transform.position = Vector3.MoveTowards (transform.position, targetBush.transform.position, speed);
	}

	void moveToPlayer()
	{
		if (lookingForBush) {
			moveToBush();
		} else {
			if (player.transform.position.x > this.transform.position.x) {
				isFacingRight = true;
			} else {
				isFacingRight = false;
			}
			Vector3 playerPos = new Vector3 (player.transform.position.x, this.transform.position.y, this.transform.position.z);
			transform.position = Vector3.MoveTowards (transform.position, playerPos, speed);
		}
	}

	void FindBush()
	{
		int closestBush = 0;
		for (int i = 0; i < bushes.Length; i++) {
			if(Vector3.Distance(this.transform.position, bushes[i].transform.position) 
			   < Vector3.Distance(this.transform.position, bushes[closestBush].transform.position)){
				closestBush = i;
			}
		}
		targetBush = bushes[closestBush];
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
		//Debug.Log ("Dig Seeds");
		if (targetSeed != null) {
			if (lookingForBush) {
				moveToBush ();
			} else {
				if (targetSeed.transform.position.x > this.transform.position.x) {
					isFacingRight = true;
				} else {
					isFacingRight = false;
				}
				transform.position = Vector3.MoveTowards (transform.position, targetSeed.transform.position, speed);
			}
		} else {
			GetTargetSeed();
		}
	}

	void GetTargetSeed()
	{
//		Debug.Log ("Get Target Seed");
		FindSeeds ();
		if (seeds.Length > 0) {
			int randomSeed = Random.Range (0, seeds.Length);
			targetSeed = seeds [randomSeed];
			currentSeed = targetSeed.GetComponent<Seed> ();
			chasingPlayer = false;
			lookingForSeed = false;
		} else {
			chasingPlayer = true;
			lookingForSeed = true;
		}
	}

	void findNextBush(Vector3 targetPosition)
	{
		int closestBush = 0;
		for (int i = 0; i < bushes.Length; i++) {
			if(Vector3.Distance(targetPosition, bushes[i].transform.position) < Vector3.Distance(targetPosition, bushes[closestBush].transform.position)){
				closestBush = i;
			}
		}
		nextBush = bushes[closestBush];
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

	void CheckRotation(){ 

		Vector3 rotation = this.transform.eulerAngles;
		//Debug.Log ("Y :" + rotation.y);


		if (isFacingRight && rotation.y != 90) {
			//Debug.Log("Difference from 90:" + "" +  (90 - rotation.y));
			if(Mathf.Abs(90 - rotation.y) < 5){
				this.transform.rotation = Quaternion.Euler(0,90,0);
			}else{
				//Debug.Log(rotation.y - 270f);
				this.transform.Rotate (0, -5f, 0);
			}
		} else if (!isFacingRight && rotation.y != 270) {
			//Debug.Log("Difference from 270: " + "" + (270 - rotation.y));
			if(Mathf.Abs(270 - rotation.y) < 5){
				this.transform.rotation = Quaternion.Euler(0,270,0);
			}else{
				//Debug.Log(this.transform.rotation.y - 90f);
				this.transform.Rotate (0, 5f, 0);
			}
		}


	}

	void OnCollisionStay(Collision other)
	{
		//Debug.Log ("collision");
		if (other.gameObject.tag == "Seed") {
			Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), false);
			if(other.gameObject == targetSeed){
				if(currentSeed.getHealth() <= 0){
					Destroy (other.gameObject);
					//lookingForSeed = true;
				}else{
					currentSeed.setHealth(1);
					//Debug.Log (currentSeed.getHealth());
				}
			}
			//GetTargetSeed();
		}
		if (other.gameObject.tag == "Player") {
			Physics.IgnoreCollision(player.GetComponent<Collider>(), GetComponent<Collider>(), false);
		}
	}

	void OnTriggerEnter(Collider other)
	{
		//Debug.Log ("trigger");
		if (other.gameObject.tag == "Bush") {
			if(other.gameObject == targetBush){
				//Debug.Log ("Hit Target Bush");
				if(lookingForBush)
					TeleportToBush();
			}
		}
	}

	void OnTriggerStay(Collider other){
		/*
		if (other.gameObject.tag == "Player") {

			//Debug.Log ("HIT");
		
			if (hitCoolDown <= 0 && !isRestrained) {
				if ((other.GetComponent<PlayerController> ().stunTimer <= 0 || other.GetComponent<PlayerController> ().canControl == true)) {	
					int hitDirection;
					
					if (transform.position.x - other.transform.position.x >= 0) {
						hitDirection = -1;
					} else {
						hitDirection = 1;
					}
					player.GetComponent<PlayerController>().damagePlayer(damageValue, hitDirection);
					//Debug.Log (player.GetComponent<Player> ().GetHealth ());
					//hitCoolDown = 60;
				}
			}
		}
		*/
	}


}
