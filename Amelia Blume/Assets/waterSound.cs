using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class waterSound : MonoBehaviour {

	public AudioClip waterSound1;
	public AudioClip waterSound2;
	private AudioSource source;
	public float volLowRange = 0.75F;
	public float volHighRange = 1.0F;
	public float lowPitchRange = 0.85F;
    public float highPitchRange = 1.25F;
	private List<AudioClip> soundList;

	// Use this for initialization
	void Start () {
		source = this.GetComponent<AudioSource> ();
		soundList = new List<AudioClip>{waterSound1, waterSound2};
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Water") {
			float vol = Random.Range (volLowRange, volHighRange);
			source.pitch = Random.Range(lowPitchRange, highPitchRange);
			source.PlayOneShot(soundList[Random.Range(0, soundList.Count)], vol);
    		//source.PlayOneShot(waterSound2, vol);
    	}
    }	
}
