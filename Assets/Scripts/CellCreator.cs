using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CellCreator : MonoBehaviour
{
    [SerializeField]
    private GameObject blockHolder, wallHolder, blockPrefab, wall_h, wall_v, cam;

    private int width = 4, height = 4;

    public Cell[] cells;

    public void generateMaze()
    {
        // Destroying old maze before creating a new one, if there exists one
        if (cells != null && cells.Length > 0)
        {
            GameObject[] oldCells = GameObject.FindGameObjectsWithTag("cell");
            foreach (GameObject cell in oldCells)
                Destroy(cell);

            GameObject[] oldWalls = GameObject.FindGameObjectsWithTag("wall");
            foreach (GameObject wall in oldWalls)
                Destroy(wall);
        }

        cells = new Cell[width * height];
        blockHolder.transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y);
        wallHolder.transform.position = new Vector2(cam.transform.position.x, cam.transform.position.y);

        int addWidth = 0;

        // Nested loop to create maze start "grid" with no walls missing
        for (int j = 0; j < height; j++)
        {
            for (int i = 0; i < width; i++)
            {
                float w = blockPrefab.GetComponent<Renderer>().bounds.size.x;
                GameObject currBox = Instantiate(blockPrefab, new Vector3(i * w, j * w, 0), Quaternion.identity);
                currBox.transform.parent = blockHolder.transform;
                currBox.tag = "cell";

                // Instantiating walls for the current cell
                GameObject leftwall = Instantiate(wall_v, new Vector3(currBox.transform.position.x - w / 2, currBox.transform.position.y, -0.01f), Quaternion.identity);
                GameObject topwall = Instantiate(wall_h, new Vector3(currBox.transform.position.x, currBox.transform.position.y + w / 2, -0.01f), Quaternion.identity);
                GameObject rightwall = Instantiate(wall_v, new Vector3(currBox.transform.position.x + w / 2, currBox.transform.position.y, -0.01f), Quaternion.identity);
                GameObject botwall = Instantiate(wall_h, new Vector3(currBox.transform.position.x, currBox.transform.position.y - w / 2, -0.01f), Quaternion.identity);

                leftwall.tag = "wall"; topwall.tag = "wall"; rightwall.tag = "wall"; botwall.tag = "wall";

                cells[i + addWidth] = new Cell(i, j, currBox);

                cells[i + addWidth].leftWall = leftwall;
                cells[i + addWidth].topWall = topwall;
                cells[i + addWidth].rightWall = rightwall;
                cells[i + addWidth].botWall = botwall;

                cells[i + addWidth].leftWall.transform.parent = wallHolder.transform;
                cells[i + addWidth].topWall.transform.parent = wallHolder.transform;
                cells[i + addWidth].rightWall.transform.parent = wallHolder.transform;
                cells[i + addWidth].botWall.transform.parent = wallHolder.transform;

            }
            addWidth += width;
        }

        blockHolder.transform.position = getCenter();
        wallHolder.transform.position = blockHolder.transform.position;

        // Main algorithm (Depth First Search + Recursive Backtracker)
        // Starts off in the bottom left, traveling to neighboring cells randomly, removing walls,
        // until no more direct neighbors are "unvisited".
        // It then backtracks using a stack, to a previous cell with univisited neighbors,
        // repeating the process, until all cells have been visited.

        Cell current = cells[0];
        current.visited = true;
        current.currSR.material.color = Color.blue;
        Stack<Cell> cellStack = new Stack<Cell>();

        Cell next = lookupNeighbors(current.getX(), current.getY()); // returns one random unvisited neighboring cell, or an empty one

        while (!next.empty)
        {
            next.visited = true;
            cellStack.Push(current);
            removeWalls(current, next);
            current = next;
            current.currSR.material.color = Color.blue;
            next = lookupNeighbors(current.getX(), current.getY()); // returns one random unvisited neighboring cell, or an empty one

            // Backtracking
            while (next.empty && cellStack.Count > 0)
            {
                current = cellStack.Pop();
                next = lookupNeighbors(current.getX(), current.getY());
            }
        }
    }

    // Helper method to find neighbor index
    private int getNeighborIndex(int i, int j)
    {
        if (i < 0 || j < 0 || i > width - 1 || j > height - 1)
            return -1;

        return i + j * width;
    }

    // Method to check which neighboring cells the current cell has, and if they are visited or not.
    // Returns one random unvisited cell, or an empty one if all neighbors are visited.
    public Cell lookupNeighbors(int i, int j)
    {
        List<Cell> neighbors = new List<Cell>();
        Cell left = new Cell();
        Cell top = new Cell();
        Cell right = new Cell();
        Cell bot = new Cell();

        if (getNeighborIndex(i - 1, j) != -1 && cells.Length > getNeighborIndex(i - 1, j))
        {
            left = cells[getNeighborIndex(i - 1, j)];
            if (!left.visited)
                neighbors.Add(left);
        }

        if (getNeighborIndex(i, j + 1) != -1 && cells.Length > getNeighborIndex(i, j + 1))
        {
            top = cells[getNeighborIndex(i, j + 1)];
            if (!top.visited)
                neighbors.Add(top);
        }

        if (getNeighborIndex(i + 1, j) != -1 && cells.Length > getNeighborIndex(i + 1, j))
        {
            right = cells[getNeighborIndex(i + 1, j)];
            if (!right.visited)
                neighbors.Add(right);
        }

        if (getNeighborIndex(i, j - 1) != -1 && cells.Length > getNeighborIndex(i, j - 1))
        {
            bot = cells[getNeighborIndex(i, j - 1)];
            if (!bot.visited)
                neighbors.Add(bot);
        }

        if (neighbors.Count > 0)
        {
            int rand = Random.Range(0, neighbors.Count);
            Cell[] nCells = neighbors.ToArray();
            return nCells[rand];
        }
        else
            return new Cell();
    }

    // Method to remove appropriate walls when visiting a "new" cell
    void removeWalls(Cell curr, Cell next)
    {
        int x = next.getX() - curr.getX();
        int y = next.getY() - curr.getY();

        // right
        if (x == 1 && y == 0)
        {
            Destroy(curr.rightWall);
            Destroy(next.leftWall);
            curr.hasRightWall = false;
            next.hasLeftWall = false;
        }
        // up
        if (x == 0 && y == 1)
        {
            Destroy(curr.topWall);
            Destroy(next.botWall);
            curr.hasTopwall = false;
            next.hasBotWall = false;
        }
        // left
        if (x == -1 && y == 0)
        {
            Destroy(curr.leftWall);
            Destroy(next.rightWall);
            curr.hasLeftWall = false;
            next.hasRightWall = false;
        }
        // down
        if (x == 0 && y == -1)
        {
            Destroy(curr.botWall);
            Destroy(next.topWall);
            curr.hasBotWall = false;
            next.hasTopwall = false;
        }
    }

    // Returns the center coordinate of the maze
    Vector3 getCenter()
    {
        Vector3 sumVector = new Vector3(0f, 0f, 0f);

        foreach (Transform child in blockHolder.transform)
        {
            sumVector += child.position;
        }

        Vector3 groupCenter = sumVector / blockHolder.transform.childCount;

        return -groupCenter;
    }

    // Method called by slider input to change maze width
    public void changeWidth(float inputWidth)
    {
        width = (int)inputWidth;
    }

    // Method called by slider input to change maze height
    public void changeHeight(float inputHeight)
    {
        height = (int)inputHeight;
    }

}
