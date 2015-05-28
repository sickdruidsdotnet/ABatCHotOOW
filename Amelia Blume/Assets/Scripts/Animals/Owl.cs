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
		Vector3 targetPos = new Vector3 (player.transform.position.x, player.transform.position.y - 0.5f, 0);
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
				if ((other.GetComponent<PlayerController> ().isStunned == false && other.GetComponent<PlayerController> ().invulnerable == false)) {	
					int hitDirection;
			
					if (transform.position.x - other.transform.position.x >= 0) {
						hitDirection = -1;
					} else {
						hitDirection = 1;
					}
					player.GetComponent<PlayerController>().damagePlayer(damageValue, hitDirection);
					hitCoolDown = 60;
				}
			}
		}
	}

	void OnTriggerEnter(Collider other){
		if (other.gameObject.tag == "Player") {
			if (isRestrained && isInfected) {
				Debug.Log ("Can convert");
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().canConvert = true;
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().conversionTarget = gameObject;
			} else {
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().canConvert = false;
				GameObject.FindGameObjectWithTag ("Player").GetComponent<PlayerController> ().conversionTarget = null;
			}
		}
	}
	




}
