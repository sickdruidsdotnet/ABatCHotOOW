using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{

    //variables for however we're handling states, for now I'll do bools
    //protected AnimalState[] state;
    public bool isRestrained;
    public bool isInfected;
	public bool isSpored;
	//how long it will last in seconds after being infected by fern spores
	public float sporeResistance;

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
		isSpored = true;
		StartCoroutine (sporeTimer ());
	}

	IEnumerator sporeTimer()
	{
		yield return new WaitForSeconds (sporeResistance);
		if (isSpored) {
			becomeRestrained ();
		}
	}
}
