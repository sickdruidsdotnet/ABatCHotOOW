using UnityEngine;
using System.Collections;

public class WaterDrop : MonoBehaviour {
	public Soil mySoil;
	int index;
	public float red;
	public float green;
	public float blue;
	public Color myColor;
	public Color startColor;
	Renderer myRenderer;
	Material myMaterial;
	float startTime;
	// Use this for initialization
	void Start () {
		//name = this.name;
		this.transform.localScale = new Vector3 (0.001f, 0.001f, 0.001f);
		myRenderer = GetComponent<Renderer> ();
		myMaterial = myRenderer.material;
		//Debug.Log (myMaterial.color.r);
		//Debug.Log (myMaterial.color.g);
		//Debug.Log (myMaterial.color.b);
		startTime = Time.time;
		startColor = myMaterial.color;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//Debug.Log ("Seconds = " + (Time.time - startTime));
		myColor = myMaterial.color;
		red = myColor.r;
		green = myColor.g;
		blue = myColor.b;
		if (this.transform.localScale.x < 0.3) {
			this.transform.localScale += new Vector3 (0.005f, 0.005f, 0.005f);
		} else {
			if(myColor.r < 0.5567f){
				red += 0.03f;
			}
			if(myColor.g > 0.2784f){
				green -= 0.06f;
			}
			if(myColor.b > 0.1765f){
				blue -= 0.06f;
			}
			myMaterial.color = new Vector4(red,green,blue,1.0f);
		}

		if (Time.time - startTime > 30f) {
			//Debug.Log ("Destroy");
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
