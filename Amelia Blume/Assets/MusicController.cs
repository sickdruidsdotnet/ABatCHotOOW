using UnityEngine;
using System.Collections;

public class MusicController : MonoBehaviour {

	public AudioClip act1;
	public AudioClip act2;
	public AudioClip act3;
	public AudioClip act4;
	public AudioClip act5;
	public AudioClip cut1;
	public AudioClip cut2;
	public AudioClip cut3;
	public AudioClip cut4;
	public AudioClip cut5;

	AudioSource[] sources;
	// We need two audio sources in order to cross fade
	AudioSource activeSource;
	AudioSource standbySource;
	bool fadeIn;
	bool fadeOut;
	bool crossFade;
	bool cfPause;
	string currentLevel;
	string currentAct;
	string prevLevel = "Act0-Nothing";
	string prevAct = "Act0";

	// Use this for initialization
	void Start () {
		sources = GameObject.FindGameObjectWithTag ("MainCamera").GetComponents<AudioSource> ();
		
		activeSource = sources[0];
		activeSource.loop = true;

		standbySource = sources[1];
		//standbySource.clip = act1;
		standbySource.loop = true;
		//standbySource.Play();
		
		fadeIn = true;
		fadeOut = false;
		crossFade = false;
	}
	
	// Update is called once per framea
	void Update () {
		if(fadeIn){
			fadeInAudio(standbySource);
		}

		if(fadeOut)
			fadeOutAudio(activeSource);

		if(crossFade && !fadeOut && !fadeIn){
			AudioSource tempSource = activeSource;
			activeSource = standbySource;
			standbySource = tempSource;
			crossFade = false;
			if (cfPause)
				standbySource.Pause();
			else
				standbySource.Stop();

		}

	}

	void fadeOutAudio(AudioSource audio) {
    	if(audio.volume > 0.0)
    	{
        	audio.volume -= (float)0.1 * Time.deltaTime;
    	}
    	else 
    	{
    		fadeIn = false;
    		if(!crossFade)
    		{
    			audio.Stop();
    			activeSource = standbySource;
    			standbySource = audio;
    		}
    	}
 	}

	void fadeInAudio(AudioSource audio) {
		if(!audio.isPlaying)
			audio.Play();

    	if(audio.volume < 1.0)
    	{
        	audio.volume += (float)0.1 * Time.deltaTime;	
    	}
    	else 
    	{
    		fadeOut = false;
    		if(!crossFade)
    		{
    			standbySource = activeSource;
    			activeSource = audio;
    		}
    	}
 	}

 	public void crossFadeActiveAndStandby(bool pause = true){
 		crossFade = true;
 		fadeIn = true;
 		fadeOut = true;
 		cfPause = pause;
 	}

 	public void setClip(AudioClip clip, string clipType){
 		if(clipType == "active")
 			activeSource.clip = clip;
 		else
 			standbySource.clip = clip;
 	}

 	public void setClip(string act, string clipType){
 		switch(act)
				{
				case "ActI":
					setClip(act1, clipType);
					break;
				case "ActII":
					setClip(act2, clipType);
					break;
				case "ActIII":
					setClip(act3, clipType);
					break;
				case "ActIV":
					setClip(act4, clipType);
					break;
				case "ActV":
					setClip(act5, clipType);
					break;
				}
 	}

 	public void fadeInStandby(){
 		fadeIn = true;
 	}

 	public void fadeOutActive(){
 		fadeOut = true;
 	}

 	void OnLevelWasLoaded(int level){
 		prevLevel = currentLevel;
 		prevAct = currentAct;
 		currentLevel = Application.loadedLevelName;
 		currentAct = extractAct(currentLevel);
 		Debug.Log("currentAct: " + currentAct);
 		
 		if(currentAct != prevAct)
 			setClip(currentAct, "standby");
 			fadeInStandby();
 	}

 	public string extractAct(string levelName){
 		return levelName.Substring(0, levelName.IndexOf('-'));
 	}

}
