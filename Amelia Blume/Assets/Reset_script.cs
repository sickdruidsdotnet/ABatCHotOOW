using UnityEngine;
using System.Collections;

public class Reset_script : MonoBehaviour {

	void Start()
	{
		if (PlayerPrefs.GetInt ("Highest Stage", 0) == 0) {
			Destroy (gameObject);
		}
	}

	// Update is called once per frame
	void Update () {
	
	}

	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			if(GameObject.Find("Input Handler").GetComponent<InputHandler>().jumpDown)
			{
				PlayerPrefs.SetInt("Highest Stage", 0);
				//reload level
				Application.LoadLevel(Application.loadedLevel);
			}
		}
	}
}
