using UnityEngine;
using System.Collections;

public class PlayerTestSceneLogic : BaseBehavior {
	
	public Texture2D textBackdrop;
	public GUIStyle textStyle;
	
	protected void OnGUI() {
		Rect pos;
		
		pos = new Rect(5, Screen.height - 80, 250, 70);		
		GUI.DrawTexture(pos, textBackdrop);
			
		pos = new Rect(20, Screen.height - 30, 200, 20);
		GUI.Label(pos, "Mouse wheel to zoom", textStyle);
		pos.y -= 15;
		GUI.Label(pos, "Right click and drag to move camera", textStyle);
		pos.y -= 15;
		GUI.Label(pos, "Space to jump", textStyle);
		pos.y -= 15;
		GUI.Label(pos, "WASD to move, Shift to run", textStyle);
	}

}
