using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class Plant : MonoBehaviour
{
    protected int waterCount;
    protected int hydrationGoal;
    protected float maturity;
    protected int collectionTimer;
    protected int collectionDelay;
    protected Vector3 baseScale;
    protected float scaleFactor;
    
    // Constructor
    public Plant()
    {
        // the amount of water this seed has collected
        waterCount = 0;
        // waterCount threshold. once reached, sproutPlant() will be called
        hydrationGoal = 100;
        // True if the seed is inside of soil object
        maturity = 0.0f;

        // initialize collection timer and delay
        collectionTimer = 0;
        collectionDelay = 30;

        Debug.Log("Plant created");
        Debug.Log("Default hydrationGoal: " + hydrationGoal);
        Debug.Log("baseScale: " + baseScale);
    }

    void Start()
    {
        // this line can't be in the constructor, for some reason.
        baseScale = transform.localScale;
    }

    // Grows the plant.
    public void grow()
    {
        // do something with procedural growth.

        maturity = (float)(waterCount / hydrationGoal);
        maturity = (float)waterCount / (float)hydrationGoal;
        scaleFactor = 2.0f * maturity;
        transform.localScale = new Vector3(baseScale.x + scaleFactor, baseScale.y + scaleFactor, baseScale.z + scaleFactor);
    }

    public void collectWater()
    {
        // increment waterCount
        waterCount++;

        Debug.Log("Plant collected water. Plant waterCount: " + waterCount + ", Soil Hydration level: " + soil.GetHydrationLevel());
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