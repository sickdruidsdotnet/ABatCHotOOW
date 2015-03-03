using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{

    //variables for however we're handling states, for now I'll do bools
    //protected AnimalState[] state;
    public bool isRestrained;
    public bool isInfected;
	public bool isSpored;
	protected float sporeModifier = 1f;
	//how long it will last in seconds after being infected by fern spores
	public float sporeResistance;

	//this will store the location on each animal where the spore effect should spawn
	protected Vector3 sporeLoc;
    //public AnimalState[] getStates(){}

    public void addState(/*AnimalState*/) { }

    public bool isState(/*AnimalState*/) { return false; }

    public void removeState(/*AnimalState*/) { }

    public void becomeRestrained(/*&Plant*/)
    {
        isRestrained = true;
    }

    public void changeInfection()
    {
        isInfected = false;
    }

	public void becomeSpored()
	{
		//start the spore breath effect only while the animal is spored
		if(!isSpored)
			StartCoroutine (sporeSpawner ());
		isSpored = true;
		sporeModifier = 0.5f;
	}

	public void resistSpores()
	{
		sporeModifier = 0.75f;
		StartCoroutine (sporeTimer ());
	}

	IEnumerator sporeTimer()
	{
		yield return new WaitForSeconds (sporeResistance);
		//check to make sure they aren't back in the fern's AOE
		if (isSpored && sporeModifier == 0.75f) {
			sporeModifier = 1f;
			isSpored = false;
		}
	}

	IEnumerator sporeSpawner()
	{
		if (isSpored) {
			Instantiate(Resources.Load("Spore Breath"), transform.position + sporeLoc, Quaternion.identity);
			yield return new WaitForSeconds(1f);
			StartCoroutine(sporeSpawner());
		}
	}
}
