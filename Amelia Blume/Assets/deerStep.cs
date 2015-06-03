using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class deerStep : MonoBehaviour {

	public AudioClip step1;
	public AudioClip step2;
	public AudioClip step3;
	public AudioClip step4;
	public float volLowRange = 0.5F;
	public float volHighRange = 1.0F;
	public float lowPitchRange = .75F;
    public float highPitchRange = 1.5F;
	public float stepTimer = 0.0F;
	public float stepCoolDown = 0.6F;
	public float prevY;
	public float currY;
	public float parentY;
	public bool goingUp = true;
	private AudioSource source;
	private List<AudioClip> stepList;
	//public GameObject Grandestparent;


	// Use this for initialization
	void Start () {
		source = GetComponent<AudioSource>();
		stepList = new List<AudioClip>{step1, step2, step3, step4};
		prevY = transform.position.y;
	
	}
	
	// Update is called once per frame
	void Update () {
		currY = transform.position.y;
		if (currY > prevY && !goingUp)
		{
			//Debug.Log("Going up");
			goingUp = true;
			//stepSound();
		}

		if(currY < prevY && goingUp)
		{
			//Debug.Log("Going down");
			goingUp = false;
		}

		//prevY = currY;
		//parentY = Grandestparent.transform.position.y;
		//float diff = parentY - currY;

		//Debug.Log("Current diff: " + diff);

		//if (diff < -0.333F)
			//stepSound();

		// Not charging and Not Charging up => walking, so longer cooldown
		//if (!this.isCharging && !this.isInChargeUp) 
		//{
		//	stepCoolDown = 0.6F;
		//	stepSound();
		
		//}
		
		//if (this.isCharging) 
		//{
			// If deer is charging, shorter cool down
			//stepCoolDown = 0.4F;
			//stepSound();
		//}
		// If deer is charging up, don't play any sound
		
		//if(stepTimer > 0)
		//	stepTimer -= Time.deltaTime;
		//else
		//	stepTimer = 0;

		//stepSound();
	}


 	void OnTriggerEnter(Collider other) {
 		Debug.Log("other tag == " + other.tag);
 		//Debug.Log(this.tag + " Enter");
    	if(other.tag=="Ground" || other.tag=="Soil" || other.tag=="Grass") {

    		source.pitch = Random.Range(lowPitchRange, highPitchRange);
    		float vol = Random.Range (volLowRange, volHighRange);

    		Debug.Log("play from trigger");
    		source.PlayOneShot(stepList[Random.Range(0, stepList.Count)], vol);

    		source.PlayOneShot(step1, vol);

    	}
 	}

 	void OnTriggerStay(Collider other) {
 		Debug.Log(this.tag + "Stay");
 		//    source.pitch = Random.Range(lowPitchRange, highPitchRange);
    	//	float vol = Random.Range (volLowRange, volHighRange);

    //		source.PlayOneShot(stepList[Random.Range(0, stepList.Count)], vol);

    		//source.PlayOneShot(step1, vol);
 	//}

 	//void OnTriggerExit(Collider other) {
 		//Debug.Log(this.tag + "Exit");
 	}


 	void stepSound() {
		//if(stepTimer == 0)
		//{
			source.pitch = Random.Range(lowPitchRange, highPitchRange);
    		float vol = Random.Range (volLowRange, volHighRange);

    		Debug.Log("play from StepSound");
    		source.PlayOneShot(stepList[Random.Range(0, stepList.Count)], vol);
			stepTimer = stepCoolDown;
		//}
	}

}