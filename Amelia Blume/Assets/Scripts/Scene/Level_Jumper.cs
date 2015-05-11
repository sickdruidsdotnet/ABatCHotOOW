using UnityEngine;
using System.Collections;


public class Level_Jumper : MonoBehaviour {

	public int levelNum;
	public string levelName;
	// Use this for initialization
	void Start () {
	
	}
	
	void OnTriggerStay(Collider other)
	{
		if (other.gameObject.tag == "Player") {
			if(GameObject.Find("Input Handler").GetComponent<InputHandler>().jumpDown)
			{
				if(levelName != null)
					Application.LoadLevel(levelName);
				else
					Application.LoadLevel(levelNum);
			}
		}
	}
}
