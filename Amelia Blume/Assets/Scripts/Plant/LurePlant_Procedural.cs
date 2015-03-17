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
		public float maxStalkHeight = 1.0f;
		public float maxStalkWidth = 0.1f;
		public float maxOvarySize = 0.08f;
		public float petalDensity = 0.5f;
		public float petalSize = 0.3f;
		public Color petalColor = Color.red;
		public float stamenLength = 0.1f;
		public float stamenDensity = 0.5f;
		public float leafDensity = 0.2f;
		public float leafSize = 0.6f;
		public float bloomRate = 0.3f;

	}

	private int lastNodeCount = 0;

	// Use this for initialization
	void Start () {

		plantSettings = new PlantSettings();

		stalk = Instantiate(Resources.Load("LurePlant/PlantStalk"), transform.position, Quaternion.identity) as GameObject;
		stalk.transform.parent = gameObject.transform;
		stalk.GetComponent<PlantStalk>().resolution = plantSettings.stalkRingResolution;
		stalk.GetComponent<PlantStalk>().maxSegLength = plantSettings.stalkSegLength;
		stalk.GetComponent<PlantStalk>().lengthGoal = plantSettings.maxStalkHeight;


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
			if (nodeCount > 1 && petals.Count == 0)
			{
				spawnBud(6, 25f, -5f, plantSettings.bloomRate);

			}

			if (nodeCount > 3)
			{
				int growNode = nodeCount - 3;
				//Debug.Log("Spawning leaf at node " + growNode);
				spawnLeaf(growNode, Random.Range(0f, 360f), Random.Range(-30f, 30f), 0);
			}

			lastNodeCount = nodeCount;
		}

		if (petals.Count > 0)
		{
			stalk.GetComponent<PlantStalk>().repositionBud(petals);
		}


		
		if (stalk.GetComponent<PlantStalk>().debugDoneGrowing)
		{
			bloomFlower();	
			stalk.GetComponent<PlantStalk>().debugDoneGrowing = false;
		}
		
	}

    private void spawnLeaf(int node, float rotation, float curlStart, float curlBloom)
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
    	leaf.GetComponent<PlantLeaf>().setLeafInfo(node, rotation, curlStart, curlBloom);
    	stalk.GetComponent<PlantStalk>().repositionLeaf(leaf);
    	leaves.Add(leaf);
    }

    private void spawnPetal(int node, float rotation, float curlStart, float curlBloom, float bloomRate)
    {
    	GameObject petal;
    	petal = Instantiate(Resources.Load("LurePlant/PlantPetal"), transform.position, Quaternion.identity) as GameObject;
    	if (petal == null)
    	{
    		Debug.Log("petal is null?");
    	}
    	if (stalk == null)
    	{
    		Debug.Log("stalk is null?");
    	}
    	petal.transform.parent = stalk.transform;
    	petal.GetComponent<PlantPetal>().setPetalInfo(node, rotation, curlStart, curlBloom, bloomRate);
    	stalk.GetComponent<PlantStalk>().repositionLeaf(petal);
    	petals.Add(petal);
    }

    private void spawnBud(int numPetals, float curlStart, float curlBloom, float bloomRate)
    {
    	int lastNodeIndex = stalk.GetComponent<PlantStalk>().getNodeCount() - 1;
    	float petalAngle = 360f / numPetals;

    	for (int p = 0; p < numPetals; p++)
    	{
    		spawnPetal(lastNodeIndex, petalAngle * (p+1), curlStart, curlBloom, bloomRate);
    	}
    }

    private void bloomFlower()
    {
    	for (int p = 0; p < petals.Count; p++)
    	{
    		petals[p].GetComponent<PlantPetal>().bloomTrigger = true;
    	}
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
