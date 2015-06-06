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
	
	}
}
