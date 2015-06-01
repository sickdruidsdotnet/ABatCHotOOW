using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour {
	GameObject player;
	GameObject gameController;
	public int nextLevel;
	public string nextLevelName;
	public bool rightSide = true;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		gameController = GameObject.FindGameObjectWithTag ("GameController");
	}
	
	// Update is called once per frame
	void Update () {
		if (rightSide && player.transform.position.x > this.transform.position.x) {
			//check to see if highest level needs updating
			if(PlayerPrefs.GetInt("Highest Stage", 0) < nextLevel)
			{
				PlayerPrefs.SetInt("Highest Stage", nextLevel);
			}
			//enter both the level num and name in order to keep it clear, but use the name for transitions
			if(nextLevelName != null){
				gameController.GetComponent<ABGameController>().BeginSceneTransition(nextLevelName);
			}
			else
			{
				gameController.GetComponent<ABGameController>().BeginSceneTransition(nextLevel);
			}
		}

		else if (!rightSide && player.transform.position.x < this.transform.position.x) {
			//check to see if highest level needs updating
			if(PlayerPrefs.GetInt("Highest Stage", 0) < nextLevel)
			{
				PlayerPrefs.SetInt("Highest Stage", nextLevel);
			}
			//enter both the level num and name in order to keep it clear, but use the name for transitions
			if(nextLevelName != null){
				gameController.GetComponent<ABGameController>().BeginSceneTransition(nextLevelName);
			}
			else
			{
				gameController.GetComponent<ABGameController>().BeginSceneTransition(nextLevel);
			}
		}
	}
}
