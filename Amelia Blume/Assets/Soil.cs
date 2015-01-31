using UnityEngine;
using System.Collections;

public class Soil : MonoBehaviour {
	private int HydrationLevel;
	private bool isPlanted;
	// Use this for initialization
	void Start () {
		HydrationLevel = 0;
		isPlanted = false;
	}
	
	// Update is called once per frame
	void Update () {
		Debug.Log (GetHydrationLevel ());
	}

	int GetHydrationLevel(){
		return HydrationLevel;
	}

	void ChangeHydrationLevel(int water){
		HydrationLevel += water;
	}

	bool CheckIsPlanted(){
		return isPlanted;
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Water") {
			ChangeHydrationLevel (1);
		}
	}



}
