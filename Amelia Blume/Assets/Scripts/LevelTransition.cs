using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour {
	GameObject player;
	GameObject gameController;
	public int nextLevel;
	public string nextLevelName;
	public bool right = true;
	public bool left = false;
	public bool up = false;
	public bool down = false;

	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
		gameController = GameObject.FindGameObjectWithTag ("GameController");
		if (left && right)
		{
			Debug.Log("LevelTransition triggers to left AND right. Should be one or the other!");
		}
		if (up && down)
		{
			Debug.Log("LevelTransition triggers to above AND below. Should be one or the other!");
		}
		if (!left && !right && !up && !down)
		{
			Debug.Log("LevelTransition will never trigger because none of the directions are true!");
		}
	}
	
	// Update is called once per frame
	void Update () {
		if ((right && player.transform.position.x > this.transform.position.x)
			|| (left && player.transform.position.x < this.transform.position.x)
			|| (up && player.transform.position.y > this.transform.position.y)
			|| (down && player.transform.position.y < this.transform.position.y)) 
		{
			transition();			
		}
	}

	public void transition()
	{
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
