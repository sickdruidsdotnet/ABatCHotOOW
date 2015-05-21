using UnityEngine;
using System.Collections;

public class BoarEventsPart1 : MonoBehaviour {

	/*SYNOPSIS
	 * ~ Amelia interacts with mushroom/ ig vines and triggers a signpost
	 * 1. At the end of signpost text, rumbling starts, triggers more text
	 * 2. At the end of that text, boar sound is played, and boar charges on screen
	 * ~. Boar gets caught in vines and immediately breaks out
	 * 3. Breaking out removes the pan Limiter + invisible wall
	 * - End cutscene
	 */

	Player player;

	public GameObject invisibleWall;
	Vector3 invisPos;
	public GameObject panLimiter;
	Vector3 panPos;

	GameObject activeBoar;

	Boar_Spawner BoarSpawner;

	SideScrollerCameraController mainCamera;

	bool hasBeenRestrained = false;
	bool waitingForBreakout = false;

	// Use this for initialization
	void Start () {
		GameObject playerObject = GameObject.FindWithTag ("Player");
		if (playerObject == null) {
			Debug.LogError("Boar Event Error: cannot locate player");
			//If you're seeing this error, you may have tagged the player incorrectly
		}
		else{
			player = playerObject.GetComponent <Player>();
		}
		GameObject BoarSpawnerObject = GameObject.Find ("Boar Spawner");
		if (BoarSpawnerObject != null) {
			BoarSpawner = BoarSpawnerObject.GetComponent<Boar_Spawner>();
		}
		GameObject cameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		if (cameraObject != null) {
			mainCamera = cameraObject.GetComponent<SideScrollerCameraController>();
		}
		invisPos = invisibleWall.transform.position;
		panPos = panLimiter.transform.position;
	}

	void FixedUpdate()
	{
		if (waitingForBreakout) {
			if(!hasBeenRestrained){
				if(activeBoar.GetComponent<Boar>().isRestrained)
				   hasBeenRestrained = true;
			} else{
				if(!activeBoar.GetComponent<Boar>().isRestrained)
					Event3();
			}
		}
	}

	public void Event1()
	{
		//lock the player because it's a cutscene
		player.gameObject.GetComponent<PlayerController> ().canControl = false;
		//rumble camera
		if (mainCamera != null) {
			//mainCamera.Rumble(2f);
		}
		WaitForABit (2f);
	}

	public void Event2(){
		activeBoar = BoarSpawner.SpawnBoar();
		waitingForBreakout = true;
	}

	public void Event3(){
		//unlock the player because they need to run for their life
		player.gameObject.GetComponent<PlayerController> ().canControl = true;
		//move the wall/limiter to make it easier to reset later
		invisibleWall.transform.position = new Vector3 (invisPos.x, invisPos.y, -10f);
		panLimiter.transform.position = new Vector3 (-20f, panPos.y, panPos.z);

	}

	public void Reset()
	{
		waitingForBreakout = false;
		hasBeenRestrained = false;
		invisibleWall.transform.position = invisPos;
		panLimiter.transform.position = panPos;
	}

	IEnumerator WaitForABit(float waitTime = 2f)
	{
		yield return new WaitForSeconds(waitTime);
		//trigger next set of text

	}

}
