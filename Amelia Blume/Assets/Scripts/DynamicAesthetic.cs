using UnityEngine;
using System.Collections;

//This is the base class which is
//also known as the Parent class.
public class DynamicAesthetic : MonoBehaviour
{
	public Color color1 = Color.red;
	public Color color2 = Color.blue;
	public Color color3 = Color.green;
	public Color color4 = Color.yellow;
	public Color color5 = Color.white;

	public Color activeColor;

	private MeshRenderer meshRenderer;

	public int act;

	public DynamicAesthetic()
	{
	}

	void Start()
	{
		meshRenderer = GetComponent<MeshRenderer>();
		act = GameObject.FindWithTag("LevelAesthetic").GetComponent<LevelAesthetic>().act;

		switch (act)
        {
        case 1:
            activeColor = color1;
            break;
        case 2:
            activeColor = color2;
            break;
        case 3:
            activeColor = color3;
            break;
        case 4:
            activeColor = color4;
            break;
        case 5:
            activeColor = color5;
            break;
        default:
            activeColor = color1;
            break;
        }
		meshRenderer.material.color = activeColor;
	}
}