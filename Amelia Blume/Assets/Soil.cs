using UnityEngine;
using System.Collections;

public class Soil : MonoBehaviour {
	private int HydrationLevel;
	private bool isPlanted;
	private Vector3 SeedLocation;
	private int SeedWater;
	private int SeedType;
	private int MaxWater;
	//private Seed plantedSeed;
	// Use this for initialization
	void Start () {
		HydrationLevel = 0;
		isPlanted = false;
		MaxWater = 100;
	}
	
	// Update is called once per frame
	void Update () {
		if (CheckIsPlanted()) {
			ChangeHydrationLevel(-1);
			//plantedSeed.CollectWater();
		}
		//Debug.Log (GetHydrationLevel ());
	}

	int GetHydrationLevel(){
		return HydrationLevel;
	}

	void SetSeedLocation(Vector3 location){
		SeedLocation = location;
	}

	Vector3 GetSeedLocation(){
		return SeedLocation;
	}

	void CreatePlant(){
		//
	}

	void ChangeHydrationLevel(int water){
		HydrationLevel += water;
	}

	bool CheckIsPlanted(){
		return isPlanted;
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Water" && GetHydrationLevel() < MaxWater) {
			ChangeHydrationLevel (1);
		}
	}

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Seed" && !CheckIsPlanted()) {
			//plantedSeed = other.gameObject.GetComponent<Seed>();
			//if(!plantedSeed.checkIfPlanted()){
				//plantedSeed.isPlanted = true;
				//this.isPlanted = true;
			//}
		}
	}



}
