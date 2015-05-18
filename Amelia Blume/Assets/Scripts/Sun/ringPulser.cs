using UnityEngine;
using System.Collections;

public class ringPulser : MonoBehaviour {
	public float offset = 0;
	float progress = 3;
	float transparency = 1f;

	void FixedUpdate () {
		if (offset > 0){
			offset-= Time.deltaTime;
			if(offset == 0)	{
				Color newColor = new Color(renderer.material.color.r, 
				                           renderer.material.color.g,
				                           renderer.material.color.b,
				                           1f);
				renderer.material.SetColor("_Color", newColor);
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

		if (progress >= 3 ) {
			progress = 0;
			transparency = 1f;
			Color newColor = new Color(renderer.material.color.r, 
			                           renderer.material.color.g,
			                           renderer.material.color.b,
			                           transparency);
			renderer.material.SetColor("_Color", newColor);
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)(progress * 60f));
			//transform.localScale = startScale;
		} else {
			progress += Time.deltaTime;
			this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)(progress * 60f));
			/*tempScale = new Vector3(transform.localScale.x + 0.1f, 
                                   transform.localScale.y + 0.1f, 
                                   transform.localScale.z + 0.1f);
			transform.localScale = tempScale;*/
			if(progress >=0.83f)
			{
				transparency -= (4.8f * Time.deltaTime);
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
