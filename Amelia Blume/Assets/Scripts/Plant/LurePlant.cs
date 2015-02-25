using UnityEngine;
using System.Collections;

public class LurePlant : MonoBehaviour {
	GameObject[] animals;
	// Use this for initialization
	void Start () {
		animals = GameObject.FindGameObjectsWithTag ("Animal");
		//Debug.Log (animals.Length);
		/*
		for(int i = 0; i < animals.Length;i++){
			animals[i].GetComponent<Animal>().LurePlant();
		}
		 */
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
