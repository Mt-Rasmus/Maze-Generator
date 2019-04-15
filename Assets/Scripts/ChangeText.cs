using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ChangeText : MonoBehaviour
{
    public GameObject widthTextGO;
    public GameObject heightTextGO;

    // Method to update the UI text element showing maze width
    public void changeWidthText(float inputWidth)
    {
        widthTextGO.GetComponent<Text>().text = inputWidth.ToString();
    }

    // Method to update the UI text element showing maze height
    public void changeHeightText(float inputHeight)
    {
        heightTextGO.GetComponent<Text>().text = inputHeight.ToString();
    }
}
