using UnityEngine;
using System.Collections;

public class Reset_script : MonoBehaviour {

	GameObject gameController;

	void Start()
	{
		if (PlayerPrefs.GetInt ("Highest Stage", 0) == 0) {
			Destroy (gameObject);
		}
		gameController = GameObject.FindGameObjectWithTag ("GameController");
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
				gameController.GetComponent<ABGameController>().BeginSceneTransition("Main Menu");
			}
		}
	}
}
