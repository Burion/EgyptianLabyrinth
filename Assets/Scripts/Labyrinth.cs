using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System;
using UnityEngine;

public class Labyrinth : MonoBehaviour
{
    /// <summary>
    /// directions for easier orientating in creating labyrinth
    /// </summary>
    enum Direction
    {
        Top,
        Right,
        Bottom,
        Left
    }

    public SearchAlgorythm sa = new SearchAlgorythm();
    public int Height { get; set; }
    public int Width { get; set; }
    //lists of basic items of labyrinth
    public List<Cell> Cells { get; set; }
    public List<Cell> Walls { get; set; }
    //prefabs
    public GameObject cellObj;
    public GameObject wall;
    public GameObject empty;
    Cell currentCell;
    //stack for Maze Generation
    Stack<Cell> stack = new Stack<Cell>();


    public void LabyrinthCreate(int height, int width)
    {
        Cells = new List<Cell>();
        Walls = new List<Cell>();

        // Setting an odd number even if it's even
        Height = height % 2 == 0 ? height - 1 : height;
        Width = width % 2 == 0 ? width - 1 : width;

        for (int i = 1; i < Height; i += 2)
        {
            for (int j = 1; j < Width; j += 2)
            {
                Cells.Add(CreateCell(i, j));
            }
        }

        for (int i = 0; i < Height; i++)
        {
            for (int j = 0; j < Width; j++)
            {
                Walls.Add(CreateCell(i, j));
            }
        }
        MazeGeneration();
    }

    Cell CreateCell(int x, int y)
    {
        Cell cell = new Cell(x, y);
        return cell;
    }

    private void Awake()
    {
        LabyrinthCreate(20, 20);
    }

    public void ShowAxis()
    {
        foreach (Cell cell in Cells)
        {
            GameObject cellobj = Instantiate(cellObj, new Vector2(cell.X, cell.Y), Quaternion.identity);
        }

    }

    /// <summary>
    /// creates basic element of graph
    /// </summary>
    /// <param name="cell"></param>
    void CreateVertex(Cell cell)
    {
        sa.vertices.Add(new Vertex(cell.X, cell.Y));
    }

    /// <summary>
    /// Maze Generation algorythm, applying to labyrinth template we created using backtracking concept
    /// </summary>
    public void MazeGeneration()
    {
        //apply algorythm starting from first cell
        currentCell = Cells.First();
        do
        {
            if (!currentCell.isVisited)
            {
                CreateVertex(currentCell);
                if (sa.vertices.Count > 1)
                    CreateEdge(1, sa.vertices[sa.vertices.Count - 2], sa.vertices.Last()); //edge between last and last but one vertices
            }
            currentCell.isVisited = true; //marking cell visited so that we won't create more than one vertex with particular coords
            Cell nextCell = GetNeighbor(currentCell);
            //if there is at least one available neighbor - remove walls between current and next cell
            if (nextCell != null)
            {
                RemoveWalls(currentCell, nextCell);
            }
            foreach (Cell wall in Walls)
            {
                //removing wall that has the same coordinates that our current cell
                if (wall.X == currentCell.X && wall.Y == currentCell.Y)
                {
                    Walls.Remove(wall);
                    break;
                }
            }

            DisplayCell(currentCell);

            //if there is available next cell - pushing current cell to stack and assigning next to current
            //else - backtracking to cell that has at least one available neighbor
            if (nextCell != null)
            {
                stack.Push(currentCell);
                currentCell = nextCell; //set current cell to next cell we considered to perform same manipulations with it
            }
            else if (stack.Count > 0)
                currentCell = stack.Pop();
            //highlight current cell
            DisplayCell(currentCell);
            // Algorithm is done when current cell is back at the beginning
        } while (!IsCompleted());
        // Display walls
        RemoveRandomWalls();
        DisplayWalls();
    }

    public int x;
    public int y;
    /// <summary>
    /// building walls
    /// </summary>
    public void DisplayWalls()
    {
        foreach (Cell cell in Walls)
        {
            DisplayWall(cell);
        }
    }

    /// <summary>
    /// building paths between walls
    /// </summary>
    public void DisplayCells()
    {
        foreach (Cell cell in Cells)
        {
            DisplayCell(cell);
        }
    }

    /// <summary>
    /// instantiating wall object on scene
    /// </summary>
    /// <param name="cell"></param>
    void DisplayWall(Cell cell)
    {
        Instantiate(wall, new Vector2(cell.X, cell.Y), Quaternion.identity);
    }

    /// <summary>
    /// instantiating cell object on scene
    /// </summary>
    /// <param name="cell"></param>
    void DisplayCell(Cell cell)
    {
        Instantiate(cellObj, new Vector2(cell.X, cell.Y), Quaternion.identity);
    }
    /// <summary>
    /// checking if the Maze Generation is completed
    /// </summary>
    /// <returns></returns>
    bool IsCompleted()
    {
        return stack.Count == 0;
    }

