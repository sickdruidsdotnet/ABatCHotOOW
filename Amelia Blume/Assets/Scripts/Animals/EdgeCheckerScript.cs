using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class EdgeCheckerScript : MonoBehaviour {

	private Deer deerScript;
	private Boar boarScript;
	public int count;
	public Collider[] colliders;
	List<Collider> lc;


	// Use this for initialization
	void Start () {
		lc = new List<Collider> ();
		deerScript = transform.parent.GetComponent<Deer> ();
		count = 0;
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (count == 0) {
			if (deerScript != null) {
				deerScript.beginRotate ();
			} 
		}

		for(int i = 0; i < lc.Count; i++)
		{
			if(lc[i] == null)
			{
				lc.RemoveAt(i);
				count--;
				i--;
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag != "Bramble") {
			count++;
			lc.Add(other);
			colliders = lc.ToArray();
		}
	}

	void OnTriggerExit(Collider other)
	{
		if (other.gameObject.tag != "Bramble") {
			count--;
			lc.Remove(other);
			colliders = lc.ToArray();
		}
	}
}
