using UnityEngine;
using System.Collections;
<<<<<<< HEAD
=======
using UnityEngine.UI;
>>>>>>> master

public class fade_in : MonoBehaviour {

	public SpriteRenderer button;
<<<<<<< HEAD

	public bool overwrite = false;
	bool exited = false;
=======
	Text prompt;

	public bool overwrite = false;
	bool exited = false;
	public bool destroyOnFadeOut = false;
	public bool isConversionPrompt;
>>>>>>> master

	public bool showup = false;
	public bool disappear = false;
	float startTime;

	void Start () {
		if (button == null)
			button = gameObject.GetComponentInChildren<SpriteRenderer> ();

		if (button == null) {
			Debug.LogError ("FADE ERROR: this gameobject lacks a child with a sprite renderer");
			Destroy (gameObject);
		}

<<<<<<< HEAD
		button.color = new Color (button.color.r, button.color.g, button.color.b, 0f);
	}
	
	// Update is called once per frame
	void Update () {
=======
		prompt = gameObject.GetComponentInChildren<Text> ();

		button.color = new Color (button.color.r, button.color.g, button.color.b, 0f);
		prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, 0f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
>>>>>>> master
		//Debug.Log (button.color.a);
		if (exited && !overwrite && button.color.a != 0) {
			float alph = button.color.a - 0.05f;
			if (button.color.a <= 0){
				alph = 0;
				exited = false;
			}
			button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
<<<<<<< HEAD
=======
			prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
>>>>>>> master
		}
		else if (overwrite) {
			if(showup && button.color.a != 1)
			{
				float alph = button.color.a + 0.05f;
				if(button.color.a >= 1){
					alph = 1;
					showup = false;
				}
				button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
<<<<<<< HEAD
=======
				prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
>>>>>>> master
			}
			else if (disappear && button.color.a != 0){
				float alph = button.color.a - 0.05f;
				if (button.color.a <= 0){
					alph = 0;
					disappear = false;
				}
				button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
<<<<<<< HEAD
=======
				prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
				if(!disappear && destroyOnFadeOut)
				{
					Destroy(gameObject);
				}
>>>>>>> master
			}

		}

	}

<<<<<<< HEAD
	void FadeIn(){
		showup = true;
		disappear = false;
	}

	void FadeOut(){
		showup = false;
		disappear = true;
	}

	void OnTriggerStay(Collider other){
		if (other.gameObject.tag == "Player" && button.color.a != 1) {
			float alph = button.color.a + 0.05f;
			if(button.color.a >= 1)
				alph = 1;
			button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
=======
	public void FadeIn(){
		if (!showup) {
			showup = true;
			disappear = false;
		}
	}

	public void FadeOut(){
		if (!disappear) {
			showup = false;
			disappear = true;
		}
	}

	void OnTriggerStay(Collider other){
		if (!isConversionPrompt) {
			if (other.gameObject.tag == "Player" && button.color.a != 1 && !overwrite) {
				float alph = button.color.a + 0.05f;
				if (button.color.a >= 1)
					alph = 1;
				button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
				prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
			}
		} else if(other.gameObject.tag == "Player"){
			FadeOut();
>>>>>>> master
		}
	}

	void OnTriggerExit(Collider other){
<<<<<<< HEAD
		if (other.gameObject.tag == "Player")
			exited = true;
=======
		if (other.gameObject.tag == "Player") {
			exited = true;
			if(isConversionPrompt) //not quuuiiiiiite functional
				FadeIn();
		}
>>>>>>> master
	}
}
