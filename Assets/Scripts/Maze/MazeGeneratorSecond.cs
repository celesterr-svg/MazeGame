using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorSecond : MonoBehaviour
{
    [Header("Tamaño del laberinto")]
    public int width = 15;
    public int height = 10;
    public float cellSize = 2f;

    [Header("Prefabs / Visual")]
    public GameObject wallPrefab;
    public bool addOuterBorder = true;
    public Transform mazeParent; // opcional: dónde anidar las paredes
    public bool generateOnStart = true;

    [Header("Algoritmo (Growing Tree)")]
    [Range(0f, 1f)]
    public float randomPickProbability = 0.25f;
    // 0 = siempre toma la celda más nueva (estilo DFS, pasillos largos)
    // 1 = siempre toma una celda al azar de la lista (más “random”)

    [Tooltip("Usar una semilla fija (0 = desactivado)")]
    public int fixedSeed = 0;

    private Cell[,] grid;
    private System.Random rng;
    private readonly Vector2Int[] dirs = new Vector2Int[]
    {
        new Vector2Int(0, 1),   // N
        new Vector2Int(1, 0),   // E
        new Vector2Int(0, -1),  // S
        new Vector2Int(-1, 0),  // W
    };

    private enum Dir { N = 0, E = 1, S = 2, W = 3 }

    private class Cell
    {
        public bool visited = false;
        // walls[0]=N, [1]=E, [2]=S, [3]=W
        public bool[] walls = { true, true, true, true };
    }

    void Start()
    {
        if (generateOnStart) Generate();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }
    }

    public void Generate()
    {
        // limpiar laberinto previo
        ClearMaze();

        // RNG
        rng = (fixedSeed != 0) ? new System.Random(fixedSeed) : new System.Random();

        // crear grilla
        grid = new Cell[width, height];
        for (int x = 0; x < width; x++)
            for (int y = 0; y < height; y++)
                grid[x, y] = new Cell();

        // algoritmo Growing Tree
        RunGrowingTree();

        // construir paredes
        BuildMazeGeometry();
    }

    private void RunGrowingTree()
    {
        List<Vector2Int> list = new List<Vector2Int>();
        Vector2Int start = new Vector2Int(rng.Next(width), rng.Next(height));
        grid[start.x, start.y].visited = true;
        list.Add(start);

        while (list.Count > 0)
        {
            // elegir índice: newest o random según probabilidad
            int idx = (rng.NextDouble() < randomPickProbability)
                ? rng.Next(list.Count)
                : list.Count - 1;

            Vector2Int current = list[idx];

            // vecinos no visitados
            List<(Vector2Int pos, Dir d)> neighbors = UnvisitedNeighbors(current);
            if (neighbors.Count > 0)
            {
                var (nextPos, dir) = neighbors[rng.Next(neighbors.Count)];
                // abrir paredes entre current y next
                KnockDownWall(current, nextPos, dir);
                // marcar visitado y agregar a la lista
                grid[nextPos.x, nextPos.y].visited = true;
                list.Add(nextPos);
            }
            else
            {
                // sin vecinos no visitados: remover de la lista
                list.RemoveAt(idx);
            }
        }
    }

    private List<(Vector2Int pos, Dir d)> UnvisitedNeighbors(Vector2Int c)
    {
        List<(Vector2Int, Dir)> result = new List<(Vector2Int, Dir)>();
        for (int i = 0; i < 4; i++)
        {
            Vector2Int n = c + dirs[i];
            if (InBounds(n) && !grid[n.x, n.y].visited)
            {
                result.Add((n, (Dir)i));
            }
        }
        return result;
    }

    private void KnockDownWall(Vector2Int a, Vector2Int b, Dir dirFromAtoB)
    {
        // Abrir pared en A
        grid[a.x, a.y].walls[(int)dirFromAtoB] = false;
        // Abrir pared opuesta en B
        Dir opposite = Opposite(dirFromAtoB);
        grid[b.x, b.y].walls[(int)opposite] = false;
    }

    private Dir Opposite(Dir d)
    {
        switch (d)
        {
            case Dir.N: return Dir.S;
            case Dir.E: return Dir.W;
            case Dir.S: return Dir.N;
            case Dir.W: return Dir.E;
        }
        return Dir.N;
    }

    private bool InBounds(Vector2Int p)
    {
        return p.x >= 0 && p.x < width && p.y >= 0 && p.y < height;
    }

    private void BuildMazeGeometry()
    {
        // Parent opcional
        Transform parent = mazeParent != null ? mazeParent : this.transform;

        // Por cada celda, instanciamos sus paredes “cerradas”
        // walls[0]=N, [1]=E, [2]=S, [3]=W
        // Centramos el laberinto alrededor del origen
        Vector3 origin = new Vector3(-(width * cellSize) * 0.5f, 0f, -(height * cellSize) * 0.5f);

        float wallThickness = 0.2f; // grosor visual si usás un cubo
        float wallHeight = 80f;      // alto visual si usás un cubo

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellCenter = origin + new Vector3((x + 0.5f) * cellSize, 0f, (y + 0.5f) * cellSize);

                // Norte
                if (grid[x, y].walls[(int)Dir.N])
                {
                    Vector3 pos = cellCenter + new Vector3(0f, 0f, +cellSize * 0.5f);
                    SpawnWall(pos, new Vector3(cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
                }
                // Este
                if (grid[x, y].walls[(int)Dir.E])
                {
                    Vector3 pos = cellCenter + new Vector3(+cellSize * 0.5f, 0f, 0f);
                    SpawnWall(pos, new Vector3(wallThickness, wallHeight, cellSize), Quaternion.identity, parent);
                }
                // Sur
                if (grid[x, y].walls[(int)Dir.S])
                {
                    Vector3 pos = cellCenter + new Vector3(0f, 0f, -cellSize * 0.5f);
                    SpawnWall(pos, new Vector3(cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
                }
                // Oeste
                if (grid[x, y].walls[(int)Dir.W])
                {
                    Vector3 pos = cellCenter + new Vector3(-cellSize * 0.5f, 0f, 0f);
                    SpawnWall(pos, new Vector3(wallThickness, wallHeight, cellSize), Quaternion.identity, parent);
                }
            }
        }

        // Borde externo (opcional) para asegurar perímetro cerrado/visible
        if (addOuterBorder)
        {
            Vector3 min = origin;
            Vector3 max = origin + new Vector3(width * cellSize, 0f, height * cellSize);

            // Norte
            SpawnWall(new Vector3((min.x + max.x) * 0.5f, 0f, max.z),
                new Vector3(width * cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
            // Sur
            SpawnWall(new Vector3((min.x + max.x) * 0.5f, 0f, min.z),
                new Vector3(width * cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
            // Este
            SpawnWall(new Vector3(max.x, 0f, (min.z + max.z) * 0.5f),
                new Vector3(wallThickness, wallHeight, height * cellSize), Quaternion.identity, parent);
            // Oeste
            SpawnWall(new Vector3(min.x, 0f, (min.z + max.z) * 0.5f),
                new Vector3(wallThickness, wallHeight, height * cellSize), Quaternion.identity, parent);
        }
    }

    private void SpawnWall(Vector3 position, Vector3 scale, Quaternion rot, Transform parent)
    {
        if (wallPrefab == null) return;
        var go = Instantiate(wallPrefab, position, rot, parent);
        go.transform.localScale = scale;
        go.name = "Wall";
    }

    private void ClearMaze()
    {
        Transform parent = mazeParent != null ? mazeParent : this.transform;

        // destruir sólo hijos instanciados (no te rompe otros objetos que cuelguen del mismo padre si renombrás por seguridad)
        List<Transform> toDestroy = new List<Transform>();
        foreach (Transform t in parent)
        {
            if (t.name == "Wall" || t.name.StartsWith("Wall"))
                toDestroy.Add(t);
        }
        for (int i = 0; i < toDestroy.Count; i++)
        {
            if (Application.isPlaying) Destroy(toDestroy[i].gameObject);
            else DestroyImmediate(toDestroy[i].gameObject);
        }
    }
}
