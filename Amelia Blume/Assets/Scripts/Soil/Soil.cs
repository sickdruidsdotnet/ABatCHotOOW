using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Soil : MonoBehaviour {
	private int HydrationLevel;
	private bool isPlanted;
	private Vector3 SeedLocation;
	private int SeedWater;
	private int SeedType;
	private int MaxWater;
	private Seed plantedSeed;
//	private int timer;
	private float size;
	private float height;
	private float startHeight;
	public int[] water;
	private float start;
	private float slotSize;
	private int waterDropCount = 0;  
	//public GameObject[][] waterDrops; 
	private GameObject[] drops;

	int addCount = 0;
	int subtractCount = 0;
	// Use this for initialization
	void Start () {
		HydrationLevel = 0;
		isPlanted = false;
		MaxWater = 100;
//		timer = 0;
		height = renderer.bounds.size.y;
		size = renderer.bounds.size.x;
		//Debug.Log (waterDrops.Length);
		water = new int[10];
		start = this.transform.position.x - size / 2;
		startHeight = this.transform.position.y - height / 2;
		slotSize = size / water.Length;
		//waterDrops = new GameObject[10][];
		//for (int i = 0; i < waterDrops.Length; i++) {
		//	waterDrops[i] = new GameObject[30];
		//}
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
	void FixedUpdate () {
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
//		Debug.Log ("Change Hydration Level");
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
		//Debug.Log ("Change Water Count");
		water [index] += changeAmount;
		if (changeAmount > 0) {
			//while(changeAmount > 0){
				//AddWater(index);
				changeAmount-=1;
			//}
		} else if (changeAmount < 0) {
			while(changeAmount < 0){
				RemoveWater(index);
				changeAmount+=1;
			}
		}
		if (water [index] < 0)
			water [index] = 0;
	}

	void AddWater(int index){
//		int nextIndex = 0;
		waterDropCount += 1;
		addCount += 1;
		float posX;
		float posY;
		GameObject newDrop = (GameObject)Resources.Load ("waterDrop");
		newDrop.name = "Drop" + waterDropCount;
		if (index < water.Length) {
			posX = Random.Range (start + slotSize * index, start + slotSize * (index + 1));
		} else {
			posX = Random.Range (start + slotSize * index, start + slotSize * (index - 1));
		}
		posY = Random.Range(startHeight+(height/2), startHeight + height-0.25f);
		newDrop.transform.position = new Vector3 (posX, posY, -5);
		newDrop.GetComponent<WaterDrop> ().SetSoil (this);
		newDrop.GetComponent<WaterDrop> ().SetIndex (index);
		Instantiate (newDrop);
	
	}

	void RemoveWater(int index){
		float posY;
		float posX;
		float xUBound;
		float xLBound = start + slotSize * index;
		float yLBound = startHeight;
		float yUBound = startHeight + height;

		int newIndex = -1;
		if (index < water.Length) {
			xUBound = start + slotSize * (index + 1);
		} else {
			xUBound = start + slotSize * (index - 1);
		}

		float xMiddle = (xUBound + xLBound) / 2;
		float yMiddle = (yUBound + yLBound) / 2;

		drops = GameObject.FindGameObjectsWithTag ("waterDrop"); 
		for (int i = 0; i < drops.Length; i++) {
			//Debug.Log (drops[i]);
			if(drops[i].GetComponent<WaterDrop>().GetSoil() == this){
				posY = drops[i].transform.position.y;
				posX = drops[i].transform.position.x;
				//Debug.Log ("X DIST: " + Mathf.Abs((posX - xMiddle)));
				//Debug.Log ("Y DIST: " + Mathf.Abs((posY - yMiddle)));
				//Debug.Log (Mathf.Abs((posX - xMiddle)) < slotSize*3 && Mathf.Abs((posY - yMiddle)) < slotSize*3);
				if(Mathf.Abs((posX - xMiddle)) < slotSize*3 && Mathf.Abs((posY - yMiddle)) < slotSize*3){
				//if(posY > startHeight && posY < startHeight+height && posX > xLBound && posX < xUBound){
					newIndex = i;
					i = drops.Length+1;
				}
			}
		}
		//Debug.Log (newIndex);
		if (newIndex >= 0 && newIndex < drops.Length) {
			Destroy (drops [newIndex]);
			subtractCount++;
		}
	}

	public int getWaterLength()
	{
		return water.Length;
	}

	void OnParticleCollision(GameObject other) {
		if (other.tag == "Water" && GetHydrationLevel() < MaxWater) {
			ChangeHydrationLevel (3);
			int index;
			if(other.transform.rotation.y > 0)
				index = (int)((other.transform.position.x+2f - start)/slotSize);
			else
				index = (int)((other.transform.position.x-2f - start)/slotSize);
			if(index >= water.Length){
				index = water.Length-1;
			}
			if(index < 0){
				index = 0;
			}
			water[index]+=3;
			//for(int i = 0; i < 3; i++){
				AddWater(index);
			//}
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
