using UnityEngine;
using System.Collections;

public class LevelTransition : MonoBehaviour {
	GameObject player;
	public int nextLevel;
	// Use this for initialization
	void Start () {
		player = GameObject.FindGameObjectWithTag ("Player");
	}
	
	// Update is called once per frame
	void Update () {
		if (player.transform.position.x > this.transform.position.x) {
			Application.LoadLevel(nextLevel);
		}
	}
}
