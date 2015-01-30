using UnityEngine;
using System.Collections;

/// <summary>
/// Extension of MonoBehaviour for all components.
/// Any functionality common enough to be needed
/// throughout the project can be added here.
/// 
/// This version only has one addition, a cached
/// lookup property for the Player to allow all
/// components to easily interact with the Player.
/// </summary>
public class BaseBehavior : MonoBehaviour {
	
	
	private static Player cachedPlayer;
	public Player player {
		get {
			if (cachedPlayer == null) {
				cachedPlayer = GameObject.Find("Player").GetComponent<Player>();
			}
			return cachedPlayer;
		}
	}
	
	
	
}
