using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdjustment : MonoBehaviour
{
    public SpriteRenderer square;
    public int width;
    public int height;
    private float orthoSize;

    public bool isOdd(int nr)
    {
        if (nr % 2 != 0)
            return true;
        else
            return false;
    }

    // Method to change size of orthographic camera based on the dimensions of the newly create maze
    public void AdjustCamera()
    {
        if ((isOdd(width) && width - height >= height + 1) || (!isOdd(width) && width - height > height))
        {
            orthoSize = width * square.bounds.size.x * Screen.height / Screen.width * 0.5f;
        }
        else
        {
            orthoSize = (height * square.bounds.size.x) * 0.5f;
        }

        Camera.main.orthographicSize = orthoSize * (orthoSize / (orthoSize - 0.03775f));
    }

    // Method to change maze width, gets input from GUI slider
    public void changeWidth(float inputWidth)
    {
        width = (int)inputWidth;
    }

    // Method to change maze height, gets input from GUI slider
    public void changeHeight(float inputHeight)
    {
        height = (int)inputHeight;
    }

}
