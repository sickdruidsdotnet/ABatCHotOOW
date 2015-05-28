using UnityEngine;
using System.Collections;

public class WaterParticles : MonoBehaviour {
	public InputHandler playerInput;

	bool rightTrigger;
	int coolDown;
	public ParticleSystem water;
	GameObject amelia;
	Player player;
	// Use this for initialization
	void Start () {
		amelia = GameObject.Find ("Player");
		player = amelia.GetComponent<Player> ();
		GameObject playerInputObj = GameObject.FindGameObjectWithTag ("Input Handler");
		if (playerInputObj != null) {
			playerInput = playerInputObj.GetComponent<InputHandler> ();
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (playerInput == null) {
			playerInput = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();	
		}
		rightTrigger = playerInput.water;
		if (rightTrigger && coolDown > 10 && !player.CUTSCENE)
		{
			StartWater ();
			coolDown = 0;
		}

		coolDown++;
	}

	void StartWater()
	{
		water.Emit (1);
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Soil") {
			//Debug.Log ("Soil collision");
			//Add to hydration level of soil later on
		}
	}
}