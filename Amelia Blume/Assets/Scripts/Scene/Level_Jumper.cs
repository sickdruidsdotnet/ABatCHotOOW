using UnityEngine;
using System.Collections;


public class Level_Jumper : MonoBehaviour {

	public int levelNum;
	public string levelName;

	GameObject gameController;
	// Use this for initialization
	void Start () {
		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			if(GameObject.Find("Input Handler").GetComponent<InputHandler>().jumpDown)
			{
				if(levelName != null)
					gameController.GetComponent<ABGameController>().BeginSceneTransition(levelName);
				else
					gameController.GetComponent<ABGameController>().BeginSceneTransition(levelNum);
			}
		}
	}
}
