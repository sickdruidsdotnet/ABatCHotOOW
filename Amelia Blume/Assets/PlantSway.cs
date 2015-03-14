using UnityEngine;
using System.Collections;

public class PlantSway : MonoBehaviour {
	float progress = 0f;
	bool swayRight = true;
	
	// Update is called once per frame
	void Update () {
		progress = (Mathf.Sin (Time.time) *25f) + 25;
		if (progress <= 0f) {
			swayRight = !swayRight;
		}

		if (swayRight) {
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (1, (float)progress);
		} else {
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)progress);
		}

	}

	void OnTriggerExit(Collider other)
	{
		if (other.tag == "Animal") {
			if(other.transform.rigidbody.velocity.x < 0){
				Debug.Log ("Animal has passed right!");
			}
			else{
				Debug.Log ("Animal has passed left!");
			}
		}


	}
}
