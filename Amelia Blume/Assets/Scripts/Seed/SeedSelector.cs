using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class SeedSelector : MonoBehaviour {

	InputHandler playerInput;

	public GameObject stickImage;
	GameObject player;
	Player playerStats;
	Renderer[] childRenderers;
	public Renderer[] sections;
	public SelectionEffect[] seeds;
	string prevDirection;
	string direction;
	float horizontal2;
	float vertical2;

	bool anyUnlocked;

	void Start(){
		player = GameObject.FindGameObjectWithTag ("Player");
		playerInput = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();
		playerStats = player.GetComponent<Player> ();
		transform.position = new Vector3 (player.transform.position.x,
		                                 player.transform.position.y + 3f,
		                                 transform.position.z);
		childRenderers = GetComponentsInChildren<Renderer> ();
		foreach(Renderer renderer in childRenderers)
			renderer.enabled = false;
		direction = "Up";
		prevDirection = "Up";

		anyUnlocked = playerStats.vineUnlocked || playerStats.treeUnlocked || playerStats.fluerUnlocked;
	}

	// Update is called once per frame
	void FixedUpdate () {
		anyUnlocked = playerStats.vineUnlocked || playerStats.treeUnlocked || playerStats.fluerUnlocked;

		transform.position = new Vector3 (player.transform.position.x,
		                                  player.transform.position.y + 3f,
		                                  transform.position.z);
		//rerender everything real quick
		foreach(Renderer renderer in childRenderers)
			renderer.enabled = true;
		//unrender seeds that aren't unlocked
		if(!playerStats.vineUnlocked){
			seeds[0].transform.renderer.enabled = false;
		}
		if(!playerStats.treeUnlocked){
			seeds[1].transform.renderer.enabled = false;
		}
		if(!playerStats.fluerUnlocked){
			seeds[2].transform.renderer.enabled = false;
		}

		if(playerInput.primaryInput != "KeyBoard"){
			if(playerInput.primaryInput == "©Microsoft Corporation Xbox 360 Wired Controller"){
				horizontal2 = Input.GetAxis("Horizontal 4");
				vertical2 = Input.GetAxis ("Vertical 4");
			}else if(playerInput.primaryInput == "Sony Computer Entertainment Wireless Controller"){
				horizontal2 = Input.GetAxis("Horizontal 3");
				vertical2 = Input.GetAxis ("Vertical 3");
			}else{
				horizontal2 = Input.GetAxis("Horizontal 3");
				vertical2 = Input.GetAxis ("Vertical 3");
			}
			if (new Vector2 (horizontal2, vertical2).magnitude >= 0.3f) {

				float angle = Vector2.Angle(Vector2.up * -1f, new Vector2(horizontal2, vertical2));
				//Debug.Log("H: " + horizontal2 + " V: " + vertical2 + " A: " + angle);
				if( angle >= 60 && angle <= 180 && horizontal2 >= 0){
					sections[0].enabled = false;
					sections[2].enabled = false;
					if(!playerStats.treeUnlocked)
					{
						sections[1].enabled = false;
					}
					direction = "Right";
				}
				else if( angle >= 60 && angle < 180 && horizontal2 < 0 ){
					sections[0].enabled = false;
					sections[1].enabled = false;
					if(!playerStats.fluerUnlocked)
					{
						sections[2].enabled = false;
					}
					direction = "Left";
				}
				else{
					sections[1].enabled = false;
					sections[2].enabled = false;
					if(!playerStats.vineUnlocked)
					{
						sections[0].enabled = false;
					}
					direction = "Up";
				}
				stickImage.transform.position = new Vector3 (transform.position.x + (horizontal2 / 3.5f),
				                                  			 transform.position.y - (vertical2 / 3.5f),
				                                             stickImage.transform.position.z);
			}
			else{
				//Keyboard Seed Handling
				if(player.GetComponent<Player>().getCurrentSeedType() == Player.SeedType.VineSeed && playerInput.firstSeed)
				{
					sections[1].enabled = false;
					sections[2].enabled = false;
					if(!playerStats.vineUnlocked)
					{
						sections[0].enabled = false;
					}
					direction = "Up";
					stickImage.transform.position = new Vector3 (transform.position.x + 0.05f,
					                                             transform.position.y + 0.25f,
					                                             stickImage.transform.position.z);
				}
				else if(player.GetComponent<Player>().getCurrentSeedType() == Player.SeedType.TreeSeed && playerInput.secondSeed)
				{
					sections[0].enabled = false;
					sections[2].enabled = false;
					if(!playerStats.treeUnlocked)
					{
						sections[1].enabled = false;
					}
					direction = "Right";
					stickImage.transform.position = new Vector3 (transform.position.x + 0.33f,
					                                             transform.position.y - 0.17f,
					                                             stickImage.transform.position.z);
				}
				else if(player.GetComponent<Player>().getCurrentSeedType() == Player.SeedType.FlowerSeed && playerInput.thirdSeed)
				{
					sections[0].enabled = false;
					sections[1].enabled = false;
					if(!playerStats.fluerUnlocked)
					{
						sections[2].enabled = false;
					}
					direction = "Left";
					stickImage.transform.position = new Vector3 (transform.position.x - 0.23f,
					                                             transform.position.y - 0.17f,
					                                             stickImage.transform.position.z);
				}
				else
				{
					foreach(Renderer renderer in childRenderers)
						renderer.enabled = false;
				}
			}
			//let's get some cool selection effects in here
			if(prevDirection != direction){
			switch (direction){
				case "Up":
					seeds[0].StartEffect();
					break;
				case "Right":
					seeds[1].StartEffect();
					break;
				case "Left":
					seeds[2].StartEffect();
					break;
				}

				switch (prevDirection){
				case "Up":
					seeds[0].EndEffect();
					break;
				case "Right":
					seeds[1].EndEffect();
					break;
				case "Left":
					seeds[2].EndEffect();
					break;
				}
			}
			prevDirection = direction;

		} else {
			foreach(Renderer renderer in childRenderers)
				renderer.enabled = false;
		}

		if (!anyUnlocked) {
			//unrender ball
			stickImage.renderer.enabled = false;
		}
	}

	public void UpdatePlayer()
	{
		player = GameObject.FindGameObjectWithTag ("Player");
		playerStats = player.GetComponent<Player> ();
	}
}
