using UnityEngine;
using System.Collections;

public class ringPulser : MonoBehaviour {
	public int offset = 0;
	Vector3 startScale;
	float progress = 120;
	float transparency = 1f;
	// Use this for initialization
	void Start () {
		startScale = transform.localScale;
	}
	
	// Update is called once per frame
	void Update () {
		if (offset > 0){
			offset--;
			if(offset == 0)	{
				Color newColor = new Color(renderer.material.color.r, 
				                           renderer.material.color.g,
				                           renderer.material.color.b,
				                           1f);
			}
			else{
				Color newColor = new Color(renderer.material.color.r, 
				                           renderer.material.color.g,
				                           renderer.material.color.b,
				                           0f);
				renderer.material.SetColor("_Color", newColor);
			}
			return;
		}

		if (progress >=120 ) {
			progress = 0;
			transparency = 1f;
			Color newColor = new Color(renderer.material.color.r, 
			                           renderer.material.color.g,
			                           renderer.material.color.b,
			                           transparency);
			renderer.material.SetColor("_Color", newColor);
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)progress);
			//transform.localScale = startScale;
		} else {
			progress += 1f;
			Vector3 tempScale = transform.localScale;
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)progress);
			/*tempScale = new Vector3(transform.localScale.x + 0.1f, 
                                   transform.localScale.y + 0.1f, 
                                   transform.localScale.z + 0.1f);
			transform.localScale = tempScale;*/
			if(progress >=50)
			{
				transparency -=0.08f;
				if(transparency < 0)
					transparency = 0;
				Color newColor = new Color(renderer.material.color.r, 
				                           renderer.material.color.g,
				                           renderer.material.color.b,
				                           transparency);
				renderer.material.SetColor("_Color", newColor);

			}
		}
		transform.Rotate(new Vector3(0f, 4f, 0f));
	}
}
