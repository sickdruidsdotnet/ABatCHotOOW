using UnityEngine;
using System.Collections;

public class TreeSeed : Seed{

	// Constructor
	public TreeSeed()
	{
		
		// set hydrationGoal to match VineSeed requirements
		this.hydrationGoal = 20;
		
		plantType = "TreePlant/TreePlant";
		
		//Debug.Log("TreeSeed created");
		//Debug.Log("TreeSeed plant type is " + plantType);
		//Debug.Log("TreeSeed hydrationGoal: " + hydrationGoal);
	}
}
