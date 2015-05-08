using UnityEngine;
using System.Collections.Generic;

public class Flower : Plant {

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
		public int stalkRingResolution = 5;
		public float stalkSegLength = 0.1f;
		public float stalkMaxHeight = 3.5f;
		public float stalkGrowthRate = 0.2f;
		public float stalkMinWidth = 0.02f;
		public float stalkCrookedFactor = 10f;
		public int stalkColorHue = 120;
		public float stalkColorSat = 0.5f;
		public float stalkColorVal = 0.5f;

		public float leafGrowthRate = 0.6f;
		public float leafDensity = 0.2f;
		public float leafMaxLength = 0.2f;
		public float leafLengthVariation = 0.05f;
		public float leafWidth = 0.08f;
		public float leafWidthVariation = 0.02f;
		public float leafThickness = 0.01f;
		public float leafThicknessVariation = 0.005f;
		public float leafCurlStart = 0f;
		public float leafCurlVariation = 7.5f;

		public float petalGrowthRate = 0.6f;
		public int petalCount = 5;
		public float petalLength = 0.2f;
		public float petalLengthVariation = 0.07f;
		public float petalWidth = 0.08f;
		public float petalWidthVariation = 0.005f;
		public float petalThickness = 0.01f;
		public float petalThicknessVariation = 0.005f;
		public float petalCurlStart = 30f;
		public float petalCurlBloom = 0f;
		public float petalCurlVariation = 2.5f;
		public int petalColorHue = 200;
		public float petalColorSat = 0.9f;
		public float petalColorVal = 0.9f;

		public float bloomRate = 0.3f;
		public float bloomMaturity = 0.8f;

