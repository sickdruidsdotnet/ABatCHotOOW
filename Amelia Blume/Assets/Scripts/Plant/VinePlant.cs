using UnityEngine;
using System.Collections.Generic;

//This is the base class which is
//also known as the Parent class.
public class VinePlant : Plant 
{
	public int numVines = 5;
	private List<GameObject> vines;
	private float vineSpawnRadians;
	private float vineSpawnRadius = 0.2f;
    public float maxVineLength = 5f;
    public float growthRate = 0.35f;
    public float range = 0f;

    public bool wasGrowing = false;

    public GameObject[] animals;

    public GameObject targetedAnimal;

	public Animal[] restrainedAnimals;
	
    // Constructor
    public VinePlant()
    {
        
        // set hydrationGoal to match VineSeed requirements
        this.hydrationGoal = 200;
		this.restrainedAnimals = new Animal[3];
        //Debug.Log("VinePlant created");
        //Debug.Log("VinePlant hydrationGoal: " + hydrationGoal);
    }

    void Start()
    {
        // initialize vines
        vines = new List<GameObject>();

        // vines should be spawned in a ring around the VinePlant's location.
        vineSpawnRadians = 2 * Mathf.PI / numVines;

        for (int vine = 0; vine < numVines; vine++)
        {
            float angle = vine * vineSpawnRadians;
            float v_x = vineSpawnRadius * Mathf.Cos(angle);
            float v_z = vineSpawnRadius * Mathf.Sin(angle);
            Vector3 vineLocation = new Vector3(v_x, 0, v_z) + transform.position;

            GameObject newVine = Instantiate(Resources.Load("VinePlant/VinePrefab"), vineLocation, Quaternion.identity) as GameObject;
            newVine.transform.parent = gameObject.transform;

            vines.Add(newVine);
        }

        animals = GameObject.FindGameObjectsWithTag("Animal");


    }

    public override void Update()
    {
        base.Update();

        bool isGrowing = vines[0].GetComponent<Vine>().isGrowing;

        if (wasGrowing && !isGrowing)
        {
            //vines[0].GetComponent<Vine>().printSkeletonInfo();
        }

        wasGrowing = isGrowing;

        // update range based on vine length
        float range = 1.5f * vines[0].GetComponent<Vine>().length;

        // check if we have a target
        if (targetedAnimal != null)
        {
            // if we do, then make sure it's still in range
            if (Vector3.Distance(transform.position, targetedAnimal.transform.position) < range)
            {
                // cool, no need to do anything
            }
            else
            {
                Debug.Log("No longer targeting " + targetedAnimal.name);
                targetedAnimal = null;
                // call off our vines, they can't reach this target.
                foreach (GameObject vineObject in vines)
                {
                    vineObject.GetComponent<Vine>().removeAnimalTarget();
                }
            }
        }
        else
        {
            // if we don't have a target, let's see if any infected animals are in range
            {
                foreach (GameObject animalObject in animals)
                {
                    if (Vector3.Distance(transform.position, animalObject.transform.position) < range
                        && animalObject.GetComponent<Animal>().isInfected)
                    {
                        // oh cool, we found a viable target
                        targetedAnimal = animalObject;

                        Debug.Log("Now targeting " + targetedAnimal.name);
                        
                        // now let's pass this target along to the vines
                        foreach (GameObject vineObject in vines)
                        {
                            vineObject.GetComponent<Vine>().setAnimalTarget(targetedAnimal);
                        }

                        // no need to continue searching for animals
                        break;
                    }
                }
            }
        }
        
    }

    /*
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Animal" && other.collider.isTrigger == false)
		{
			restrain (other.transform.GetComponent<Animal>());
		}
	}
    */

    // restrain enemy
    public void restrain(Animal other)
    {
		// check to make sure it needs to be restrained
		if (other.isRestrained || !other.isInfected) {
			return;
		}

		// make enemy a child of this GameObject
		//doing that means anything affecting the plants transform(growth) would apply to animal
		//storing it in an array instead, for now anyway
		for (int i = 0; i < restrainedAnimals.Length; i++) {
			if (restrainedAnimals [i] == null) {
				restrainedAnimals [i] = other;
				other.becomeRestrained(collider);
				break;
			}
			else if(restrainedAnimals [i] == other)
			{
				//animal is already in here, restrained
				break;
			}
		}
 
    }

    public override void grow()
    {
        
        float goal = maturity * maxVineLength;

        for (int vine = 0; vine < vines.Count; vine++)
        {
            vines[vine].GetComponent<Vine>().setGrowthInfo(goal, growthRate);
        }
    }

    public void shredVines()
    {
        foreach (GameObject vine in vines)
        {
            vine.GetComponent<Vine>().shredVine();
        }

        Destroy(gameObject);
    }
}