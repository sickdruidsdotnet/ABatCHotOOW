using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Linq;

class HOTW : MonoBehaviour
{
	public GameObject ameliaObject;
	public GameObject fungusObject;
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
			getFungus();
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
				string level;
				if (fungusObject.GetComponent<FungusCreature>().dead)
				{
					level = "Cutscene8-HeartNoFungus";
				}
				else
				{
					level = "Cutscene9-HeartFungus";
				}
                GetComponent<LevelTransition>().nextLevelName = level;
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

	private void getPlayer()
	{
		ameliaObject = GameObject.FindWithTag("Player");
		if (ameliaObject != null)
		{
			playerScript = ameliaObject.GetComponent<Player>();
		}
	}

	private void getFungus()
	{
		fungusObject = GameObject.Find("FungusCreature");
	}
}