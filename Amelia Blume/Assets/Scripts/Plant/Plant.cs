using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class Plant : MonoBehaviour
{
    public int waterCount;
    public int hydrationGoal;
    public float maturity;
    public Soil soil;
    public int collectionTimer;
    public int collectionDelay;
    private float scaleFactor;
	public float sunFactor = 1.0f;
    public float sunRange = 2.0f;
	private int soilIndex;
    public float matureRate = 0.05f;
    public float maturityGoal;
    public bool isMaturing = false;
    private GameObject amelia;
    private Player playerScript;
    private GameObject sun;

	public float startTime;
	public float lifeTime = 9999999f;
    
    // Constructor
    public Plant()
    {
        // the amount of water this seed has collected
        waterCount = 0;
        // waterCount threshold. once reached, sproutPlant() will be called
        hydrationGoal = 100;

        maturity = 0.0f;

        // initialize collection timer and delay
        collectionTimer = 0;
        collectionDelay = 30;
        
        //Debug.Log("Plant created");
        //Debug.Log("Default hydrationGoal: " + hydrationGoal);
    }

    void Start()
    {
        // this can't be in the constructor or Unity complains
		startTime = Time.time;
        amelia = GameObject.FindWithTag("Player");
        playerScript = amelia.GetComponent<Player>();
    }

    public virtual void Update()
    {
        if (amelia == null || playerScript == null)
        {
            getPlayer();
        }
        // maturity is value between 0 and 1. Used for linearly interpolating growth goals.
        maturityGoal = Mathf.Clamp01((float)waterCount / (float)hydrationGoal);

        //bool wasMaturing = isMaturing;

        if (maturity < maturityGoal)
        {
            isMaturing = true;
            maturity += matureRate * sunFactor * Time.deltaTime;

            if (maturity > maturityGoal)
            {
                maturity = maturityGoal;
            }
        }
        else
        {
            isMaturing = false;
        }

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
            //Debug.Log("Plant has no soil!");
        }
        
        sunFactor = 1.0f;
        if (playerScript.isSunning())
        {
            CollectSun();
            //Debug.Log("Sunning");
        }
        
        collectionTimer++;

        grow();
		if (Time.time - startTime >= lifeTime)
			Destroy (gameObject);
    }

    // Grows the plant.
    public virtual void grow()
    {
        // do something with procedural growth.

        /*
        maturity = (float)waterCount / (float)hydrationGoal;
        scaleFactor = 2.0f * maturity * Sunfactor;
        transform.localScale = new Vector3(baseScale.x + scaleFactor, baseScale.y + scaleFactor, baseScale.z + scaleFactor);
        */
    }

    public void collectWater()
    {
        // increment waterCount
        //waterCount++;
        //soil.ChangeHydrationLevel(-1);
		// increment waterCount
		//waterCount++;
		//soil.ChangeHydrationLevel(-1);
		if (soil.GetWaterCount (soilIndex) > 0) { //WaterCount of Current Slot
			//Debug.Log ("Soil Water Before: " + soilIndex + " = " + soil.GetWaterCount (soilIndex));
			soil.ChangeHydrationLevel(-3);
			soil.ChangeWaterCount(soilIndex, -3);
			//Debug.Log ("Soil Water After: " + soilIndex + " = " + soil.GetWaterCount (soilIndex));
			waterCount+=3;
		}
		
		if (soilIndex > 0) { //Water Count of left neighbor
			if (soil.GetWaterCount (soilIndex - 1) > 0) {
				//Debug.Log ("Soil Water Before: " + (soilIndex-1) + " = " + soil.GetWaterCount (soilIndex-1));
				soil.ChangeHydrationLevel (-2);
				soil.ChangeWaterCount (soilIndex-1, -2);
				//Debug.Log ("Soil Water After: " + (soilIndex-1) + " = " + soil.GetWaterCount (soilIndex-1));
				waterCount+=2;
			}
		}
		
		if (soilIndex < soil.getWaterLength () - 1) { //water count of right neighbor
			if (soil.GetWaterCount (soilIndex + 1) > 0) {
				//Debug.Log ("Soil Water Before: " + (soilIndex+1) + " = " + soil.GetWaterCount (soilIndex+1));
				soil.ChangeHydrationLevel (-2);
				soil.ChangeWaterCount (soilIndex + 1, -2);
				//Debug.Log ("Soil Water After: " + (soilIndex+1) + " = " + soil.GetWaterCount (soilIndex+1));
				waterCount+=2;
			}
		}
		
		if (soilIndex > 1) { //water count of 2nd left neighbor
			if (soil.GetWaterCount (soilIndex - 2) > 0) { 
				//Debug.Log ("Soil Water Before: " + (soilIndex-2) + " = " + soil.GetWaterCount (soilIndex-2));
				soil.ChangeHydrationLevel (-1);
				soil.ChangeWaterCount (soilIndex - 2, -1);
				//Debug.Log ("Soil Water After: " + (soilIndex-2) + " = " + soil.GetWaterCount (soilIndex-2));
				waterCount++;
			}
		}
		
		if (soilIndex < soil.getWaterLength () - 2) { //water count of 2nd right neighbor
			if (soil.GetWaterCount (soilIndex + 2) > 0) {
				//Debug.Log ("Soil Water Before: " + (soilIndex+2) + " = " + soil.GetWaterCount (soilIndex+2));
				soil.ChangeHydrationLevel (-1);
				soil.ChangeWaterCount (soilIndex + 2, -1);
				//Debug.Log ("Soil Water After: " + (soilIndex+2) + " = " + soil.GetWaterCount (soilIndex+2));
				waterCount++;
			}
		}

        //Debug.Log("Seed collected water. Seed waterCount: " + waterCount + ", Soil Hydration level: " + soil.GetHydrationLevel());
        // check to see if we've collected enough water
    }

	public void CollectSun()
	{
        if (sun == null)
        {
		  sun = GameObject.Find ("Sun");
        }
		if (sun != null) {
			float distance = transform.position.x - sun.transform.position.x;
			if (distance <= sunRange && distance >= -sunRange) {
				sunFactor = 1.5f;
                //Debug.Log("Collecting sun");
			}
		}
        else
        {
            //Debug.Log("Player is sunning, but can't find sun object!");
        }
	}

    private void getPlayer()
    {
        amelia = GameObject.FindWithTag("Player");
        playerScript = amelia.GetComponent<Player>();
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

	public void SetSoilIndex(int index)
	{
		soilIndex = index;
	}
}