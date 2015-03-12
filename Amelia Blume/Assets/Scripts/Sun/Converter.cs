﻿using UnityEngine;
using System.Collections;

public class Converter : MonoBehaviour {

	public float life = 200;
	float scaleValue;
	private GameObject player;
	bool fading = true;
	bool conversionSuccess = false;
	public GameObject target;
	void Start()
	{
		player = GameObject.Find ("Player");
		scaleValue = transform.localScale.x / 11f;
	}
	// Update is called once per frame
	void Update () {
		HandleInput ();
		life--; 
		if (life < 0) {
			Destroy (this.gameObject);
		}

		if (player.GetComponent<Player> ().isConverting () || conversionSuccess) {
			if (life < 20 && life > 10) {
				transform.localScale = new Vector3 (transform.localScale.x - scaleValue,
			                        transform.localScale.y + 8f,
			                        transform.localScale.z);
			}
			else if(life == 20)
			{
				//successful conversion
				conversionSuccess = true;
				target.SendMessage("changeInfection");
				player.GetComponent<Player>().SetConverting(false);
			}
		} else {
			Renderer[] childrays = transform.GetComponentsInChildren<Renderer>();
			foreach( Renderer child in childrays)
			{
				if(child.material.color.a > 0)
				{
					Color newColor;
					if(child.material.color.a - 0.016f <= 0){
						newColor = new Color(0.95f, 0.95f,0.95f, 0f);
					}
					else{
						newColor = new Color(0.95f, 0.95f,0.95f, child.material.color.a - 0.016f);
					}
					child.material.SetColor ("_Color", newColor);
				}
			}
		}
	}

	protected void HandleInput() {
		if (Input.GetButtonUp ("Sun")) {
			player.GetComponent<Player>().SetConverting(false);
			life = 60;
		}
	}

}
