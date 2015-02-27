using UnityEngine;
using System.Collections;

public class WaterParticles : MonoBehaviour {
	bool rightTrigger;
	int coolDown;
	public ParticleSystem water;
	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
		rightTrigger = Input.GetButton ("Water");
		if (rightTrigger && coolDown > 10)
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