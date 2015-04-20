using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class TreePlant : Plant {

	// how long the tree stay out in seconds;
	public bool isPermanent;
	public float lifeSpan;
	public float fadeTime;
	public int fadeProgress;
	bool fading;
	List<Transform> childrenWithMesh = new List<Transform>();

	public TreePlant()
	{
		//default to 20 seconds
		this.lifeSpan = 15;
		this.fadeTime = 3;
	}

	// Use this for initialization
	void Start () {
		fading = false;
		fadeTime = this.lifeSpan / 5;
		if(!isPermanent)
			StartCoroutine(destroyTimer ());
		fadeProgress = (int)(fadeTime * 60f);
		MeshRenderer temp;
		foreach (Transform child in transform) {
			temp = child.GetComponent<MeshRenderer>();
			if(temp != null)
				childrenWithMesh.Add(child);

			foreach (Transform grandChild in child.transform)
			{
				temp = grandChild.GetComponent<MeshRenderer>();
				if(temp != null)
					childrenWithMesh.Add(grandChild);
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
		if (!isPermanent && fading) {
			for(int i = 0; i < childrenWithMesh.Count; i++)
			{
				Color newColor = new Color(childrenWithMesh[i].renderer.material.color.r, 
				                           childrenWithMesh[i].renderer.material.color.g,
				                           childrenWithMesh[i].renderer.material.color.b,
				                           ((float)fadeProgress/(fadeTime*60f)));
				childrenWithMesh[i].renderer.material.SetColor("_Color", newColor);
			}
			fadeProgress--;
		}
	}

	IEnumerator destroyTimer()
	{
		yield return new WaitForSeconds (lifeSpan - fadeTime);
		fading = true;
		yield return new WaitForSeconds (fadeTime);
		Destroy (this.gameObject);
	}
}
