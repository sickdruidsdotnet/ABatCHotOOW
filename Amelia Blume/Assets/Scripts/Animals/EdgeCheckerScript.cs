using UnityEngine;
using System.Collections;

public class EdgeCheckerScript : MonoBehaviour {

	private Deer deerScript;
	private Boar boarScript;
	public int count;

	// Use this for initialization
	void Start () {
		deerScript = transform.parent.GetComponent<Deer> ();
		boarScript = transform.parent.GetComponent<Boar> ();
		count = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (count == 0) {
			if (deerScript != null) {
				deerScript.beginRotate ();
			} else if (boarScript != null) {
				boarScript.beginRotate ();
			}
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if(other.gameObject.tag != "Bramble")
		count++;
	}

	void OnTriggerExit(Collider other)
	{
		if(other.gameObject.tag != "Bramble")

		count--;
	}
}
