using UnityEngine;
using System.Collections;

public class BoarEventsPart2 : MonoBehaviour {

	/*SYNOPSIS
	 * ~ Amelia Walks left and enters triggerbox
	 * 1. Rumbling starts, but only for a short moment.
	 * 2. text appears telling the player they only have a short time to prepare
	 * ~ Rumbling increases over the next 10-15 seconds
	 * 3. Boar Spawns
	 * 4. Boar is purified, Invisible wall is removed. 
	 * TODO:
	 * ~ It'd be neat if we had it walk over to a tree and knock it over to provide a way
	 * 		Forward instead, but we're out of Time
	 * - End cutscene
	 */

	Player player;
	SignPost sign;

	public GameObject invisibleWall;
	Vector3 invisPos;
	public GameObject panLimiter;
	Vector3 panPos;

	GameObject activeBoar;

	Boar_Spawner BoarSpawner;

	SideScrollerCameraController mainCamera;

	public bool E1Started = false;
	public bool E1Done = false;

	public bool E2Started = false;
	public bool E2Done = false;

	public bool E3Started = false;
	public bool E3Done = false;

	public bool rumbling = false;
	float rumbleStart;
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
		//Event 1's timing
		if (E1Started && !E1Done && !rumbling) {
			E1Done = true;
			Event2 ();
		} else if (E2Started && !E2Done) {
			//event2 is underway, we need to check if the player has finished with the text
			if (!sign.beingRead) {
				Debug.Log ("E2 Done");
				player.SetReadSign(false);
				E2Done = true;
				player.gameObject.GetComponent<PlayerController> ().canControl = true;
				StartCoroutine (RumbleInterval (0.15f, 0.56f, 5));
				rumbleStart = Time.time;
			}
		} else if (E2Done && !E3Started) {
			//change rumbling intervals to make it sound like it's getting closer
			if (Time.time - rumbleStart > 15) {
				Event3 ();
			} else if (Time.time - rumbleStart > 10 && !rumbling) {
				Debug.Log ("Stage3");
				RumbleInterval (0.15f, 0.2f, 5);
			} else if (Time.time - rumbleStart > 5 && !rumbling) {
				Debug.Log ("Stage2");
				RumbleInterval (0.15f, 0.2f, 5);
			}
		} else if (E3Done && !activeBoar.GetComponent<Animal> ().isInfected) {
			//Player has purified the boar! time to get rid of the fake progress blocks
			Event4();
		}
	
	}

	public void Event1()
	{
		E1Started = true;
		//lock the player because it's a cutscene
		player.gameObject.GetComponent<PlayerController> ().canControl = false;
		StartCoroutine(RumbleInterval (0.15f, 0.7f, 3));
	}

	public void Event2(){
		E2Started = true;
		player.gameObject.GetComponent<PlayerController> ().canControl = true;
		sign.CutsceneStart ("3-4 Time to Prepare");
	}

	public void Event3(){
		E3Started = true;
		Debug.Log ("Spawning Boar");
		activeBoar = BoarSpawner.SpawnBoar();
		E3Done = true;
	}

	public void Event4(){
		invisibleWall.transform.position = new Vector3 (invisPos.x, invisPos.y, -10f);
		panLimiter.transform.position = new Vector3 (-20f, panPos.y, panPos.z);
	}

	public void Reset()
	{
		invisibleWall.transform.position = invisPos;
		panLimiter.transform.position = panPos;
		E1Started = false;
		E1Done = false;
		E2Started = false;
		E2Done = false;
		E3Started = false;
		E3Done = false;
		player.SetReadSign(false);
	}

	IEnumerator RumbleInterval(float rumbleTime, float bwRumbles, float duration)
	{
		rumbling = true;
		float startTime = Time.time;
		while (Time.time - startTime < duration) {
			mainCamera.Rumble(rumbleTime);
			yield return new WaitForSeconds(rumbleTime + bwRumbles);
		}
		rumbling = false;
	}

	void OnTriggerEnter(Collider other){
		if (other.tag == "Player") {
			if (!E1Started) {
				Event1 ();
			}
		}
	}

}
