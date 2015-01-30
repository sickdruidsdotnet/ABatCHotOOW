using UnityEngine;
using System.Collections;
using System.Linq;

public class Mover : BaseBehavior {
	
	public float arrivalDistance = 0.1f;
	
	private MoverStop[] stops;
	private float velocity;
	private float rotateSpeed = 1f;
	private MoverStop destination;
	private int stopIndex = -1;
	private float currentWait;

	
	void Awake() {
		stops = transform.parent.GetComponentsInChildren<MoverStop>().OrderBy(s => s.order).ToArray();
		stopIndex = -1;
	}

	void Start() {
		UpdateDestination();
	}
	
	void FixedUpdate() {
		float distance;
		
		if (currentWait > 0) {
			currentWait -= Time.deltaTime;	
		} else {
			velocity = Mathf.Lerp(velocity, destination.moveSpeed, Time.deltaTime);
			rotateSpeed = Mathf.Lerp(rotateSpeed, destination.rotateSpeed, Time.deltaTime);
			
			rigidbody.MovePosition(Vector3.MoveTowards(
				transform.position,
				destination.transform.position,
				Time.fixedDeltaTime * velocity
			));

			if (destination.rotate) {
				Quaternion rot = Quaternion.RotateTowards(
					transform.rotation,
					destination.transform.rotation,
					Time.fixedDeltaTime * rotateSpeed
				);
				rigidbody.MoveRotation(rot);
			}
		}
		
		distance = (transform.position - destination.gameObject.transform.position).magnitude;
		
		if (distance < arrivalDistance) {
			if (destination.stopTime > 0) {
				currentWait = destination.stopTime;	
				velocity = 0;
				rotateSpeed = 0;
			}

			UpdateDestination();	
		}
		
	}
	
	private void UpdateDestination() {
		stopIndex = (stopIndex + 1) % stops.Length;
		
		destination = stops[stopIndex];
	}

}
