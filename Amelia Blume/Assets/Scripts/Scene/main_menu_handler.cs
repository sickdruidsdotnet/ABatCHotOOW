using UnityEngine;
using System.Collections;

public class main_menu_handler : MonoBehaviour {

	int unlockedNum;
	// Use this for initialization
	void Start () {
		unlockedNum = PlayerPrefs.GetInt ("Highest Stage", 0);
		transform.Translate (5f * unlockedNum, 0, 0);
	}
}
