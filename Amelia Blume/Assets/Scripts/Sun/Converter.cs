using UnityEngine;
using System.Collections;

public class Converter : MonoBehaviour {

	public float life = 320;
	float scaleValue;
	void Start()
	{
		scaleValue = transform.localScale.x / 11f;
	}
	// Update is called once per frame
	void Update () {
		life--;
		if (life < 0) {
			Destroy (this.gameObject);
		}else if (life < 20 && life > 10) {
			transform.localScale = new Vector3(transform.localScale.x - scaleValue,
			                        transform.localScale.y + 8f,
			                        transform.localScale.z);
		}
	}
}
