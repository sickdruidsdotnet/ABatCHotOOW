using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class Plant : MonoBehaviour
{
    protected int waterCount;
    protected int hydrationGoal;
    protected float maturity;
    public Soil soil;
    private int collectionTimer;
    private int collectionDelay;
    private Vector3 baseScale;
    private float scaleFactor;
    
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
    }

    void Start()
    {
        // this can't be in the constructor or Unity complains
        baseScale = transform.localScale;
    }

    void Update()
    {

        if(soil != null)
        {
            if(soil.GetHydrationLevel() > 0)
            {
                if (collectionTimer > collectionDelay)
                {
                    collectWater();
                    grow();
                    collectionTimer = 0;
                }
            }
        }
        else
        {
            Debug.Log("Plant has no soil!");
        }
        collectionTimer++;
    }

    // Grows the plant.
    public void grow()
    {
        // do something with procedural growth.

        maturity = (float)waterCount / (float)hydrationGoal;
        scaleFactor = 2.0f * maturity;
        transform.localScale = new Vector3(baseScale.x + scaleFactor, baseScale.y + scaleFactor, baseScale.z + scaleFactor);
    }

    public void collectWater()
    {
        // increment waterCount
        waterCount++;
        soil.ChangeHydrationLevel(-1);

        Debug.Log("Seed collected water. Seed waterCount: " + waterCount + ", Soil Hydration level: " + soil.GetHydrationLevel());
        // check to see if we've collected enough water
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Animal")
        {
            // call becomeRestrained() on animal
            if (other.isTrigger == false)
            {
                other.gameObject.GetComponent<Animal>().becomeRestrained();
            }
            
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
    public void setSoil(GameObject s)
    {
        soil = s.gameObject.GetComponent<Soil>();
    }
    public Soil getSoil()
    {
        return soil;
    }
}