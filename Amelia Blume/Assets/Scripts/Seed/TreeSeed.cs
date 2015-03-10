using UnityEngine;
using System.Collections;

public class TreeSeed : Seed{

	// Constructor
	public TreeSeed()
	{
		
		// set hydrationGoal to match VineSeed requirements
		this.hydrationGoal = 20;
		
		this.plantType = "TreePlant";
		
		Debug.Log("TreeSeed created");
		Debug.Log("TreeSeed hydrationGoal: " + hydrationGoal);
	}
}
