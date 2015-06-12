using UnityEngine;
using System.Collections;
using UnityEngine.UI;

class FungusEvents : MonoBehaviour {
	public GameObject signPostObject;
	public SignPost signPostScript;
	public GameObject ameliaObject;
	public Player playerScript;
	public GameObject fungusObject;
	public GameObject conversionPrompt;
	GameObject activePrompt;

	bool showYButton = false;

	void Start()
	{}

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

		if (fungusObject == null)
		{
			getFungus();
		}

		// when the player is finished speaking to the fungus,
		// show the prompt to Kill the creature
		if(signPostScript.doneReading && !showYButton)
		{
			showYButton = true;
			activePrompt = Instantiate(conversionPrompt, 
				signPostObject.transform.position, 
				signPostObject.transform.rotation) as GameObject;
			activePrompt.name = "Fungus Kill Prompt";
			fade_in fadeInComponent = activePrompt.GetComponent<fade_in>();
			fadeInComponent.isConversionPrompt = false;
			fadeInComponent.overwrite = false;
			fadeInComponent.destroyOnFadeOut = false;
			GameObject prompt = activePrompt.transform.Find("prompt").gameObject;
			Text textComponent = prompt.GetComponent<Text>();
			textComponent.text = "Kill";

			//prepare the fungus for death
			fungusObject.GetComponent<FungusCreature>().readyToDie = true;

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
		signPostObject = GameObject.Find("FungusSignPost");
		if (signPostObject != null)
		{
			signPostScript = signPostObject.GetComponent<SignPost>();
		}
	}

	private void getFungus()
	{
		fungusObject = GameObject.Find("FungusCreature");
	}
}