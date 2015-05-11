﻿using UnityEngine;
using System.Collections;

public class Boar_Spawner : MonoBehaviour {

	public bool isSpawner = false;
	public bool boarSpawned = false;
	public Transform boarPrefab;
	public GameObject activeBoar = null;
	public GameObject spawner = null;
	public GameObject despawner = null;

	GameObject mainCamera;

	void Start(){
		mainCamera = GameObject.FindGameObjectWithTag ("MainCamera");
	}
	
	void OnTriggerEnter(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			//check if it's the trigger for spawning the boar
			if(isSpawner){
				//check if it's been spawned already
				if(!boarSpawned)
				{
					boarSpawned = true;
					activeBoar = Instantiate(boarPrefab, new Vector3( transform.position.x - 8f, transform.position.y, 0), 
					                         new Quaternion(0f, 0.5f, 0f, 1f)) as GameObject;
					activeBoar = GameObject.Find ("Boar(Clone)");
					despawner.GetComponent<Boar_Spawner>().LoadBoar();
					//update the camera
					mainCamera.BroadcastMessage("recalculateTrackables");
				}
			}
			else{
				if(activeBoar != null){
					Destroy (activeBoar);
					boarSpawned = false;
					spawner.GetComponent<Boar_Spawner> ().boarSpawned = false;
					mainCamera.BroadcastMessage("recalculateTrackables");

				}
			}
		}
	}

	public void LoadBoar(){
		boarSpawned = true;
		activeBoar = GameObject.Find ("Boar(Clone)");
	}
}
