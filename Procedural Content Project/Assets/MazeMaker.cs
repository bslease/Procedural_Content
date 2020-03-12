using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeMaker : MonoBehaviour
{
    public int mazeWidth;
    public int mazeHeight;
    public Location mazeStart = new Location(5,0);

    GridLevel levelOne;

    // Start is called before the first frame update
    void Start()
    {
        //Debug.Log("Generating maze with Width = " + mazeWidth + " and Height = " + mazeHeight);
        levelOne = new GridLevel(mazeWidth, mazeHeight);
        generateMaze(levelOne, mazeStart);

        // levelOne.cells is now a two-dimensional array representing each cell in the grid
        // each cell know if it's in the maze and has an array of booleans indicating whether or not
        // it's connected to each of its four neighbors: +x, +y, -y, -x
        // e.g. assuming a traditional 2D coordinate space, true true false false means 
        // this cell connects to its neighbors to the right and down,
        // but not to the cells to the left and up

        //for (int i = 0; i < mazeWidth; i++)
        //{
        //    for (int j = 0; j < mazeHeight; j++)
        //    {
        //        Debug.Log("Cell " + i + "," + j + ": " + levelOne.cells[i, j]);
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        // draw the maze
        for (int x = 0; x < mazeWidth; x++)
        {
            for (int y = 0; y < mazeHeight; y++)
            {
                Connections currentCell = levelOne.cells[x, y];
                if (levelOne.cells[x, y].inMaze)
                { 
                    Vector3 cellPos = new Vector3(x, 0, y);
                    float lineLength = 1f;
                    if (currentCell.directions[0])
                    {
                        // positive x
                        Vector3 neighborPos = new Vector3(x + lineLength, 0, y);
                        Debug.DrawLine(cellPos, neighborPos, Color.cyan);
                    }
                    if (currentCell.directions[1])
                    {
                        // positive y
                        Vector3 neighborPos = new Vector3(x, 0, y + lineLength);
                        Debug.DrawLine(cellPos, neighborPos, Color.cyan);
                    }
                    if (currentCell.directions[2])
                    {
                        // negative y
                        Vector3 neighborPos = new Vector3(x, 0, y - lineLength);
                        Debug.DrawLine(cellPos, neighborPos, Color.cyan);
                    }
                    if (currentCell.directions[3])
                    {
                        // negative x
                        Vector3 neighborPos = new Vector3(x - lineLength, 0, y);
                        Debug.DrawLine(cellPos, neighborPos, Color.cyan);
                    }
                }
            }
        }
    }

    void generateMaze(Level level, Location start)
    {
        // a stack of locations we can branch from
        Stack<Location> locations = new Stack<Location>();
        locations.Push(start);
        level.startAt(start);

        while (locations.Count > 0)
        {
            Location current = locations.Peek();

            // try to connect to a neighboring location
            Location next = level.makeConnection(current);
            if (next != null)
            {
                // if successful, it will be our next iteration
                locations.Push(next);
            }
            else
            {
                locations.Pop();
            }
        }
    }
}
