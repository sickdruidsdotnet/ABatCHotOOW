using UnityEngine;
using System.Collections;

public class Ground_Recolorer : MonoBehaviour {

	public bool overwriteColors;
	public Color grassColor;
	public Color grassFluffColor;

	// Use this for initialization
	void Start () {
		if (overwriteColors) {
			GameObject[] allGround = GameObject.FindGameObjectsWithTag ("Ground");
			GameObject[] allSoil = GameObject.FindGameObjectsWithTag ("Soil");

			foreach (GameObject ground in allGround) {
				//first recolor the fluff
				Fluff_spawner[] fluff = ground.GetComponentsInChildren<Fluff_spawner>();
				int i = 0;
				foreach(Fluff_spawner spawner in fluff)
				{
					fluff[i].overrideColor = true;
					fluff[i].newColor = grassFluffColor;
					//fluff[i].respawnFluff();
					i++;
				}
				//now find the ground and change the grass color
				MeshRenderer render = ground.transform.Find("Ground").GetComponent<MeshRenderer>();
				render.materials[1].color = grassColor;

			}

			foreach (GameObject soil in allSoil) {
				//first recolor the fluff
				Fluff_spawner[] fluff = soil.GetComponentsInChildren<Fluff_spawner>();
				int i = 0;
				foreach(Fluff_spawner spawner in fluff)
				{
					fluff[i].overrideColor = true;
					fluff[i].newColor = grassFluffColor;
					//fluff[i].respawnFluff();
					i++;
				}
				
			}

			//now check for all lingering fluff
			GameObject[] grassFluff = GameObject.FindGameObjectsWithTag("Grass Fluff");
			foreach(GameObject grass in grassFluff)
			{
				grass.renderer.material.color = grassFluffColor;
			}
		}
	}

}
