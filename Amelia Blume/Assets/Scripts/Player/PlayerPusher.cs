using UnityEngine;
using System.Collections;

/// <summary>
/// This behavior allows an object with a rigidbody to push
/// a Player around, both by applying a push along the object's
/// velocity multiplied by a customizable push force, and by
/// sending a notification event to the Player.
/// 
/// The Push can be triggered manually by the PlayerPushHandler
/// if it detects it's within range of the object, or automatically
/// if the object detects that its collider overlaps with that
/// of the Player.
/// </summary>
public class PlayerPusher : BaseBehavior {
	
	public float pushForceMultiplier = 1.1f;
	
	public class PushEventArgs {
		public Vector3 pushForce;
		public PlayerPusher pusher;
		public bool overlapped;
		
		public PushEventArgs(Vector3 force, PlayerPusher p, bool overlap) {
			pushForce = force;
			pusher = p;
			overlapped = overlap;
		}
	}
	
	public void PushPlayer(Player p) {
		DoPush(p, false);		
	}
	
	protected void FixedUpdate() {
		if (player.characterController.bounds.Intersects(collider.bounds)) {
			DoPush(player, true);
		}
	}

	protected void DoPush(Player p, bool overlapped) {
		Vector3 force = rigidbody.velocity * pushForceMultiplier * Time.fixedDeltaTime;
		//player.Broadcast("OnPlayerPush", new PushEventArgs(force, this, overlapped));
		p.Push(force);
		
	}
	
}