    /// <summary>
    /// returns random neighbor of cell
    /// </summary>
    /// <param name="cell"></param>
    /// <returns></returns>
    Cell GetNeighbor(Cell cell)
    {
        System.Random rand = new System.Random();

        List<Cell> neighbors = new List<Cell>();
        Cell top = (cell.X - 2 > 0) ? Cells.Find(c => c.X == cell.X - 2 && c.Y == cell.Y) : null;
        Cell right = (cell.Y + 2 < Width - 1) ? Cells.Find(c => c.Y == cell.Y + 2 && c.X == cell.X) : null;
        Cell bottom = (cell.X + 2 < Height - 1) ? Cells.Find(c => c.X == cell.X + 2 && c.Y == cell.Y) : null;
        Cell left = (cell.Y - 2 > 0) ? Cells.Find(c => c.Y == cell.Y - 2 && c.X == cell.X) : null;

        if (top != null && !top.isVisited)
        {
            neighbors.Add(top);
        }
        if (right != null && !right.isVisited)
        {
            neighbors.Add(right);
        }
        if (bottom != null && !bottom.isVisited)
        {
            neighbors.Add(bottom);
        }
        if (left != null && !left.isVisited)
        {
            neighbors.Add(left);
        }
        if (neighbors.Count > 0)
        {
            int index = rand.Next(neighbors.Count);
            return neighbors[index];
        }

        return null;
    }

    /// <summary>
    /// removing random walls to make different paths in labyrinth
    /// </summary>
    void RemoveRandomWalls()
    {
        System.Random r = new System.Random();
        for (int i = 0; i < Height; i++)
        {
            RemoveWall(r.Next(Walls.Count));
        }
    }
    void RemoveWall(int x)
    {
        Cell wall = Walls[x];
        if (wall.X != Width - 1 && wall.X != 0 && wall.Y != Height - 1 && wall.Y != 0)
        {

            Cell cell = CreateCell(wall.X, wall.Y);
            CreateVertex(cell);
            if (sa.FindVertex(wall.X, wall.Y + 1) != null)
            {
                CreateEdge(1, sa.FindVertex(wall.X, wall.Y), sa.FindVertex(wall.X, wall.Y + 1));
            }

            if (sa.FindVertex(wall.X, wall.Y - 1) != null)
            {
                CreateEdge(1, sa.FindVertex(wall.X, wall.Y), sa.FindVertex(wall.X, wall.Y - 1));
            }

            if (sa.FindVertex(wall.X + 1, wall.Y) != null)
            {
                CreateEdge(1, sa.FindVertex(wall.X, wall.Y), sa.FindVertex(wall.X + 1, wall.Y));
            }

            if (sa.FindVertex(wall.X - 1, wall.Y) != null)
            {
                CreateEdge(1, sa.FindVertex(wall.X, wall.Y), sa.FindVertex(wall.X - 1, wall.Y));
            }
            Cells.Add(cell);
            DisplayCell(cell);
            Walls.Remove(wall);
        }
    }
    void CreateEdge(int w, Vertex v1, Vertex v2)
    {
        Edge edge1 = new Edge(w, v1, v2);
        sa.edges.Add(edge1);
    }
    void RemoveWalls(Cell a, Cell b)
    {

        int x = (a.X != b.X) ? (a.X > b.X ? a.X - 1 : a.X + 1) : a.X;
        int y = (a.Y != b.Y) ? (a.Y > b.Y ? a.Y - 1 : a.Y + 1) : a.Y;

        foreach (Cell wall in Walls)
        {
            if (wall.X == x && wall.Y == y)
            {
                CreateVertex(wall);
                Cell cell = CreateCell(wall.X, wall.Y);
                CreateEdge(1, sa.FindVertex(a.X, a.Y), sa.FindVertex(wall.X, wall.Y));
                Cells.Add(cell);
                DisplayCell(cell);
                Walls.Remove(wall);
                break;
            }
        }

        if (a.X - b.X == 2)
        {
            a.walls[(int)Direction.Top] = false;
            b.walls[(int)Direction.Bottom] = false;
        }
        else if (a.X - b.X == -2)
        {
            a.walls[(int)Direction.Bottom] = false;
            b.walls[(int)Direction.Top] = false;
        }

        if (a.Y - b.Y == 2)
        {
            a.walls[(int)Direction.Left] = false;
            b.walls[(int)Direction.Right] = false;
        }
        else if (a.Y - b.Y == -2)
        {
            a.walls[(int)Direction.Right] = false;
            b.walls[(int)Direction.Left] = false;
        }
    }
}
public class SearchAlgorythm
{
    public List<Vertex> mapVertex = new List<Vertex>();
    public List<Edge> mapEdges = new List<Edge>();
    public List<Vertex> vertices { get; set; }
    public List<Edge> edges { get; set; }
    public Vertex beginPoint { get; set; }
    public Vertex endPoint { get; set; }


