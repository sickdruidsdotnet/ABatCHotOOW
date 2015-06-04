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
	SignPost sign;
	public SignPost mushSign;

	public GameObject vinePrefab;

	public GameObject invisibleWall;
	Vector3 invisPos;
	public GameObject panLimiter;
	Vector3 panPos;

	GameObject activeBoar;

	Boar_Spawner BoarSpawner;

	SideScrollerCameraController mainCamera;

	bool hasReadMush = false;

	bool hasBeenRestrained = false;
	bool waitingForBreakout = false;

	public bool E1Started = false;
	public bool E1Done = false;

	public bool E2Started = false;
	public bool E2Done = false;

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
		sign = gameObject.GetComponent<SignPost>();
	}

	/*void Update(){
		if (Input.GetKeyDown (KeyCode.T)) {
			GameObject.Find ("Sign_Post").GetComponent<SignPost>().Read("tutorial dash");
		}
	}*/

	void FixedUpdate()
	{
		//if/else madness!!!
		//Pre Event 1
		if (!E1Started) {
			if (!hasReadMush) {
				if (!mushSign.doneReading)
					hasReadMush = true;
			} else {
				if (mushSign.doneReading) {
					Event1 ();
					E1Started = true;
				}
			}
		}

		if (E1Done && !E2Started) {
			if(!sign.beingRead)
			{
				player.SetReadSign(false);
				E2Started = true;
				Event2();
			}
		}

		//Post E2, Pre E3
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
		//move the original signpost back because it's being annoying
		mushSign.transform.position = new Vector3 (mushSign.transform.position.x, mushSign.transform.position.y, mushSign.transform.position.z + 3);
		//rumble camera
		if (mainCamera != null) {
			mainCamera.Rumble(2f);
		}
		StartCoroutine(WaitForABit (2f));
	}

	public void Event2(){
		mainCamera.Rumble(0.15f);
		activeBoar = BoarSpawner.SpawnBoar();
		waitingForBreakout = true;
		E2Done = true;
	}

	public void Event3(){
		//unlock the player because they need to run for their life
		player.gameObject.GetComponent<PlayerController> ().canControl = true;
		//move the wall/limiter to make it easier to reset later
		invisibleWall.transform.position = new Vector3 (invisPos.x, invisPos.y, -10f);
		panLimiter.transform.position = new Vector3 (-20f, panPos.y, panPos.z);
		//exit the cutscene
	}

	public void Reset()
	{
		invisibleWall.transform.position = invisPos;
		panLimiter.transform.position = panPos;
		mushSign.transform.position = new Vector3 (mushSign.transform.position.x, mushSign.transform.position.y, mushSign.transform.position.z - 3);
		hasReadMush = false;
		hasBeenRestrained = false;
		waitingForBreakout = false;
		E1Started = false;
		E1Done = false;
		E2Started = false;
		E2Done = false;
		player.SetReadSign(false);

		//respawn a vine because the first one broke
		GameObject newVine = Instantiate (vinePrefab, new Vector3 (23.93f, -1.93f, 0), vinePrefab.transform.rotation) as GameObject;
		newVine.GetComponent<VinePlant> ().waterCount = 100;
	}

	IEnumerator WaitForABit(float waitTime = 2f)
	{
		yield return new WaitForSeconds(waitTime);
		player.gameObject.GetComponent<PlayerController> ().canControl = true;
		sign.CutsceneStart ("3-2 What Was That");
		E1Done = true;

	}

}
