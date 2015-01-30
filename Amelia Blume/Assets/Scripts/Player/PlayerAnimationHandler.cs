using UnityEngine;
using System.Collections;

/// <summary>
/// Utility class to feed parameters to an Animator.
/// Some of this should become obsolete with 4.3 when
/// you can explicitly switch to a named state, but
/// until then there are utility methods to flash
/// boolean values long enough to trigger state changes,
/// along with utility methods to temporarily or
/// permanently alter bools and floats.
/// </summary>
public class PlayerAnimationHandler : BaseBehavior {
	
	protected class ToggleFlagArgs {
		public string which;
		public float delay;
		public bool toggleTo;
		
		public ToggleFlagArgs(string which, float delay, bool toggleTo) {
			this.which = which;
			this.delay = delay;
			this.toggleTo = toggleTo;
		}
	}
	protected class BumpFloatArgs {
		public string which;
		public float delay;
		public float bumpValue;
		
		public BumpFloatArgs(string which, float delay, float bumpValue) {
			this.which = which;
			this.delay = delay;
			this.bumpValue = bumpValue;
		}
	}
	
	protected Animator cachedAnimator;
	public Animator animator {
		get {
			if (cachedAnimator == null) {
				cachedAnimator = GetComponentInChildren<Animator>();
			}
			return cachedAnimator;
		}
	}
	
	
	
	public bool isEnabled {
		get {
			return animator.enabled;
		}
		set {
			animator.enabled = value;
		}
	}
	
	
	protected void LateUpdate() {
		SetValue("Speed", player.motor.movement.speed);
		SetValue("XMove", player.motor.lastInput.x);
		SetValue("ZMove", player.motor.lastInput.z);
		
		if (player.controller.isTurning) {
			SetValue("XMove", player.controller.turnDirection);
		}
		
		if (!player.isGrounded && player.motor.environment.altitude > 0.5f) {
			ClearFlag("Grounded");
		} else {
			SetFlag("Grounded");
		}

	}
	
	
	
	
	public void TriggerAnimation(string which) {
		ToggleAnimatorFlag(which);	
	}
	
	public void SetFlag(string which) {
		animator.SetBool(which, true);
	}
	
	public void SetFlagTo(string which, bool value) {
		animator.SetBool(which, value);
	}
	
	public void ClearFlag(string which) {
		animator.SetBool(which, false);	
	}
	
	
	public void GetValue(string which) {
		animator.GetFloat(which);
	}
	
	public void SetValue(string which, float value) {
		animator.SetFloat(which, value);	
	}
	
	
	
	
	public void BumpValue(string which) {
		BumpValue (which, 1f);	
	}
	
	public void BumpValue(string which, float bumpValue) {
		BumpValue (which, 0f, bumpValue);
	}
	
	public void BumpValue(string which, float delay, float bumpValue) {
		StartCoroutine("BumpValueWorker", new BumpFloatArgs(which, delay, bumpValue));
	}

	IEnumerator BumpValueWorker(BumpFloatArgs args) {
		float currentValue = animator.GetFloat(args.which);
		animator.SetFloat(args.which, currentValue + args.bumpValue);
		yield return new WaitForSeconds(args.delay);
		currentValue = animator.GetFloat(args.which);
		animator.SetFloat(args.which, currentValue - args.bumpValue);
	}

	
	
	
	protected void ToggleAnimatorFlag(string which) {
		StartCoroutine("ToggleAnimatorFlagWorker", new ToggleFlagArgs(which, 0f, true));
	}

	protected void ToggleAnimatorFlag(string which, float delay) {
		StartCoroutine("ToggleAnimatorFlagWorker", new ToggleFlagArgs(which, delay, true));
	}
	
	protected void ToggleAnimatorFlag(string which, float delay, bool toggleTo) {
		StartCoroutine("ToggleAnimatorFlagWorker", new ToggleFlagArgs(which, delay, toggleTo));
	}

	
	IEnumerator ToggleAnimatorFlagWorker(ToggleFlagArgs args) {
		animator.SetBool(args.which, args.toggleTo);
		yield return null; 
		yield return new WaitForSeconds(args.delay);
		animator.SetBool(args.which, !args.toggleTo);
	}

		
}
