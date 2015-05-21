using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioClip act1;
	public AudioClip act2;
	public AudioClip act3;
	public AudioClip act4;
	public AudioClip act5;
	AudioSource[] sources;
	// We need two audio sources in order to cross fade
	AudioSource active;
	AudioSource standby;
	bool fadeIn;
	bool fadeOut;
	bool crossFade;
	int currentLevel;

	// Use this for initialization
	void Start () {
		sources = GameObject.FindGameObjectWithTag ("MainCamera").GetComponents<AudioSource> ();
		active = sources[0];
		standby = sources[1];
		active.clip = act1;
		active.loop = true;
		active.Play();
	}
	
	// Update is called once per framea
	void Update () {
	
	}

	void fadeOutAudio(float vol, AudioSource audio) {
    	if(audio.vol > 0.1)
    	{
        	audio.vol -= 0.1 * Time.deltaTime;
        	//audio.volume = vol;
    	}
 	}

	void fadeInAudio(float vol, AudioSource audio) {
    	if(audio.vol < 1.0)
    	{
        	audio.vol += 0.1 * Time.deltaTime;
        	//audio.volume = vol;
    	}	
 	}
 
}
