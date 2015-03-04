using UnityEngine;
using System.Collections;

public class NavMeshAgentController : MonoBehaviour {

	public Transform target;
	public NavMeshAgent agent;

	// Use this for initialization
	void Start () {
		agent = GetComponent<NavMeshAgent> ();
		agent.enabled = false;

	}
	
	// Update is called once per frame
	void Update () {
		if (target != null)
			agent.enabled = true;
		Debug.Log ("Has path: " + agent.hasPath);
	}

	public void EnableAgent(Transform plant)
	{
		//target = plant;
		//agent.SetDestination (plant.position);
		//NavMeshPath path = new NavMeshPath ();
		//if(agent.CalculatePath (plant.position,path))
			//agent.SetPath (path);
		//agent.Warp (plant.position);

	}


}
