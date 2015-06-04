using UnityEngine;
using System.Collections;

public class Amelia_Animation_Controller : MonoBehaviour {
	PlayerController amelia;
	Player player;
	public bool running;
	public bool jumping;
	public bool isTakingDamage;
	public bool dashing;
	public bool airDashing;
	public bool stunned;
	public bool planting;
	public bool sunLighting;
	public bool watering;
	public float fallSpeed;
	
	Animator anim;

	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindWithTag ("Player");
		player = playerObject.GetComponent<Player> ();
		amelia = playerObject.GetComponent<PlayerController>();
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (!player.CUTSCENE) {
			running = amelia.isRunning;
			jumping = amelia.isJumping;
			dashing = amelia.isDashing;
			airDashing = amelia.isAirDashing;
			stunned = amelia.isStunned;
			planting = amelia.isPlanting;
			sunLighting = amelia.isSunLighting;
			watering = amelia.watering;
			fallSpeed = amelia.player.motor.movement.fallSpeed;


			if (running && !jumping)
				anim.SetBool ("isRunning", true);
			else
				anim.SetBool ("isRunning", false);

			if (jumping) {
				if (!dashing && !airDashing)
					anim.SetBool ("isJumping", true);
			} else
				anim.SetBool ("isJumping", false);

			if (dashing)
				anim.SetBool ("isDashing", true);
			else
				anim.SetBool ("isDashing", false);
		
			if (airDashing)
				anim.SetBool ("isAirDashing", true);
			else
				anim.SetBool ("isAirDashing", false);

			if (stunned)
				anim.SetBool ("isStunned", true);
			else
				anim.SetBool ("isStunned", false);

			if (planting) {
				anim.SetBool ("isPlanting", true);
				amelia.isPlanting = false;
			} else
				anim.SetBool ("isPlanting", false);

			if (sunLighting)
				anim.SetBool ("isSunLighting", true);
			else
				anim.SetBool ("isSunLighting", false);

			if (fallSpeed < 0)
				anim.SetBool("isFalling", true);
			else
				anim.SetBool("isFalling", false);

			if (watering)
				anim.SetBool("isWatering", true);
			else
				anim.SetBool("isWatering", false);

		}
		
		//anim.SetFloat ("watering", watering);



	
	}
}
