using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class Seed : MonoBehaviour
{
    protected int waterCount;
    protected int hydrationGoal;
    protected bool isPlanted;
    public string plantType;
    public Soil soil;
    private int collectionTimer;
    private int collectionDelay;

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
        // Default plant type
        plantType = "Tree";
        // initialize collection timer and delay
        collectionTimer = 0;
        collectionDelay = 30;

        Debug.Log("Seed created");
        Debug.Log("Default hydrationGoal: " + hydrationGoal);
    }

    void Update()
    {
        if(isPlanted)
        {
            if(soil != null)
            {
                if(soil.GetHydrationLevel() > 0)
                {
                    if (collectionTimer > collectionDelay)
                    {
                        collectWater();
                        collectionTimer = 0;
                    }
                }
            }
            else
            {
                Debug.Log("planted, but no soil!");
            }
        }
        collectionTimer++;
    }

    // Destroys this Seed object, and creates new Plant object.
    public void sproutPlant()
    {
        // Create new Plant entity, specificcal the type stored in PlantType.

        // spawn a seed


        Vector3 loc = new Vector3(0, 0, 0);
        loc += transform.position;
        GameObject newPlant = Instantiate(Resources.Load(plantType), loc, Quaternion.identity) as GameObject;
        newPlant.gameObject.GetComponent<Plant>().setSoil(soil.gameObject);
        Debug.Log("called sproutPlant()");

        // Destroy this Seed entity. The plant will carry on its legacy.
        // Goodnight, sweet seed. You served Amelia well.
        
        Object.Destroy(this.gameObject);
    }

    public void collectWater()
    {
        // increment waterCount
        waterCount++;
        soil.ChangeHydrationLevel(-1);

        Debug.Log("Seed collected water. Seed waterCount: " + waterCount + ", Soil Hydration level: " + soil.GetHydrationLevel());
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

    public void setPlanted(bool b)
    {
        isPlanted = b;
    }

    public void setPlantType(string p)
    {
        plantType = p;
    }
    public string getPlantType()
    {
        return plantType;
    }

    public void setSoil(GameObject s)
    {
        soil = s.gameObject.GetComponent<Soil>();
    }
    public Soil getSoil()
    {
        return soil;
    }
}