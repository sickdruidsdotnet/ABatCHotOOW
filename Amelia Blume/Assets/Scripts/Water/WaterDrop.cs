using UnityEngine;
using System.Collections;

public class WaterDrop : MonoBehaviour {
	public string name;
	// Use this for initialization
	void Start () {
		name = this.name;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	public void Kill(){
		//this.Destroy ();
		DestroyObject (this.gameObject);
	}
}
