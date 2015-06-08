using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class BecomeHOTW : MonoBehaviour {
	public GameObject ameliaObject;
	public Player playerScript;
	public GameObject signPostObject;
	public SignPost signPostScript;
	public GameObject heartObject;
	public GameObject conversionPrompt;
	GameObject activePrompt;

	bool showYButton = false;

	void Start()
	{
		getPlayer();
		getSign();
		getHeart();
	}

	void Update()
	{
		// makse sure we have accss to all our necessary objects
		if (ameliaObject == null || playerScript == null)
		{
			getPlayer();
		}

		if (signPostObject == null || signPostScript == null)
		{
			getSign();
		}

		if (heartObject == null)
		{
			getHeart();
		}

		if(signPostScript.doneReading && !showYButton)
		{
			showYButton = true;
			activePrompt = Instantiate(conversionPrompt, 
				signPostObject.transform.position, 
				signPostObject.transform.rotation) as GameObject;
			activePrompt.name = "Become HOTW Prompt";
			fade_in fadeInComponent = activePrompt.GetComponent<fade_in>();
			fadeInComponent.isConversionPrompt = false;
			fadeInComponent.overwrite = false;
			fadeInComponent.destroyOnFadeOut = false;
			GameObject prompt = activePrompt.transform.Find("prompt").gameObject;
			Text textComponent = prompt.GetComponent<Text>();
			textComponent.text = "Become HOTW";

			//prepare the fungus for death
			heartObject.GetComponent<HOTW>().readyForAmelia = true;

			// get rid of the dialogue prompt...
			// well, at least make it inaccessible to the player.
			signPostObject.transform.position = new Vector3 (signPostObject.transform.position.x, 
				signPostObject.transform.position.y, 
				signPostObject.transform.position.z + 3);
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

	private void getSign()
	{
		signPostObject = GameObject.Find("HOTWSignPost");
		if (signPostObject != null)
		{
			signPostScript = signPostObject.GetComponent<SignPost>();
		}
	}

	
	private void getHeart()
	{
		heartObject = GameObject.Find("hotw_dead");
	}
}