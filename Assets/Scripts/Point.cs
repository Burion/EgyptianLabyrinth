using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Point
{

    public virtual int X { get; set; }
    public virtual int Y { get; set; }


    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }
}
