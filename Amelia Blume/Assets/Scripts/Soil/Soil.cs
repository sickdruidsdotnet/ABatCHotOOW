using UnityEngine;
using System.Collections;

public class Soil : MonoBehaviour {
	public int HydrationLevel;
	public bool isPlanted;
	private Vector3 SeedLocation;
	private int SeedWater;
	private int SeedType;
	private int MaxWater;
	private Seed plantedSeed;
//	private int timer;
	private float size;
	public int[] water;
	private float start;
	private float slotSize;
	// Use this for initialization
	void Start () {
		HydrationLevel = 0;
		isPlanted = false;
		MaxWater = 100;
//		timer = 0;
		size = renderer.bounds.size.x;
		water = new int[10];
		start = this.transform.position.x - size / 2;
		slotSize = size / water.Length;
		/*
		for (int i = 0; i < water.Length; i++) {
			float x = start+slotSize*i;
			GameObject fruit = GameObject.CreatePrimitive(PrimitiveType.Cube);
			fruit.transform.position = new Vector3(x, this.transform.position.y, this.transform.position.z);
			Instantiate(fruit);
		}
		*/
		//Debug.Log (start);
	}
	
	// Update is called once per frame
	void Update () {
		if (plantedSeed == null)
		{
			isPlanted = false;
		}
	}


	//Return Amount of Water in the complete plot of soil
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
		if (HydrationLevel < 0)
			HydrationLevel = 0;
	}

	public bool CheckIsPlanted(){
		return isPlanted;
	}

	//Return the amount of water in an indiviual slot
	public int GetWaterCount(int index)
	{
		return water[index];
	}

	public void ChangeWaterCount(int index, int changeAmount)
	{
		water [index] += changeAmount;
		if (water [index] < 0)
			water [index] = 0;
	}

	public int getWaterLength()
	{
		return water.Length;
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Water" && GetHydrationLevel() < MaxWater) {
			ChangeHydrationLevel (3);
			//Debug.Log("Soil water:" + GetHydrationLevel());
			//Debug.Log ("Position = " + other.transform.position.x);
			int index;
			if(other.transform.rotation.y > 0)
				index = (int)((other.transform.position.x+2f - start)/slotSize);
			else
				index = (int)((other.transform.position.x-2f - start)/slotSize);
			if(index >= water.Length)
				index = water.Length-1;
			//Debug.Log ("index: " + index);
			//Debug.Log ("Rotation: " + other.transform.rotation.y);
			water[index]+=3;
		}
	}

	void OnCollisionEnter(Collision other){
		if (other.gameObject.tag == "Seed") {
			plantedSeed = other.gameObject.GetComponent<Seed>();
			int index = (int)((other.transform.position.x - start)/slotSize);
			if(index == 10){
				index = 9;
				//Debug.Log ("Too big");
			}
			other.gameObject.GetComponent<Seed>().setSoil(this.gameObject);
			plantedSeed.setSoilIndex(index);
			//Debug.Log ("Planted: " + index);

			if(!plantedSeed.checkIfPlanted()){
				plantedSeed.setPlanted(true);
				//this.isPlanted = true;
				//Debug.Log ("Planted");
			}

		}
	}



}
