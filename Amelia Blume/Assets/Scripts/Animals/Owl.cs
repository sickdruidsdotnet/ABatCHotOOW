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
	// Use this for initialization
	void Start () {
		playerObject = GameObject.FindGameObjectWithTag ("Player");
		player = playerObject.GetComponent <Player>();
		//to = -this.transform.up;
		speed = 0.05f;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//from = player.transform.position - this.transform.position;
		//angle = (Vector3.Angle(to, from));
		distance = (Vector3.Distance(player.transform.position, this.transform.position));
		//Debug.DrawLine(to, from, Color.red,100f,false);
		if (distance < fieldOfView)
			ChasePlayer ();
		else 
			ReturnToPerch ();
	}

	void ChasePlayer()
	{
		//Debug.Log ("In pursuit");
		transform.position = Vector3.MoveTowards (transform.position, player.transform.position, speed);
	}

	void ReturnToPerch()
	{
		//Debug.Log ("Back to Perch");
		transform.position = Vector3.MoveTowards (transform.position, perch.position, speed);
	}

}
