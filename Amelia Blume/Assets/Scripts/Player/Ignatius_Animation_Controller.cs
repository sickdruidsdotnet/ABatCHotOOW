using UnityEngine;
using System.Collections;

public class Ignatius_Animation_Controller : MonoBehaviour {
	public bool walking;
	public bool talking;
	public bool gesturing;
	
	Animator anim;

	// Use this for initialization
	void Start () {
		anim = this.GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void FixedUpdate () {
	
		if (walking)
			anim.SetBool ("isWalking", true);
		else
			anim.SetBool ("isWalking", false);

		if (talking) {
			anim.SetBool ("isTalking", true);
		} else
			anim.SetBool ("isTalking", false);

		if (gesturing)
			anim.SetBool ("isGesturing", true);
		else
			anim.SetBool ("isGesturing", false);
	}
}
