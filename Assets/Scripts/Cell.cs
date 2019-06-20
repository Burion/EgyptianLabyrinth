using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : Point
{
    //variables for creating labyrinth
    public bool[] walls;
    public bool isVisited;

    public Cell(int x, int y) : base(x, y)
    {
        isVisited = false;
        walls = new bool[] { true, true, true, true };
    }
}
