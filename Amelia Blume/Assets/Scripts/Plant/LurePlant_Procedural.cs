using UnityEngine;
using System.Collections.Generic;

public class LurePlantProcedural : MonoBehaviour {
	GameObject[] animals;
	public int health;
	Animal animalCon;

	[System.Serializable]
	public class PlantSettings {
		// how many faces does each vine segment have? 4 = square, 6 = hexagonal, etc.
		public int resolution = 5;
		public float maxStalkHeight = 2.0f;
		public float maxStalkWidth = 0.1f;
		public float maxOvarySize = 0.08f;
		public float petalDensity = 0.5f;
		public float petalSize = 0.3f;
		public Color petalColor = Color.red;
		public float stamenLength = 0.1f;
		public float stamenDensity = 0.5f;
		public float leafDensity = 0.2f;
		public float leafSize = 0.6f;
	}

	// Use this for initialization
	void Start () {
		animals = GameObject.FindGameObjectsWithTag ("Animal");
		health = 120;
		//Debug.Log (animals.Length);

		for(int i = 0; i < animals.Length;i++){
			animalCon = animals[i].GetComponent<Animal>();
			animalCon.LurePlant(this.transform);
			//Debug.Log (animals[i]);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other)
	{
		Debug.Log ("collision stay");
		if (other.gameObject.tag == "Animal") {
			this.health-=1;
		}
	}
	
}
