using UnityEngine;
using System.Collections;

public class SporeEffectScript : MonoBehaviour {

	public int startLife = 120;
	public float transparencyMax = 0.5f;
	public bool skipStart = false;
	int life;
	int lifeSpanThird;
	float transparency;
	float transparencySpeed;
	// Use this for initialization
	void Start () {
		if (skipStart){
			transparency = transparencyMax;
		} else {
			transparency = 0f;
		}
		foreach (Transform child in transform) {
			Color newColor = new Color(child.renderer.material.color.r, 
			                           child.renderer.material.color.g,
			                           child.renderer.material.color.b,
			                           transparency);
			child.renderer.material.SetColor("_Color", newColor);
		}
		lifeSpanThird = startLife / 3;
		life = startLife;
		transparencySpeed = (float)transparencyMax / (float)lifeSpanThird;
	}
	
	// Update is called once per frame
	void Update () {
		if (life <= 0) {
			Destroy(this.gameObject);
		} else if (!skipStart && life > startLife - lifeSpanThird) {
			transparency += transparencySpeed;
			foreach (Transform child in transform) {
				Color newColor = new Color(child.renderer.material.color.r, 
				                           child.renderer.material.color.g,
				                           child.renderer.material.color.b,
				                           transparency);
				child.renderer.material.SetColor("_Color", newColor);
			}
		} else if (life < startLife - (2 * lifeSpanThird) ) {
			transparency -= transparencySpeed;
			foreach (Transform child in transform) {
				Color newColor = new Color (child.renderer.material.color.r, 
				                            child.renderer.material.color.g,
				                            child.renderer.material.color.b,
				                            transparency);
				child.renderer.material.SetColor ("_Color", newColor);
			}
		} 
		life--;
	}
}
