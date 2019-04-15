using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell
{
    [SerializeField]
    private int x, y; // x and y coordinate of object
    private GameObject box;

    public GameObject topWall, botWall, leftWall, rightWall;
    public bool hasRightWall, hasLeftWall, hasTopwall, hasBotWall = true;
    public SpriteRenderer currSR;
    public bool visited = false;
    public bool empty = false;

    public Cell(int i, int j, GameObject currBox)
    {
        x = i;
        y = j;
        box = currBox;
        currSR = box.GetComponent<SpriteRenderer>();
        empty = false;
    }

    public Cell()
    {
        empty = true;
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

}