		public float ovaryMaxSize = 0.08f;
		public float stamenLength = 0.1f;
		public float stamenDensity = 0.5f;
	}

	private int lastNodeCount = 0;

	// Use this for initialization
	void Start () {
		
		plantSettings = new PlantSettings();

		setRandomValues();

		stalk = Instantiate(Resources.Load("LurePlant/PlantStalk"), transform.position, Quaternion.identity) as GameObject;
		stalk.transform.parent = gameObject.transform;
		stalk.GetComponent<PlantStalk>().resolution = plantSettings.stalkRingResolution;
		stalk.GetComponent<PlantStalk>().maxSegLength = plantSettings.stalkSegLength;
		stalk.GetComponent<PlantStalk>().lengthGoal = plantSettings.stalkMaxHeight;
		stalk.GetComponent<PlantStalk>().initialRadius = plantSettings.stalkMinWidth;
		stalk.GetComponent<PlantStalk>().growthRate = plantSettings.stalkGrowthRate;
		stalk.GetComponent<PlantStalk>().colorHue = plantSettings.stalkColorHue;
		stalk.GetComponent<PlantStalk>().colorSat = plantSettings.stalkColorSat;
		stalk.GetComponent<PlantStalk>().colorVal = plantSettings.stalkColorVal;


		animals = GameObject.FindGameObjectsWithTag ("Animal");
		health = 120;
		//Debug.Log (animals.Length);

		for(int i = 0; i < animals.Length;i++){
			animalCon = animals[i].GetComponent<Animal>();
			animalCon.LurePlant(this.transform);
			//Debug.Log (animals[i]);
		}
	}

	private void setRandomValues()
    {
    	plantSettings.stalkMaxHeight = Random.Range(1.5f, 2.0f);
    	//plantSettings.stalkGrowthRate = Random.Range(0.05f, 0.2f);
    	plantSettings.stalkMinWidth = Random.Range(0.01f, 0.05f);
    	plantSettings.stalkCrookedFactor = Random.Range(1f, 20f);

    	plantSettings.stalkColorHue = Random.Range(100, 130);
    	plantSettings.stalkColorSat = Random.Range(0.5f, 0.75f);
    	plantSettings.stalkColorVal = Random.Range(0.25f, 0.65f);

    	plantSettings.leafGrowthRate = Random.Range(plantSettings.stalkGrowthRate * 2f, plantSettings.stalkGrowthRate * 4f);
    	plantSettings.leafDensity = Random.Range(0.2f, 0.8f);
    	plantSettings.leafMaxLength = Random.Range(0.1f, 0.4f);
    	plantSettings.leafLengthVariation = Random.Range(0f, 0.15f);
    	plantSettings.leafWidth = Random.Range(0.04f, 0.12f);
    	plantSettings.leafWidthVariation = Random.Range(0f, 0.04f);
    	plantSettings.leafThickness = Random.Range(0.005f, 0.015f);
    	plantSettings.leafThicknessVariation = Random.Range(0f, 0.0025f);
    	plantSettings.leafCurlStart = Random.Range(-30f, 10f);
    	plantSettings.leafCurlVariation = Random.Range(0f, 15f);

    	plantSettings.petalGrowthRate = Random.Range(plantSettings.stalkGrowthRate * 2f, plantSettings.stalkGrowthRate * 4f);
    	plantSettings.petalLength = Random.Range(0.1f, 0.4f);
    	plantSettings.petalLengthVariation = Random.Range(0f, 0.15f);
    	plantSettings.petalWidth = Random.Range(0.04f, 0.12f);
    	plantSettings.petalCount = Random.Range(4,10);
    	// designate enough petals to ensure bud coverage
    	float circumference = 2 * Mathf.PI * plantSettings.stalkMinWidth;
    	float petalBaseWidth = 0.614f * plantSettings.petalWidth;
    	int minNumPetals = Mathf.CeilToInt(circumference / petalBaseWidth);

    	Debug.Log("circumference = " + circumference);
    	Debug.Log("petal width = " + petalBaseWidth);
    	Debug.Log("minNumPetals = " + minNumPetals);

    	plantSettings.petalCount = Mathf.Max(6, Random.Range(minNumPetals, minNumPetals));

    	plantSettings.petalWidthVariation = Random.Range(0f, 0.01f);
    	plantSettings.petalThickness = Random.Range(0.005f, 0.015f);
    	plantSettings.petalThicknessVariation = Random.Range(0f, 0.0025f);
    	plantSettings.petalCurlStart = Random.Range(25f, 40f);
    	plantSettings.petalCurlBloom = Random.Range(-15f, 5f);
    	plantSettings.petalCurlVariation = Random.Range(0f, 5f);
    	plantSettings.petalColorHue = Random.Range(-30, 30);
    	// if (plantSettings.petalColorHue > 70)
    	// {
    	// 	// skip any green hues
    	// 	plantSettings.petalColorHue += 89;
    	// }
    	plantSettings.petalColorSat = Random.Range(0.7f, 1f);
    	plantSettings.petalColorVal = Random.Range(0.85f, 1f);

    	plantSettings.bloomRate = Random.Range(0.1f, 0.8f);
		plantSettings.bloomMaturity = Random.Range(0.75f, 0.99f);
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
				spawnBud(plantSettings.petalCount, plantSettings.bloomRate);

			}

			if (nodeCount > 3)
			{
				// decide whether to grow a leaf based on the plant's leaf density
				if (Random.value < plantSettings.leafDensity)
				{
					int growNode = nodeCount - 3;
					// determine random values for this leaf
					float angle = Random.Range(0f, 360f);
					float cStart = plantSettings.leafCurlStart + Random.Range(-plantSettings.leafCurlVariation, plantSettings.leafCurlVariation);
					float len = plantSettings.leafMaxLength + Random.Range(-plantSettings.leafLengthVariation, plantSettings.leafLengthVariation);
					float wid = plantSettings.leafWidth + Random.Range(-plantSettings.leafWidthVariation, plantSettings.leafWidthVariation);
					float thick = plantSettings.leafThickness + Random.Range(-plantSettings.leafThicknessVariation, plantSettings.leafThicknessVariation);

					spawnLeaf(growNode, angle, cStart, len, wid, thick);
				}
			}

			lastNodeCount = nodeCount;
		}

		if (petals.Count > 0)
		{
			stalk.GetComponent<PlantStalk>().repositionBud(petals);
		}

		if (stalk.GetComponent<PlantStalk>().debugDoneGrowing)
		{
			stalk.GetComponent<PlantStalk>().debugDoneGrowing = false;
		}

		if (this.maturity > plantSettings.bloomMaturity)
		{
			bloomFlower();
		}
	}

    private void spawnLeaf(int node, float rotation, float curlStart, float length, float width, float thickness)
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
    	leaf.GetComponent<PlantLeaf>().stalkNode = node;
    	leaf.GetComponent<PlantLeaf>().growthAngle = rotation;
    	leaf.GetComponent<PlantLeaf>().curlStart = curlStart;
    	leaf.GetComponent<PlantLeaf>().lengthGoal = length;
    	leaf.GetComponent<PlantLeaf>().leafWidth = width;
    	leaf.GetComponent<PlantLeaf>().leafThickness = thickness;
		leaf.GetComponent<PlantLeaf>().colorHue = plantSettings.stalkColorHue;
		leaf.GetComponent<PlantLeaf>().colorSat = plantSettings.stalkColorSat;
		leaf.GetComponent<PlantLeaf>().colorVal = plantSettings.stalkColorVal;

    	stalk.GetComponent<PlantStalk>().repositionLeaf(leaf);
    	leaves.Add(leaf);
    }

    private void spawnPetal(int node, float rotation, float curlStart, float curlBloom, float bloomRate, float length, float width, float thickness)
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
    	petal.GetComponent<PlantPetal>().stalkNode = node;
    	petal.GetComponent<PlantPetal>().growthAngle = rotation;
    	petal.GetComponent<PlantPetal>().curlStart = curlStart;
    	petal.GetComponent<PlantPetal>().curlBloom = curlBloom;
    	petal.GetComponent<PlantPetal>().lengthGoal = length;
    	petal.GetComponent<PlantPetal>().petalWidth = width;
    	petal.GetComponent<PlantPetal>().petalThickness = thickness;
    	petal.GetComponent<PlantPetal>().colorHue = plantSettings.petalColorHue;
    	petal.GetComponent<PlantPetal>().colorSat = plantSettings.petalColorSat;
    	petal.GetComponent<PlantPetal>().colorVal = plantSettings.petalColorVal;

    	stalk.GetComponent<PlantStalk>().repositionLeaf(petal);
    	petals.Add(petal);
    }

    private void spawnBud(int numPetals, float bloomRate)
    {
    	int lastNodeIndex = stalk.GetComponent<PlantStalk>().getNodeCount() - 1;
    	float petalAngle = 360f / numPetals;

    	for (int p = 0; p < numPetals; p++)
    	{
    		// determine random variables for this petal
    		float cStart = plantSettings.petalCurlStart;
    		float cBloom = plantSettings.petalCurlBloom + Random.Range(-plantSettings.petalCurlVariation, plantSettings.petalCurlVariation);
			float len = plantSettings.petalLength + Random.Range(-plantSettings.petalLengthVariation, plantSettings.petalLengthVariation);
			float wid = plantSettings.petalWidth + Random.Range(-plantSettings.petalWidthVariation, plantSettings.petalWidthVariation);
			float thick = plantSettings.petalThickness + Random.Range(-plantSettings.petalThicknessVariation, plantSettings.petalThicknessVariation);

    		spawnPetal(lastNodeIndex, petalAngle * (p+1), cStart, cBloom, bloomRate, len, wid, thick);
    	}
    }

    private void bloomFlower()
    {
    	for (int p = 0; p < petals.Count; p++)
    	{
    		petals[p].GetComponent<PlantPetal>().bloomTrigger = true;
    	}
    }

    public override void grow()
    {
        
        float goal = maturity * plantSettings.stalkMaxHeight;

        stalk.GetComponent<PlantStalk>().setGrowthInfo(goal, plantSettings.stalkGrowthRate);
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