using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Act_Text : MonoBehaviour {
	GameObject ActObject;
	Text ActText;
	GameObject SubtitleObject;
	Text SubtitleText;

	// Use this for initialization
	void Start () {
		ActObject = GameObject.Find ("Act Card");
		ActText = ActObject.GetComponent<Text> ();
		SubtitleObject = GameObject.Find ("Subtitle");
		SubtitleText = SubtitleObject.GetComponent<Text> ();

		ActText.color = new Color (ActText.color.r, ActText.color.g, ActText.color.g, 0);
		SubtitleText.color = new Color (SubtitleText.color.r, SubtitleText.color.g, SubtitleText.color.b, 0); 

	}
	
	public void FadeInText(string actText, string subtitleText = "")
	{
		SubtitleText.text = subtitleText;
		ActText.text = actText;
		StartCoroutine (FadeInText ());
	}

	IEnumerator FadeInText(){
		float startTime = Time.time;
		while (ActText.color.a < 1) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime) / 0.5f;
			ActText.color = new Color (ActText.color.r, ActText.color.g,
			                           ActText.color.b, Mathf.SmoothStep (0, 1, t));
			SubtitleText.color = new Color (SubtitleText.color.r, SubtitleText.color.g,
			                                SubtitleText.color.b, Mathf.SmoothStep (0, 1, t));
		}
		yield return new WaitForSeconds (3f);
		startTime = Time.time;
		while (ActText.color.a > 0) {
			yield return new WaitForSeconds (0.01f);
			float t = (Time.time - startTime)/1f;
			ActText.color = new Color (ActText.color.r, ActText.color.g,
			                           ActText.color.b, 1 - Mathf.SmoothStep(0,1, t));
			SubtitleText.color = new Color (SubtitleText.color.r, SubtitleText.color.g,
			                                SubtitleText.color.b, 1 - Mathf.SmoothStep(0,1, t));
		}
		SubtitleText.text = "";
		ActText.text = "";
	}
}
