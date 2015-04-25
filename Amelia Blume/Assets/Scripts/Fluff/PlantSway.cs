using UnityEngine;
using System.Collections;

public class PlantSway : MonoBehaviour {
	float progress = 0f;
	bool swayRight = true;
	bool inPassingLeft = false;
	bool inPassingRight = false;
	public int swayCount = 0;
	bool hitGoal = false;
	bool moveBack = false;

	GameObject mainCameraObject;
	bool rendered = false;

	float offsetValue;
	
	void Start()
	{
		offsetValue = transform.position.x / 2f;
		mainCameraObject = GameObject.FindGameObjectWithTag ("MainCamera");
		Vector2 camPosition = mainCameraObject.camera.WorldToViewportPoint (transform.position);
		if (camPosition.x < 0 || camPosition.x > 1 || camPosition.y < 0 || camPosition.y > 1) {
			rendered = false;
			renderer.enabled = false;
		}
	}

	// Update is called once per frame
	void FixedUpdate () {
		if (rendered) {
			if (inPassingLeft || inPassingRight) {
				//handle things moving left passed it
				if (inPassingLeft) {
					float tempProgress = (Mathf.Sin (Time.time + offsetValue) * 50f);

					// part 1, get to the leftmost part of animation
					if (!hitGoal) {
						progress -= 5f;
						if (progress <= (Mathf.Cos ((Time.time) * 4) * 12.5f) - 75) {
							hitGoal = true;
							progress = (Mathf.Cos ((Time.time) * 4) * 12.5f) - 75;
						}
					} // Part 2,we've hit the left most frame time to stop
					else if (!moveBack) {
						progress = (Mathf.Cos ((Time.time) * 4) * 12.5f) - 75;
						if (tempProgress <= -47) {
							moveBack = true;
						}
					}// Part 3, return to normal sway
					else if (moveBack) {
						if (progress + 5 > tempProgress) {
							//we've caught up, stop this nonsense and return to normal sway
							progress = tempProgress;
							moveBack = false;
							hitGoal = false;
							inPassingLeft = false;
						} else {
							progress += 1;
						}
					}
				} else {
					//inPassingRight
					float tempProgress = (Mathf.Sin (Time.time + offsetValue) * 50f);
					
					// part 1, get to the leftmost part of animation
					if (!hitGoal) {
						progress += 5f;
						if (progress >= (Mathf.Cos ((Time.time) * 4) * 12.5f) + 75) {
							hitGoal = true;
							progress = (Mathf.Cos ((Time.time) * 4) * 12.5f) + 75;
						}
					} // Part 2,we've hit the left most frame time to stop
					else if (!moveBack) {
						progress = (Mathf.Cos ((Time.time) * 4) * 12.5f) + 75;
						if (tempProgress >= 47) {
							moveBack = true;
						}
					}// Part 3, return to normal sway
					else if (moveBack) {
						if (progress - 5 < tempProgress) {
							//we've caught up, stop this nonsense and return to normal sway
							progress = tempProgress;
							moveBack = false;
							hitGoal = false;
							inPassingRight = false;
						} else {
							progress -= 1f;
						}
					}
				}
			} else {
				progress = (Mathf.Sin (Time.time + offsetValue) * 50f);
			}

			//handles which blendshapes to use for left and right
			if (progress <= 0f) {
				swayRight = false;
			} else {
				swayRight = true;
			}
	
			if (swayRight) {
				this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (1, (float)progress);
			} else {
				this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)progress * -1);
			}

			Vector2 camPosition = mainCameraObject.camera.WorldToViewportPoint (transform.position);
			if (camPosition.x < 0 || camPosition.x > 1 || camPosition.y < 0 || camPosition.y > 1) {
				rendered = false;
				renderer.enabled = false;
			}
		} else {
			Vector2 camPosition = mainCameraObject.camera.WorldToViewportPoint (transform.position);
			if (camPosition.x > 0 && camPosition.x < 1 && camPosition.y > 0 && camPosition.y < 1) {
				rendered = true;
				renderer.enabled = true;
			}
		}

	}

	void OnTriggerStay(Collider other)
	{
		if (other.tag == "Animal") {
			//check if the center has passed
			if (Mathf.Abs (other.transform.position.x - transform.position.x) <= 0.3) {
				hitGoal = false;
				moveBack = false;
				if (other.transform.rigidbody.velocity.x < 0) {
					//Debug.Log ("Animal has passed right!");
					inPassingRight = true;
					inPassingLeft = false;
				} else {
					//Debug.Log ("Animal has passed left!");
					inPassingLeft = true;
					inPassingRight = false;
				}
			}
		} else if (other.tag == "Player") {
			if (Mathf.Abs (other.transform.position.x - transform.position.x) <= 0.3) {
				hitGoal = false;
				moveBack = false;
				//here's where anything involving player passby, like blooming flowers, would happen

				if (other.transform.GetComponent<CharacterController>().velocity.x >= 0) {
					//Debug.Log ("Animal has passed right!");
					inPassingRight = true;
					inPassingLeft = false;
				} else {
					//Debug.Log ("Animal has passed left!");
					inPassingLeft = true;
					inPassingRight = false;
				}
			}
		}
	}

}
