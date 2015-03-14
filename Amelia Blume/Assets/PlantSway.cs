using UnityEngine;
using System.Collections;

public class PlantSway : MonoBehaviour {
	float progress = 100f;
	bool swayRight = true;
	bool adding = true;
	
	// Update is called once per frame
	void Update () {
		if (adding) {
			progress++;
			if (progress >= 50f) {
				adding = !adding;
			}

		} else {
			progress--;
			if (progress <= 0f) {
				adding = !adding;
				swayRight = !swayRight;
			}
		}

		if (swayRight) {
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (1, (float)progress);
		} else {
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)progress);
		}

	}
}
