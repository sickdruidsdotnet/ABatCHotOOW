using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class VineSeed : Seed 
{
    
    // Constructor
    public VineSeed()
    {
        
        // set hydrationGoal to match VineSeed requirements
        this.hydrationGoal = 20;

        this.plantType = "VinePlant/VinePlant_Procedural";

        //Debug.Log("VineSeed created");
        //Debug.Log("VineSeed hydrationGoal: " + hydrationGoal);
    }
}