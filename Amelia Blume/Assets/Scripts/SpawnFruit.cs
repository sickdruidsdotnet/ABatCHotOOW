using UnityEngine;
using System.Collections;

public class SpawnFruit : MonoBehaviour {

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnCollisionEnter(Collision other){
		GameObject amelia = GameObject.Find ("Player");
		Player script = amelia.GetComponent<Player>();
		script.SetCanGrow(true);
		Debug.Log ("Destroy fruit");
		Destroy(this.gameObject);
	}
}
