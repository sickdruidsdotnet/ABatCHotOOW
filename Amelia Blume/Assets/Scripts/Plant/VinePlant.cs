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
    public float maxVineLength = 7f;
    public float growthRate = 0.35f;

	public Animal[] restrainedAnimals;
	
    // Constructor
    public VinePlant()
    {
        
        // set hydrationGoal to match VineSeed requirements
        this.hydrationGoal = 200;
		this.restrainedAnimals = new Animal[3];
        Debug.Log("VinePlant created");
        Debug.Log("VinePlant hydrationGoal: " + hydrationGoal);
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

            vines.Add(Instantiate(Resources.Load("VinePrefab"), vineLocation, Quaternion.identity) as GameObject);
        }
    }

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Animal" && other.collider.isTrigger == false)
		{
			restrain (other.transform.GetComponent<Animal>());
		}
	}

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
				other.becomeRestrained();
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
}