using UnityEngine;
using System.Collections;

public class Color_Inversion : MonoBehaviour {
	Renderer[] renderers;
	Material[] materials;
	// Use this for initialization
	void Start () {
		float red;
		float green;
		float blue;
		renderers = GetComponentsInChildren<Renderer>();
		for(int i = 0; i < renderers.Length; i++){
			//Debug.Log(materials[i]);
			materials = renderers[i].materials;
			for(int j = 0; j < materials.Length; j++){
				red = materials[j].color.r;
				green = materials[j].color.g;
				blue = materials[j].color.b;
				//Debug.Log ("Before: " + materials[j].GetColor("_Color"));
				materials[j].color = new Color(1f - red, 1f - green, 1f - blue, 1f);
				//Debug.Log ("After: " + materials[j].GetColor("_Color"));
			}
		}
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
