using UnityEngine;
using System.Collections;

public class RenderOptimizer : MonoBehaviour {

	public MeshRenderer[] allRenderers;
	public bool shouldRender = true;
	public bool rendered = true;
	GameObject mainCameraObject;

	// Use this for initialization
	void Start () {
		//get the camera to check position
		mainCameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		MeshRenderer[] childRenderers = gameObject.GetComponentsInChildren<MeshRenderer> ();
		MeshRenderer currentRenderer = gameObject.GetComponent<MeshRenderer> ();
		if (currentRenderer != null) {
			allRenderers = new MeshRenderer[childRenderers.Length + 1];
			childRenderers.CopyTo (allRenderers, 0);
			allRenderers [childRenderers.Length - 1] = currentRenderer;
		} else {
			allRenderers = childRenderers;
		}
		rendered = true;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		shouldRender = false;
		foreach (MeshRenderer rend in allRenderers) {
			Bounds thisBounds = rend.bounds;
			Vector3 checkPos = new Vector3(thisBounds.center.x + thisBounds.extents.x,
			                               thisBounds.center.y, thisBounds.center.z);
			Vector2 camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
			checkPos = new Vector3(thisBounds.center.x - thisBounds.extents.x,
			                       thisBounds.center.y, thisBounds.center.z);
			camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
			checkPos = new Vector3(thisBounds.center.x, thisBounds.center.y + thisBounds.extents.y, 
			                       thisBounds.center.z);
			camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
			checkPos = new Vector3(thisBounds.center.x, thisBounds.center.y - thisBounds.extents.y, 
			                       thisBounds.center.z);
			camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
		}

		if (shouldRender && !rendered) {
			foreach (MeshRenderer rend in allRenderers) {
				rend.enabled = true;
			}
			rendered = true;
		} else if (!shouldRender && rendered) {
			foreach (MeshRenderer rend in allRenderers) {
				rend.enabled = false;
			}
			rendered = false;
		}

		
	}
}
