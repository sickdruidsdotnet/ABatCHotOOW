using UnityEngine;
using System.Collections;

public class WaterDrop : MonoBehaviour {
	public Soil mySoil;
	int index;
	public float red;
	public float green;
	public float blue;
	public Color myColor;
	Renderer myRenderer;
	Material myMaterial;
	float startTime;
	// Use this for initialization
	void Start () {
		//name = this.name;
		this.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		myRenderer = GetComponent<Renderer> ();
		myMaterial = myRenderer.material;
		Debug.Log (myMaterial.color.r);
		Debug.Log (myMaterial.color.g);
		Debug.Log (myMaterial.color.b);
		startTime = Time.time;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		Debug.Log ("Seconds = " + (Time.time - startTime));
		myColor = myMaterial.color;
		if (this.transform.localScale.x < 0.3) {
			this.transform.localScale += new Vector3 (0.005f, 0.005f, 0.005f);
		} else {
			if(myMaterial.color.r < 0.2980f){
				red += 0.2f;
			}
			if(myMaterial.color.g > 0.1137f){
				green -= 0.3f;
			}
			if(myMaterial.color.b > 0.0353f){
				blue -= 0.5f;
			}
			myMaterial.color = new Color(red,green,blue,1);
		}

		if (Time.time - startTime > 10f) {
			Debug.Log ("Destroy");
			Destroy(this.gameObject);
		}
	}

	public Soil GetSoil(){
		return mySoil;
	}

	public void SetSoil(Soil newSoil){
		mySoil = newSoil;
	}

	public void Kill(){
		//this.Destroy ();
		DestroyObject (this.gameObject);
	}

	public void SetIndex(int newIndex){
		index = newIndex;
	}

	public int GetIndex(){
		return index;
	}
}
