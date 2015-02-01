using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class Seed : MonoBehaviour
{
    protected int waterCount;
    protected int hydrationGoal;
    protected bool isPlanted;

    public int testInt;
    
    // Constructor
    public Seed()
    {
        // the amount of water this seed has collected
        waterCount = 0;
        // waterCount threshold. once reached, sproutPlant() will be called
        hydrationGoal = 0;
        // True if the seed is inside of soil object
        isPlanted = false;

        testInt = 69;

        Debug.Log("Seed created");
        Debug.Log("Default hydrationGoal: " + hydrationGoal);
    }

    // Destroys this Seed object, and creates new Plant object.
    public void sproutPlant()
    {
        // Create new Plant entity, specificcal the type stored in PlantType.

        // Destroy this Seed entity. The plant will carry on its legacy.
        // Goodnight, sweet seed. You served Amelia well.
        
        // Object.Destroy(this.gameObject);
    }

    public void collectWater()
    {
        // increment waterCount
        waterCount++;

        // check to see if we've collected enough water
        if (waterCount >= hydrationGoal)
        {
            sproutPlant();
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

    public bool checkIfPlanted()
    {
        return isPlanted;
    }
}