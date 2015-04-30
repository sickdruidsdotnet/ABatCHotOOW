using UnityEngine;
using System.Collections;

public class disappearer : MonoBehaviour {

	// Use this for initialization
	void Start () {
		// the point of this whole script is to shrink down things so they aren't
		// viewable  in game but durting editing
		transform.localScale = new Vector3(0.000001f, 0.000001f, 0.000001f);
	}
}
