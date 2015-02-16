using UnityEngine;
using System.Collections;

public class Soil : MonoBehaviour {
	private int HydrationLevel;
	public bool isPlanted;
	private Vector3 SeedLocation;
	private int SeedWater;
	private int SeedType;
	private int MaxWater;
	private Seed plantedSeed;
	private int timer;
	public float alpha = 1;
	// Use this for initialization
	void Start () {
		HydrationLevel = 0;
		isPlanted = false;
		MaxWater = 100;
		timer = 0;
	}
	
	// Update is called once per frame
	void Update () {
		if (plantedSeed == null)
		{
			isPlanted = false;
		}
	}

	public int GetHydrationLevel(){
		return HydrationLevel;
	}

	public void SetSeedLocation(Vector3 location){
		SeedLocation = location;
	}

	public Vector3 GetSeedLocation(){
		return SeedLocation;
	}

	public void ChangeHydrationLevel(int water){
		HydrationLevel += water;
	}

	public bool CheckIsPlanted(){
		return isPlanted;
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Water" && GetHydrationLevel() < MaxWater) {
			ChangeHydrationLevel (1);
			Debug.Log("Soil water:" + GetHydrationLevel());
		}
	}

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Seed" && !CheckIsPlanted()) {
			plantedSeed = other.gameObject.GetComponent<Seed>();
			other.gameObject.GetComponent<Seed>().setSoil(this.gameObject);
			if(!plantedSeed.checkIfPlanted()){
				plantedSeed.setPlanted(true);
				this.isPlanted = true;
				//Debug.Log ("Planted");
			}
		}
	}



}
