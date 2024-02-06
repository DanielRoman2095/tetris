using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeColorButton : MonoBehaviour
{
    [SerializeField]
    private Button button;
    [SerializeField]
    private Image[] images;
    [SerializeField]
    private Color color;

	public void ChangeColor()
	{

        button.enabled = true;

        Debug.Log("Changing highlighed color");
		ColorBlock colorVar = button.colors;
        colorVar.highlightedColor = color; //new Color(color.r, color.g, color.b);
        colorVar.normalColor = color;
        colorVar.pressedColor = color;
        colorVar.selectedColor = color;
		button.colors = colorVar;

		foreach(Image image in images)
        {
            image.color = color;
        }
	}


}
