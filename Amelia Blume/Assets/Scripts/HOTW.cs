using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

class HOTW : MonoBehaviour
{
	public GameObject ameliaObject;
	public Player playerScript;
	private GameObject sun;
	public GameObject faderPrefab;
	private GameObject activeFader;
	public bool startedFade = false;
	public float outTime = 0.15f;
	public float waitTime = 1f;
	public float inTime = 2f;
	public bool readyForAmelia = false; // Super Nintendo, Sega Genesis
	public bool ameliaIsHOTW = false;


	void Start()
	{
		// makse sure we have accss to all our necessary objects
		if (ameliaObject == null || playerScript == null)
		{
			getPlayer();
		}
	}

	void Update()
	{
		if (playerScript.isSunning() && readyForAmelia)
        {
            reactToSunlight();
        }
	}

	void reactToSunlight()
	{
		if (sun == null)
        {
		  sun = GameObject.Find ("Sun");
        }
		if (sun != null) {
			float distance = transform.position.x - sun.transform.position.x;
			if (distance <= 9f && distance >= -9f && !startedFade) {
				startedFade = true;
				/*
				activeFader = Instantiate(faderPrefab, faderPrefab.transform.position, Quaternion.identity) as GameObject;
				activeFader.GetComponent<Image>().color = new Color(.063f, .584f, .153f, 0);
				*/
				Debug.Log("Collecting sun");
                GetComponent<LevelTransition>().transition();
			}
			else
			{
				//Debug.Log("Sun out of range");
			}
		}
        else
        {
            //Debug.Log("Player is sunning, but can't find sun object!");
        }
	}

	/*
	public IEnumerator FadeOut()
	{
		float startTime = Time.time;

		while (activeFader.GetComponent<Image>().color.a < 1) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/outTime;//(timepassed/duration)
			activeFader.GetComponent<Image> ().color = new Color (activeFader.GetComponent<Image> ().color.r, activeFader.GetComponent<Image> ().color.g,
			                                                   activeFader.GetComponent<Image> ().color.b, Mathf.SmoothStep(0,1, t));
		}

		StartCoroutine(FadeIn(waitTime));

        Destroy(GameObject.Find("Become HOTW Prompt"));
	}

	public IEnumerator FadeIn(float delay = 0)
	{
		yield return new WaitForSeconds (delay);
		float startTime = Time.time;
		while (activeFader.GetComponent<Image>().color.a > 0) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/inTime; //(timepassed/duration)
			activeFader.GetComponent<Image> ().color = new Color (activeFader.GetComponent<Image> ().color.r, activeFader.GetComponent<Image> ().color.g,
			                                                   activeFader.GetComponent<Image> ().color.b, 1 - Mathf.SmoothStep(0,1, t));
		}

		ameliaIsHOTW = true;
	}
	*/

	private void getPlayer()
	{
		ameliaObject = GameObject.FindWithTag("Player");
		if (ameliaObject != null)
		{
			playerScript = ameliaObject.GetComponent<Player>();
		}
	}
}