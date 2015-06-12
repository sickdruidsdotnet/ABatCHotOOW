using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.UI;

class MushroomSprout : MonoBehaviour {
	public float yIncrease = 0.1f;
	public float rotation = 0f;
	public float scale = 1f;
	public float sproutSeconds = 1f;
	public float delaySeconds = 0f;

	float startTime;
	Vector3 startPos;
	Quaternion startRot;
	Vector3 startScale;
	bool sprouting = false;
	bool sprouted = false;

	void Update()
	{

		if (startPos == new Vector3(startPos.x, startPos.y + yIncrease, startPos.z))
		{
			sprouting = false;
			sprouted = true;
		}
		if (sprouting)
		{

			float percent = (Time.time - startTime) / sproutSeconds;
			if (sproutSeconds == null || sproutSeconds <= 0)
			{
				Debug.Log("spoutSeconds must be greater than zero.");
				Debug.Break();
			}
			// lerp position
			if (yIncrease != 0)
			{
				if (yIncrease < 0)
				{
					Debug.Log("Mushroom is moving down... it this intended?");
				}
				if(startPos == null)
				{
					Debug.Log("startPos is null.");
					Debug.Break();
				}

				transform.position = Vector3.Lerp(startPos, new Vector3(startPos.x, startPos.y + yIncrease, startPos.z), percent);
			}

			if (rotation != 0)
			{
				if(startRot == null)
				{
					Debug.Log("startRot is null.");
					Debug.Break();
				}

				transform.rotation = Quaternion.Lerp(startRot, Quaternion.AngleAxis(rotation, Vector3.up), percent);
			}

			if (scale > 1)
			{
				if(startScale == null)
				{
					Debug.Log("startScale is null.");
					Debug.Break();
				}

				transform.localScale = Vector3.Lerp(startScale, new Vector3(startScale.x * scale, startScale.y * scale, startScale.z * scale), percent);
			}

		}
	}

	public void sprout()
	{
		StartCoroutine(delayedSprout(delaySeconds));
	}

	public IEnumerator delayedSprout(float s)
	{
		yield return new WaitForSeconds (s);

		if (sprouting)
		{
			Debug.Log("Called sprout() when mushroom is currently sprouting!");
			Debug.Break();
		}
		if (sprouted)
		{
			Debug.Log("Called sprout() when mushroom is already done sprouting!");
			Debug.Break();
		}

		startTime = Time.time;
		startPos = transform.position;
		startRot = transform.rotation;
		startScale = transform.localScale;
		sprouting = true;
	}
}