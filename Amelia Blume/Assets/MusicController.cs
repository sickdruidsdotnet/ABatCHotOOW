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
	AudioSource[] allSources;
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

	float max_vol = 0.7F;

	// Use this for initialization
	void Start () {
		//Debug.Log("MusicController Start called");
		sources = this.GetComponents<AudioSource> ();
		allSources = GameObject.FindSceneObjectsOfType(typeof(AudioSource)) as AudioSource[];
		
		activeSource = sources[0];
		activeSource.loop = true;
		activeSource.volume = 0.0F;

		standbySource = sources[1];
		standbySource.loop = true;
		standbySource.volume = 0.0F;

		currentLevel = Application.loadedLevelName;
 		currentAct = extractAct(currentLevel);
 		//Debug.Log("currentAct == " + currentAct);
 		setClip(currentAct, "standby");

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

		  // When a key is pressed list all the gameobjects that are playing an audio
        if(Input.GetKeyUp(KeyCode.L))
        { 
        	foreach(AudioSource audioSource in allSources)
            {
                if(audioSource.isPlaying) Debug.Log(audioSource.name+" is playing "+audioSource.clip.name);
            }
            Debug.Log("---------------------------"); //to avoid confusion next time
            Debug.Break(); //pause the editor     
        }

	}

	void fadeOutAudio(AudioSource audio) {
    	if(audio.volume > 0.0)
    	{
        	audio.volume -= (float)0.5 * Time.deltaTime;
    	}
    	else 
    	{
    		fadeOut = false;
    		if(!crossFade)
    		{
    			audio.Stop();
    			//activeSource = standbySource;
    			//standbySource = audio;
    		}
    	}
 	}

	void fadeInAudio(AudioSource audio) {
		if(!audio.isPlaying)
			audio.Play();

    	if(audio.volume < max_vol)
    	{
        	audio.volume += (float)0.1 * Time.deltaTime;	
    	}
    	else 
    	{
    		fadeIn = false;
    		if(!crossFade)
    		{
    			standbySource = activeSource;
    			activeSource = audio;

    			//standbySource.volume = 0.0;
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
				default:
					setClip(act1, clipType);
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
 		//Debug.Log("MusicController OnLevelWasLoaded called");
 		prevLevel = currentLevel;
 		prevAct = currentAct;
 		currentLevel = Application.loadedLevelName;
 		currentAct = extractAct(currentLevel);

 		if(currentAct != prevAct) {
 		//	Debug.Log("Act change. " + prevAct + " to " + currentAct);
 			if(!fadeOut)
 				fadeOutActive();

 			setClip(currentAct, "standby");
 			fadeInStandby();
 		}
 		else{
 		//	Debug.Log("No act change. " + prevAct + " to " + currentAct);
 		}
 	}

 	public string extractAct(string levelName){

 		string actName;
 		try{
 			actName = levelName.Substring(0, levelName.IndexOf('-'));
 		}
 		catch{
 			actName = levelName;
 		}

 		return actName;
 	}

}