    public SearchAlgorythm()
    {
        vertices = new List<Vertex>();
        edges = new List<Edge>();
    }

    public void Annulate()
    {
        foreach (Vertex v in vertices)
        {
            v.valueMark = 10000;
            v.isChecked = false;
            v.prevVert = null;
        }
    }
    public SearchAlgorythm(List<Vertex> vertlist, List<Edge> edgelist)
    {
        vertices = vertlist;
        edges = edgelist;
    }




    /// <summary>
    /// Dejikstra search for finding shortest path to target vertex 
    /// </summary>
    /// <param name="beginPnt">start vertex</param>
    /// <param name="endPnt">finish vertex</param>
    /// <returns></returns>
    public string Dejikstra(Vertex beginPnt, Vertex endPnt)
    {

        beginPoint = beginPnt;
        Step(beginPoint);

        while (!isCompleted())
        {
            Vertex vertToCheck = FindMin();
            Step(vertToCheck);
        }

        string pathstring = "";

        foreach (Vertex v in GetPath(endPnt))
        {
            pathstring = pathstring + "(" + v.x + " ," + v.y + ") -";
        }
        return pathstring;
    }


    public List<Vertex> GetPath()
    {
        return GetPath(vertices.Last());
    }

    /// <summary>
    /// returns list of vertex representing the path to aim vertex
    /// </summary>
    /// <param name="v"></param>
    /// <returns></returns>
    public List<Vertex> GetPath(Vertex v)
    {
        List<Vertex> path = new List<Vertex>();
        if (v.prevVert != null)
            path = GetPath(v.prevVert);
        path.Add(v);
        return path;


    }

    bool isCompleted()
    {
        foreach (Vertex v in vertices)
        {
            if (!v.isChecked)
                return false;
        }
        return true;
    }
    /// <summary>
    /// finding vertex with minimum mark
    /// </summary>
    /// <returns></returns>
    Vertex FindMin()
    {
        int min = 10000;
        Vertex minV = null;
        foreach (Vertex v in vertices)
        {
            if (v.valueMark < min && !v.isChecked)
            {
                min = v.valueMark;
                minV = v;
            }

        }
        return minV;
    }
    /// <summary>
    /// step of Dejikstra algorythm
    /// </summary>
    /// <param name="beginpoint"></param>
    public void Step(Vertex beginpoint)
    {
        foreach (Vertex nextp in Neigbors(beginpoint))
        {
            if (nextp.isChecked == false)//не отмечена
            {
                int newmark = beginpoint.valueMark + GetMyEdge(nextp, beginpoint).Weight;
                if (nextp.valueMark > newmark)
                {
                    nextp.valueMark = newmark;
                    nextp.prevVert = beginpoint;
                }
                else
                {

                }
            }
        }
        beginpoint.isChecked = true;//вычеркиваем
    }
    /// <summary>
    /// returns list of neighbour vertexes
    /// </summary>
    /// <param name="vert"></param>
    /// <returns></returns>
    List<Vertex> Neigbors(Vertex vert)
    {
        List<Vertex> neighbors = new List<Vertex>();
        foreach (Edge e in edges)
        {
            if (e.v1 == vert || e.v2 == vert)
            {
                if (e.v1 == vert)
                    neighbors.Add(e.v2);
                else
                    neighbors.Add(e.v1);
            }
        }
        return neighbors;
    }

    Edge GetMyEdge(Vertex vert1, Vertex vert2)
    {
        foreach (Edge e in edges)
        {
            if ((e.v1 == vert1 && e.v2 == vert2) || (e.v1 == vert2 && e.v2 == vert1))
            {
                return e;
            }
        }
        return null;

    }

    public Vertex GetRandomVertex()
    {
        System.Random rand = new System.Random();
        return vertices[rand.Next(vertices.Count)];
    }
    public Vertex FindVertex(int x, int y)
    {
        foreach (Vertex v in vertices)
        {
            if (v.x == x && v.y == y)
                return v;
        }
        return null;
    }


}

//classes of grapgh data strucrure

public class Vertex
{
    public int id;
    public bool isChecked;
    public int valueMark;
    public Vertex prevVert;
    public int x;
    public int y;

    public Vertex(int x, int y)
    {
        isChecked = false;
        valueMark = 10000;
        prevVert = null;
        this.x = x;
        this.y = y;
    }

}

public class Edge
{
    public int Weight;
    public Vertex v1;
    public Vertex v2;
    public Edge(int w, Vertex v1, Vertex v2)
    {
        Weight = w;
        this.v1 = v1;
        this.v2 = v2;
    }
}

