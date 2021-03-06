﻿using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class fade_in : MonoBehaviour {

	public SpriteRenderer button;
	Text prompt;

	public bool overwrite = false;
	bool exited = false;
	public bool destroyOnFadeOut = false;
	public bool isConversionPrompt;

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

		prompt = gameObject.GetComponentInChildren<Text> ();

		button.color = new Color (button.color.r, button.color.g, button.color.b, 0f);
		prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, 0f);
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log (button.color.a);
		if (exited && !overwrite && button.color.a != 0) {
			float alph = button.color.a - 0.05f;
			if (button.color.a <= 0){
				alph = 0;
				exited = false;
			}
			button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
			prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
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
				prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
			}
			else if (disappear && button.color.a != 0){
				float alph = button.color.a - 0.05f;
				if (button.color.a <= 0){
					alph = 0;
					disappear = false;
				}
				button.color = new Color (button.color.r, button.color.g, button.color.b, alph);
				prompt.color = new Color (prompt.color.r, prompt.color.g, prompt.color.b, alph);
				if(!disappear && destroyOnFadeOut)
				{
					Destroy(gameObject);
				}
			}

		}

	}

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
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Player") {
			exited = true;
			if(isConversionPrompt) //not quuuiiiiiite functional
				FadeIn();
		}
	}
}
