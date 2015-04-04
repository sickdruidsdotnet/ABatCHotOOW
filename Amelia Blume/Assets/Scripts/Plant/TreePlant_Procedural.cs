using UnityEngine;
using System.Collections;
using System.Collections.Generic;

class TreePlant_Procedural : Plant
{
	public GameObject treeStructure;
	public GameObject rootStructure;
	public List<GameObject> leaves;
	public TreeSettings treeSettings;

	[System.Serializable]
	public class TreeSettings {
		// how many faces does each vine segment have? 4 = square, 6 = hexagonal, etc.
		public int stalkRingResolution = 8;
		public float stalkSegLength = 0.1f;
		public float stalkMaxHeight = 1.0f;
		public float stalkGrowthRate = 0.1f;
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

	void Start () {

		treeSettings = new TreeSettings();

		setRandomValues();

		treeStructure = Instantiate(Resources.Load("TreePlant/TreeStructure"), transform.position, Quaternion.identity) as GameObject;
		treeStructure.transform.parent = gameObject.transform;
	}

	private void setRandomValues()
    {
    	treeSettings.stalkMaxHeight = Random.Range(0.2f, 2.0f);
    	treeSettings.stalkGrowthRate = Random.Range(0.05f, 0.2f);
    	treeSettings.stalkMinWidth = Random.Range(0.01f, 0.05f);
    	treeSettings.stalkCrookedFactor = Random.Range(1f, 20f);

    	treeSettings.stalkColorHue = Random.Range(100, 130);
    	treeSettings.stalkColorSat = Random.Range(0.5f, 0.75f);
    	treeSettings.stalkColorVal = Random.Range(0.25f, 0.65f);

    	treeSettings.leafGrowthRate = Random.Range(treeSettings.stalkGrowthRate * 2f, treeSettings.stalkGrowthRate * 4f);
    	treeSettings.leafDensity = Random.Range(0.2f, 0.8f);
    	treeSettings.leafMaxLength = Random.Range(0.1f, 0.4f);
    	treeSettings.leafLengthVariation = Random.Range(0f, 0.15f);
    	treeSettings.leafWidth = Random.Range(0.04f, 0.12f);
    	treeSettings.leafWidthVariation = Random.Range(0f, 0.04f);
    	treeSettings.leafThickness = Random.Range(0.005f, 0.015f);
    	treeSettings.leafThicknessVariation = Random.Range(0f, 0.0025f);
    	treeSettings.leafCurlStart = Random.Range(-30f, 10f);
    	treeSettings.leafCurlVariation = Random.Range(0f, 15f);

    	treeSettings.petalGrowthRate = Random.Range(treeSettings.stalkGrowthRate * 2f, treeSettings.stalkGrowthRate * 4f);
    	treeSettings.petalCount = Random.Range(4,10);
    	treeSettings.petalLength = Random.Range(0.1f, 0.4f);
    	treeSettings.petalLengthVariation = Random.Range(0f, 0.15f);
    	treeSettings.petalWidth = Random.Range(0.04f, 0.12f);
    	treeSettings.petalWidthVariation = Random.Range(0f, 0.01f);
    	treeSettings.petalThickness = Random.Range(0.005f, 0.015f);
    	treeSettings.petalThicknessVariation = Random.Range(0f, 0.0025f);
    	treeSettings.petalCurlStart = Random.Range(25f, 40f);
    	treeSettings.petalCurlBloom = Random.Range(-15f, 5f);
    	treeSettings.petalCurlVariation = Random.Range(0f, 5f);
    	treeSettings.petalColorHue = Random.Range(0, 270);
    	if (treeSettings.petalColorHue > 70)
    	{
    		// skip any green hues
    		treeSettings.petalColorHue += 89;
    	}
    	treeSettings.petalColorSat = Random.Range(0.7f, 1f);
    	treeSettings.petalColorVal = Random.Range(0.85f, 1f);

    	treeSettings.bloomRate = Random.Range(0.1f, 0.8f);
		treeSettings.bloomMaturity = Random.Range(0.75f, 0.99f);
    }
}