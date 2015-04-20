using UnityEngine;
using System.Collections;

public class FernPlant : Plant 
{
	// Constructor
	public FernPlant()
	{
		
		// set hydrationGoal to match VineSeed requirements
		this.hydrationGoal = 200;
		//Debug.Log("FernPlant created");
		//Debug.Log("FernPlant hydrationGoal: " + hydrationGoal);
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Animal" && other.collider.isTrigger == false)
		{
			infect (other.transform.GetComponent<Animal>());
		}
	}

	void OnTriggerExit(Collider other){
		if (other.gameObject.tag == "Animal" && other.collider.isTrigger == false)
		{
			disInfect (other.transform.GetComponent<Animal>());
		}
	}

	public void infect(Animal other){
		other.becomeSpored();
		Vector3 spawnLoc = new Vector3(transform.position.x, transform.position.y, transform.position.z + 0.8f);
		Instantiate(Resources.Load("Spore_effect"), spawnLoc, Quaternion.identity);

	}

	public void disInfect(Animal other){
		if (other.isSpored) {
			other.resistSpores();
		}
	}
}
