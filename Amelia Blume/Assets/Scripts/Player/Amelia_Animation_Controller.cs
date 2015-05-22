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
	public bool planting;
	public bool sunLighting;
	public float watering;

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
		planting = amelia.isPlanting;
		sunLighting = amelia.isSunLighting;
		watering = amelia.watering;

		if(running && !jumping)
			anim.SetBool ("isRunning", true);
		else
			anim.SetBool ("isRunning", false);

		if (jumping) {
			if (!dashing && !airDashing)
				anim.SetBool ("isJumping", true);
		}else
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

		if (planting){
			anim.SetBool ("isPlanting", true);
			amelia.isPlanting = false;
		}
		else
			anim.SetBool ("isPlanting", false);

		if (sunLighting)
			anim.SetBool ("isSunLighting", true);
		else
			anim.SetBool ("isSunLighting", false);
		
		anim.SetFloat ("watering", watering);



	
	}
}
