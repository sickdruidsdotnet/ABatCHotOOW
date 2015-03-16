using UnityEngine;
using System.Collections.Generic;

public class LurePlant_Procedural : Plant {

	public GameObject stalk;
	public List<GameObject> leaves;
	public List<GameObject> petals;
	private GameObject[] animals;
	public int health;
	private Animal animalCon;
	public PlantSettings plantSettings;

	[System.Serializable]
	public class PlantSettings {
		// how many faces does each vine segment have? 4 = square, 6 = hexagonal, etc.
		public int stalkRingResolution = 8;
		public float stalkSegLength = 0.1f;
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

	private int lastNodeCount = 0;

	// Use this for initialization
	void Start () {

		plantSettings = new PlantSettings();

		stalk = Instantiate(Resources.Load("LurePlant/PlantStalk"), transform.position, Quaternion.identity) as GameObject;
		stalk.transform.parent = gameObject.transform;
		stalk.GetComponent<PlantStalk>().resolution = plantSettings.stalkRingResolution;
		stalk.GetComponent<PlantStalk>().maxSegLength = plantSettings.stalkSegLength;


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

		// grow a leaf at each new node, excluding the top and bottom
		int nodeCount = stalk.GetComponent<PlantStalk>().getNodeCount();
		if (lastNodeCount < nodeCount)
		{
			if (nodeCount > 3)
			{
				int growNode = nodeCount - 3;
				Debug.Log("Spawning leaf at node " + growNode);
				spawnLeaf(growNode, Random.Range(0f, 360f));
			}

			lastNodeCount = nodeCount;
		}


		/*
		if (stalk.GetComponent<PlantStalk>().debugDoneGrowing)
		{
			spawnLeaf(5,90f);
			stalk.GetComponent<PlantStalk>().debugDoneGrowing = false;
		}
		*/
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

    /*
	void OnTriggerStay(Collider other)
	{
		Debug.Log ("collision stay");
		if (other.gameObject.tag == "Animal") {
			this.health-=1;
		}
	}
	*/
	
}
