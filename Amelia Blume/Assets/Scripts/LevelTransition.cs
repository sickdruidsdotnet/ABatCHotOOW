using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour {
	GameObject player;
	GameObject gameController;
	public int nextLevel;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x > this.transform.position.x) {
			//check to see if highest level needs updating
			if(PlayerPrefs.GetInt("Highest Stage", 0) < nextLevel)
			{
				PlayerPrefs.SetInt("Highest Stage", nextLevel);
			}
			gameController.GetComponent<ABGameController>().BeginSceneTransition(nextLevel);
			//Application.LoadLevel(nextLevel);
		}
	}
}
