using UnityEngine;
using System.Collections;

public class Converter : MonoBehaviour {

	public InputHandler playerInput;

	//public float life = 200;
	public float life;
	public float startTime;
	float scaleValue;
	private GameObject player;
	public bool conversionSuccess = false;
	bool exited = false;
	public GameObject target;
	public GameObject parent;
	void Start()
	{
		playerInput = GameObject.Find ("Input Handler").GetComponent<InputHandler> ();
		player = GameObject.Find ("Player");
		scaleValue = transform.localScale.x / 11f;
		life = 3.33333f;
		startTime = Time.time;
	}
	// Update is called once per frame
	void FixedUpdate () {
		HandleInput ();
		if (Time.time - startTime > life) {
			Destroy (gameObject);
		}

		if (player.GetComponent<Player> ().isConverting () || conversionSuccess) {
			if (Time.time - startTime  > 3f && Time.time - startTime < 3.15f) {
				//if (parent == null) {
					Debug.Log ("Null");
					transform.localScale = new Vector3 (transform.localScale.x - scaleValue,
					                                    transform.localScale.y + 8f,
					                                    transform.localScale.z);
				/*} else {
					Debug.Log ("Not Null");
					transform.position = parent.transform.position;
				}*/

			}
			else if(Time.time - startTime > 2.95f && !conversionSuccess && !exited)
			{
				//successful conversion
				conversionSuccess = true;
				if(target != null && target.GetComponent<Animal>().isRestrained){
					target.SendMessage("changeInfection");
				}
				player.GetComponent<Player>().SetConverting(false);
			}
		} else {
			Renderer[] childrays = transform.GetComponentsInChildren<Renderer>();
			foreach( Renderer child in childrays)
			{
				if(child.material.color.a > 0)
				{
					Color newColor;
					if(child.material.color.a - 0.016f <= 0){
						newColor = new Color(0.95f, 0.95f,0.95f, 0f);
					}
					else{
						newColor = new Color(0.95f, 0.95f,0.95f, child.material.color.a - 0.016f);
					}
					child.material.SetColor ("_Color", newColor);
				}
			}
		}
	}

	protected void HandleInput() {
		if (playerInput.sunUp) {
			player.GetComponent<Player>().SetConverting(false);
			life = 1;
			exited = true;
		}
	}

}
