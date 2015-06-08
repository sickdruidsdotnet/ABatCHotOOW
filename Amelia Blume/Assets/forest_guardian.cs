using UnityEngine;
using System.Collections;

public class forest_guardian : MonoBehaviour {
	float progress = 0f;
	bool swayRight = true;
	bool inPassingLeft = false;
	bool inPassingRight = false;
	public int swayCount = 0;
	bool hitGoal = false;
	bool moveBack = false;

	public SignPost guardianSP;

	public bool stopping = false;

	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		if (guardianSP.currentPassage == "2-1 Forest Guardian e")
			stopping = true;
		if (stopping) {
			if(progress > -100)
			{
				progress-=0.75f;
			}
			if(progress > 0){
				this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (1, (float)progress);
			} else {
				this.GetComponent<SkinnedMeshRenderer> ().SetBlendShapeWeight (0, (float)progress * -1);
			}
		} else {
			if (inPassingLeft || inPassingRight) {
				//handle things moving left passed it
				if (inPassingLeft) {
					float tempProgress = (Mathf.Sin (Time.time) * 50f);
				
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
					float tempProgress = (Mathf.Sin (Time.time) * 50f);
				
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
				progress = (Mathf.Sin (Time.time) * 50f);
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
		}
	}
}
