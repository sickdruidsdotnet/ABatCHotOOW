using UnityEngine;
using System.Collections;

public class LurePlant : MonoBehaviour {
	GameObject[] animals;
	int health;
	Animal animalCon;
	// Use this for initialization
	void Start () {
		animals = GameObject.FindGameObjectsWithTag ("Animal");
		health = 120;
		//Debug.Log (animals.Length);

		for(int i = 0; i < animals.Length-1;i++){
			animalCon = animals[i].GetComponent<Animal>();
			animalCon.LurePlant(this.transform);
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}

	void OnColliderStay(Collider other)
	{
		if (other.gameObject.tag == "Animal") {
			this.health-=1;
		}
	}
}
