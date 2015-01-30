using UnityEngine;
using System.Collections;
using System.Collections.Generic;


/// <summary>
/// Handles interactions with PlayerPusher objects.
/// If a Player touches a collider with a PlayerPusher
/// and a Rigidbody, PlayerPusher will attempt to move
/// the player with the same velocity as the object.
/// </summary>
public class PlayerPushHandler : BaseBehavior {
	
	protected LayerMask pusherCheckMask;
	
	public List<PlayerPusher> pushers;
	
	protected void Awake() {
		pushers = new List<PlayerPusher>();	
		pusherCheckMask = ~(1 << LayerMask.NameToLayer("Player"));
	}
	
	protected void FixedUpdate() {
		FindNearbyPushers();
		
		ApplyPushes();
	}
	
	protected void ApplyPushes() {
		for (int i = pushers.Count - 1; i > -1; i--) {
			pushers[i].PushPlayer(player);
		}
		pushers.Clear();
	}
	
	protected void OnTriggerEnter(Collider c) {
		PlayerPusher pusher = c.gameObject.GetComponent<PlayerPusher>();
		
		if (pusher != null && !pushers.Contains(pusher)) {
			pushers.Add(pusher);	
		}
	}
	protected void OnTriggerExit(Collider c) {
		PlayerPusher pusher = c.gameObject.GetComponent<PlayerPusher>();
		
		if (pusher != null) {
			pushers.Remove(pusher);	
		}
	}
	
	protected void FindNearbyPushers() {
		PlayerPusher pusher;
		RaycastHit[] hits;
		float checkRadius = player.characterController.radius * 1.2f;
		float checkHeight = player.characterController.height * 1.2f;
		
		Vector3 point1 = transform.position + Vector3.up * checkRadius;
		Vector3 point2 = transform.position + Vector3.up * (checkHeight - checkRadius);
		
		hits = Physics.CapsuleCastAll(point1, point2, player.characterController.radius, transform.forward, 0.1f, pusherCheckMask);
		
		if (hits.Length > 0) {
			for (int i = hits.Length - 1; i > -1; i--) {
				pusher = hits[i].collider.gameObject.GetComponent<PlayerPusher>();
				if (pusher != null) {
					if (!pushers.Contains(pusher)) {
						pushers.Add(pusher);
					}
				}
			}
		}
	}
	
}
