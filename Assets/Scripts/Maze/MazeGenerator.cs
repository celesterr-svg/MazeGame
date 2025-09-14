using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGenerator : MonoBehaviour
{
    [Header("Grid")]
    public int width = 3;
    public int height = 3;
    public float cellSize = 4f;
    public MazeCell cellPrefab;

    [Header("Random")]
    public bool useRandomSeed = true;
    public int seed = 0;

    MazeCell[,] grid;
    System.Random rng;

    void Start()
    {
        buildGrid();
        generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            clearGrid();
            buildGrid();
            generate();
        }
    }

    void buildGrid()
    {
        rng = useRandomSeed ? new System.Random() : new System.Random(seed);
        grid = new MazeCell[width, height];

        for(int x = 0; x < width; x++)
            for(int y = 0; y < height; y++) 
            { 
                var pos = new Vector3(x * cellSize, 0, y * cellSize);
                var cell = Instantiate(cellPrefab, pos, Quaternion.identity, transform);
                cell.name = $"Cell {x},{y}";
                grid[x, y] = cell;
            }
    }

    void clearGrid()
    {
        if(grid == null)
        {
            return;
        }
        foreach(var c in grid) 
        {
            if (c)
            {
                Destroy(c.gameObject);
            }
        }
    }

    void generate()
    {
        var stack = new Stack<Vector2Int>();
        var current = new Vector2Int(0, 0);

        grid[0,0].visited = true;
        int visitedCount = 1;
        int total = width * height;

        while (visitedCount < total)
        {
            var neigh = UnvisitedNeighbors(current);
            if(neigh.Count > 0)
            {
                var next = neigh[rng.Next(neigh.Count)];
                openBetween(current, next);
                stack.Push(current);
                current = next;
                grid[current.x, current.y].visited = true;
                visitedCount++;
            }
            else if(stack.Count > 0)
            {
                current = stack.Pop();
            } else
            {
                break;
            }

        }
    }

    void openBetween(Vector2Int a, Vector2Int b)
    {
        int dx = b.x - a.x;
        int dy = b.y - a.y;

        if (dx == 1)
        {
            grid[a.x, a.y].Open("E");
            grid[b.x, b.y].Open("W");
        }
        else if (dx == -1)
        {
            grid[a.x, a.y].Open("W");
            grid[a.x, a.y].Open("E");
        }
        else if (dy == 1)
        {
            grid[a.x, a.y].Open("N");
            grid[a.x, a.y].Open("S");
        }
        else if (dy == -1)
        {
            grid[a.x, a.y].Open("S");
            grid[a.x, a.y].Open("N");
        }

    }

    List<Vector2Int> UnvisitedNeighbors(Vector2Int c)
    {
        var list = new List<Vector2Int>();

        if(c.x > 0 && !grid[c.x - 1, c.y].visited)
        {
            list.Add(new Vector2Int(c.x - 1, c.y)); //West
        }

        if(c.x < width - 1 && !grid[c.x + 1, c.y].visited)
        {
            list.Add(new Vector2Int(c.x + 1, c.y)); //East
        }

        if (c.y > 0 && !grid[c.x, c.y -1].visited)
        {
            list.Add(new Vector2Int(c.x, c.y - 1)); //South
        }

        if (c.y < height - 1 && !grid[c.x, c.y + 1].visited)
        {
            list.Add(new Vector2Int(c.x, c.y + 1)); //North
        }

        return list;
    }
}
