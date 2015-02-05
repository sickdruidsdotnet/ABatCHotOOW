using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class Plant : MonoBehaviour
{
    protected int waterCount;
    protected int hydrationGoal;
    protected float maturity;
    
    // Constructor
    public Plant()
    {
        // the amount of water this seed has collected
        waterCount = 0;
        // waterCount threshold. once reached, sproutPlant() will be called
        hydrationGoal = 100;
        // True if the seed is inside of soil object
        maturity = 0.0f;

        Debug.Log("Plant created");
        Debug.Log("Default hydrationGoal: " + hydrationGoal);
    }

    // Grows the plant.
    public void grow()
    {
        // do something with procedural growth.

        maturity = (float)(waterCount / hydrationGoal);
    }

    public void collectWater()
    {
        // increment waterCount
        waterCount++;

        // check to see if we've collected enough water
        if (waterCount >= hydrationGoal)
        {
            grow();
        }
    }

    public int getWaterCount()
    {
        return waterCount;
    }

    public int getHydrationGoal()
    {
        return hydrationGoal;
    }

    public float getMaturity()
    {
        return maturity;
    }
}