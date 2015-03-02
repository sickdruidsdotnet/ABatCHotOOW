using UnityEngine;
using System.Collections;

public class FernPlant : Plant 
{
	public int sporesLeft;
	// Constructor
	public FernPlant()
	{
		
		// set hydrationGoal to match VineSeed requirements
		this.hydrationGoal = 200;
		this.sporesLeft = 3;
		Debug.Log("FernPlant created");
		Debug.Log("FernPlant hydrationGoal: " + hydrationGoal);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Animal" && other.collider.isTrigger == false && sporesLeft > 0)
		{
			infect (other.transform.GetComponent<Animal>());
			sporesLeft--;
		}
	}

	public void infect(Animal other){
		if (!other.isSpored) {
			other.becomeSpored();
			Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.8f);
			GameObject newPlant = Instantiate(Resources.Load("Spore_effect"), spawnLoc, Quaternion.identity) as GameObject;
		}
	}
}
