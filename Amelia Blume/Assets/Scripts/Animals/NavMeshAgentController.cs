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
		//agent.SetDestination (target.position);
		//Debug.Log (target);
	}


}
