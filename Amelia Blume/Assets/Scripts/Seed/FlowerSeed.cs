using UnityEngine;
using System.Collections;

public class FlowerSeed : Seed {

	// Constructor
	public FlowerSeed()
	{
		
		// set hydrationGoal to match VineSeed requirements
		this.hydrationGoal = 20;
		
		this.plantType = "FlowerPlant/Flower";
		
		//Debug.Log("FlowerSeed created");
		//Debug.Log("FlowerSeed hydrationGoal: " + hydrationGoal);
	}
}
