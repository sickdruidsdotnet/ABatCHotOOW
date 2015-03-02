﻿using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{

    //variables for however we're handling states, for now I'll do bools
    //protected AnimalState[] state;
    public bool isRestrained;
    public bool isInfected;
	public bool isBeingLured;

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

	public void LurePlant(Transform plantPosition)
	{
		Debug.Log ("LurePlant called: " + gameObject.name);
		NavMeshAgentController agent = this.GetComponent<NavMeshAgentController> ();
		agent.target = plantPosition;
		agent.enabled = true;
		//agent.SetDestination (plantPosition);
		isBeingLured = true;
	}

	public void DoThis()
	{
		Debug.Log ("Debug");
	}

}
