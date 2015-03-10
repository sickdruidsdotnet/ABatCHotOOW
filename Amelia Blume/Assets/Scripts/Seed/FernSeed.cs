using UnityEngine;
using System.Collections;

public class FernSeed : Seed {
	
	// Constructor
	public FernSeed()
	{
		
		// set hydrationGoal to match VineSeed requirements
		this.hydrationGoal = 20;
		
		this.plantType = "FernPlant";
		
		Debug.Log("FernSeed created");
		Debug.Log("FernSeed hydrationGoal: " + hydrationGoal);
	}
}
