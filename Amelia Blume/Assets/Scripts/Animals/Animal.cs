using UnityEngine;
using System.Collections;

public class Animal : MonoBehaviour
{

    //variables for however we're handling states, for now I'll do bools
    //protected AnimalState[] state;
    public bool isRestrained;
    public bool isInfected;
	public bool isBeingLured;
	public Vector3 target;
	public float targetOffset;


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
		//NavMeshAgentController agentController = this.GetComponent<NavMeshAgentController> ();
		//agentController.EnableAgent (plantPosition);
		if (plantPosition.position.x > this.transform.position.x)
			targetOffset = -0.5f;
		else
			targetOffset = 1.2f;
		SetTarget (plantPosition.position);
		isBeingLured = true;
	}

	public void SetTarget(Vector3 position){
		this.target = new Vector3 (position.x + targetOffset, transform.position.y, transform.position.z);
	}



}
