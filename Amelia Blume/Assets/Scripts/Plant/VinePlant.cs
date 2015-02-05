using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class VinePlant : Plant 
{
    
    // Constructor
    public VinePlant()
    {
        
        // set hydrationGoal to match VineSeed requirements
        this.hydrationGoal = 200;

        Debug.Log("VinePlant created");
        Debug.Log("VinePlant hydrationGoal: " + hydrationGoal);
    }

    // restrain enemy
    public void restrain()
    {
    	// make enemy a child of this GameObject
    	// switch to 
    }
}