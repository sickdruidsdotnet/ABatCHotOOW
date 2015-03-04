using UnityEngine;
using System.Collections;

public class LurePlant : MonoBehaviour {
	GameObject[] animals;
	public int health;
	Animal animalCon;
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
