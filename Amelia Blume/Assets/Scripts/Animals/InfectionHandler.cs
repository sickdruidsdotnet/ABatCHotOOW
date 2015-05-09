using UnityEngine;
using System.Collections;

public class InfectionHandler : MonoBehaviour {

	public GameObject infectionObject;
	Color originalColor;
	Color infectColor;
	public float saturationDif = 0.3f;

	// Use this for initialization
	void Start () {
		//load the infection 
		if (infectionObject == null) {
			foreach (Transform child in transform)
			{
				if (child.tag == "Infection")
				{
					infectionObject = child.gameObject;
					break;
				}
			}
		}
		Vector3 hsv = RGBandHSVconverter.RGBtoHSV (GetComponentInChildren<Renderer> ().material.color);
		hsv = new Vector3 (hsv.x, hsv.y - saturationDif, hsv.z);
		infectColor = RGBandHSVconverter.HSVtoRGB (hsv.x, hsv.y, hsv.z);

		originalColor = GetComponentInChildren<Renderer> ().material.color;
		GetComponentInChildren<Renderer> ().material.color = infectColor;
	}
	
	public void clearInfection(){
		GetComponentInChildren<Renderer> ().material.color = originalColor;
		Destroy (infectionObject);
	}
}
