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
	private int soilIndex;

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

        //Debug.Log("Seed created");
        //Debug.Log("Default hydrationGoal: " + hydrationGoal);
    }
	void Start()
	{
		Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<BoxCollider> (), transform.GetComponent<SphereCollider>(), true);
		Physics.IgnoreCollision (GameObject.FindGameObjectWithTag("Player").GetComponent<CharacterController> (), transform.GetComponent<SphereCollider>(), true);
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

        Debug.Log(plantType);

        Vector3 loc = new Vector3(0, 0, 0);
        loc += transform.position;
        GameObject newPlant = Instantiate(Resources.Load(plantType), loc, Quaternion.identity) as GameObject;
        newPlant.gameObject.GetComponent<Plant>().setSoil(soil.gameObject);
		newPlant.gameObject.GetComponent<Plant>().SetSoilIndex (soilIndex);
		//newPlant.gameObject.GetComponent<Plant>().setSoil(soil.gameObject)
        Debug.Log("called sproutPlant()");

        // Destroy this Seed entity. The plant will carry on its legacy.
        // Goodnight, sweet seed. You served Amelia well.
        
        Object.Destroy(this.gameObject);
    }

    public void collectWater()
    {
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
		//Debug.Log ("Watercount: " + waterCount);
		//Debug.Log ("SoilIndex: " + soilIndex);
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

	public void setSoilIndex(int index)
	{
		soilIndex = index;
	}

	public int getSoilIndex()
	{
		return soilIndex;
	}

	//freezes the seed when it collides with soil
	void OnCollisionEnter(Collision collision)
	{
		if (collision.transform.tag == "Soil") {
			transform.rigidbody.constraints = RigidbodyConstraints.FreezePositionX | RigidbodyConstraints.FreezePositionZ | RigidbodyConstraints.FreezeRotation;
		} else if (collision.transform.tag == "Player") {
			Physics.IgnoreCollision(collision.collider, collider, true);
		}
	}

}