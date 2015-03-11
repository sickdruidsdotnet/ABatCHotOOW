using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour {
	private int health;
	private Text healthText;
	Player player;
	private GameObject amelia;
	// Use this for initialization
	void Start () {
		amelia = GameObject.Find ("Player");
		player = amelia.GetComponent<Player> ();
		healthText = GetComponent<Text>();
		health = player.health;
		healthText.text = health + "";
		Debug.Log ("UI:" + health);
	}
	
	// Update is called once per frame
	void Update () {
		health = player.health;
		healthText.text =  "";
	}
}
