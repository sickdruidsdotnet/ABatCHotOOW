using UnityEngine;
using System.Collections.Generic;

public class LurePlant_Procedural : Plant {

	public GameObject stalk;
	public List<GameObject> leaves;
	public List<GameObject> petals;
	private GameObject[] animals;
	public int health;
	private Animal animalCon;

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
		stalk = Instantiate(Resources.Load("LurePlant/PlantStalk"), transform.position, Quaternion.identity) as GameObject;

		animals = GameObject.FindGameObjectsWithTag ("Animal");
		health = 120;
		//Debug.Log (animals.Length);

		for(int i = 0; i < animals.Length;i++){
			animalCon = animals[i].GetComponent<Animal>();
			animalCon.LurePlant(this.transform);
			//Debug.Log (animals[i]);
		}
	}

	public override void Update()
	{
		base.Update();
		if (stalk.GetComponent<PlantStalk>().debugDoneGrowing)
		{
			spawnLeaf(5,0f);
			stalk.GetComponent<PlantStalk>().debugDoneGrowing = false;
		}
	}

    private void spawnLeaf(int node, float rotation)
    {
    	GameObject leaf;
    	leaf = Instantiate(Resources.Load("LurePlant/PlantLeaf"), transform.position, Quaternion.identity) as GameObject;
    	if (leaf == null)
    	{
    		Debug.Log("leaf is null?");
    	}
    	if (stalk == null)
    	{
    		Debug.Log("stalk is null?");
    	}
    	leaf.transform.parent = stalk.transform;
    	leaf.GetComponent<PlantLeaf>().setLeafInfo(node, rotation);
    	stalk.GetComponent<PlantStalk>().repositionLeaf(leaf);
    }

	void OnTriggerStay(Collider other)
	{
		Debug.Log ("collision stay");
		if (other.gameObject.tag == "Animal") {
			this.health-=1;
		}
	}
	
}
