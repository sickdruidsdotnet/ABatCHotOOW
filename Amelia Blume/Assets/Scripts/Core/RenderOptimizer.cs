using UnityEngine;
using System.Collections;

public class RenderOptimizer : MonoBehaviour {

	public Renderer[] allRenderers;
	public bool shouldRender = true;
	public bool rendered = true;
	GameObject mainCameraObject;

	// Use this for initialization
	void Start () {
		//get the camera to check position
		mainCameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		Renderer[] childRenderers = gameObject.GetComponentsInChildren<Renderer> ();
		Renderer currentRenderer = gameObject.GetComponent<Renderer> ();
		if (currentRenderer != null && childRenderers[0] != null) {
			allRenderers = new Renderer[childRenderers.Length + 1];
			childRenderers.CopyTo (allRenderers, 0);
			allRenderers [childRenderers.Length - 1] = currentRenderer;
		} else if(currentRenderer == null) {
			allRenderers = childRenderers;
		}
		else
		{
			allRenderers = new Renderer[1];
			allRenderers[0] = currentRenderer;
		}
		rendered = true;

	}
	
	// Update is called once per frame
	void FixedUpdate () {
		shouldRender = false;
		foreach (Renderer rend in allRenderers) {
			if(rend == null)
				break;
			Bounds thisBounds = rend.bounds;
			Vector3 checkPos = new Vector3(thisBounds.center.x + thisBounds.extents.x,
			                               thisBounds.center.y + thisBounds.extents.y,
			                               thisBounds.center.z);
			Vector2 camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
			checkPos = new Vector3(thisBounds.center.x + thisBounds.extents.x,
			                       thisBounds.center.y - thisBounds.extents.y, thisBounds.center.z);
			camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
			checkPos = new Vector3(thisBounds.center.x - thisBounds.extents.x,
			                       thisBounds.center.y + thisBounds.extents.y, 
			                       thisBounds.center.z);
			camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
			checkPos = new Vector3(thisBounds.center.x - thisBounds.extents.x,
			                       thisBounds.center.y - thisBounds.extents.y, 
			                       thisBounds.center.z);
			camPosition = mainCameraObject.camera.WorldToViewportPoint (checkPos);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				shouldRender = true;
				break;
			}
		}

		if (shouldRender && !rendered) {
			foreach (Renderer rend in allRenderers) {
				if(rend == null)
					break;
				rend.enabled = true;
			}
			rendered = true;
		} else if (!shouldRender && rendered) {
			foreach (Renderer rend in allRenderers) {
				if(rend == null)
					break;
				rend.enabled = false;
			}
			rendered = false;
		}

		
	}
}
