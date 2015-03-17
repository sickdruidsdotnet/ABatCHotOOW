using UnityEngine;
using System.Collections;

public class Amelia_Animation_Controller : MonoBehaviour {
	PlayerController amelia;
	public bool running;
	public bool jumping;
	public bool isTakingDamage;
	public bool dashing;
	public bool airDashing;
	public bool stunned;

	Animator anim;

	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindWithTag ("Player");
		amelia = playerObject.GetComponent<PlayerController>();
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
		running = amelia.isRunning;
		jumping = amelia.isJumping;
		dashing = amelia.isDashing;
		airDashing = amelia.isAirDashing;
		stunned = amelia.isStunned;

		if(running)
			anim.SetBool ("isRunning", true);
		else
			anim.SetBool ("isRunning", false);

		if(jumping)
			anim.SetBool ("isJumping", true);
		else
			anim.SetBool ("isJumping", false);

		if(dashing)
			anim.SetBool ("isDashing", true);
		else
			anim.SetBool ("isDashing", false);
		
		if(airDashing)
			anim.SetBool ("isAirDashing", true);
		else
			anim.SetBool ("isAirDashing", false);

		if(stunned)
			anim.SetBool ("isStunned", true);
		else
			anim.SetBool ("isStunned", false);
	
	}
}
