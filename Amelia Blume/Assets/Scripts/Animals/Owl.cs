using UnityEngine;
using System.Collections;

public class Owl : Animal {
	private GameObject playerObject;
	private Player player;
	private Vector3 to;
	private Vector3 from;
	private float angle;
	private float fieldOfView = 13;
	private float speed;
	public Transform perch;
	private float distance;
	public int damageValue;
	int hitCoolDown = 60;
	// Use this for initialization
	void Start () {
		playerObject = GameObject.FindGameObjectWithTag ("Player");
		player = playerObject.GetComponent <Player>();
		//to = -this.transform.up;
		speed = 0.1f;
		damageValue = 5;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		hitCoolDown--;
		//from = player.transform.position - this.transform.position;
		//angle = (Vector3.Angle(to, from));
		distance = (Vector3.Distance(player.transform.position, this.transform.position));
		//Debug.DrawLine(to, from, Color.red,100f,false);
		if (!isRestrained) {
			if (distance < fieldOfView)
				ChasePlayer ();
			else 
				ReturnToPerch ();
		}
	}

	void ChasePlayer()
	{
		//Debug.Log ("In pursuit");
		Vector3 targetPos = new Vector3 (player.transform.position.x, player.transform.position.y - 0.5f, player.transform.position.z);
		transform.position = Vector3.MoveTowards (transform.position, targetPos, speed);
	}

	void ReturnToPerch()
	{
		//Debug.Log ("Back to Perch");
		transform.position = Vector3.MoveTowards (transform.position, perch.position, speed);
	}

	void OnTriggerStay(Collider other){
		if (other.gameObject.tag == "Player") {
//			Debug.Log ("HIT");
			if (hitCoolDown <= 0 && !isRestrained) {
				if ((other.GetComponent<PlayerController> ().stunTimer <= 0 || other.GetComponent<PlayerController> ().canControl == true)) {	
					int hitDirection;
			
					if (transform.position.x - other.transform.position.x >= 0) {
						hitDirection = -1;
					} else {
						hitDirection = 1;
					}
					player.GetComponent<Player> ().ReduceHealth (damageValue);
					//other.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * 4, 8f, 0f), 100f);
					if (!(player.GetComponent<Player> ().GetHealth () - damageValue <= 0)) {
						player.GetComponent<ImpactReceiver> ().AddImpact (new Vector3 (hitDirection * 8, 8f, 0f), 100f);
					}
					other.GetComponent<PlayerController> ().canControl = false;
					other.GetComponent<PlayerController> ().stunTimer = 30;
					//Debug.Log (player.GetComponent<Player> ().GetHealth ());
					hitCoolDown = 60;
				}
			}
		}
	}




}
