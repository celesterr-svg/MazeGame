using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MazeGeneratorSecond : MonoBehaviour
{
    [Header("Tamaño del laberinto")]
    public int width = 15;
    public int height = 10;
    public float cellSize = 2f;
    public float baseY = 0f;
    public float wallThickness = 0.2f;
    public float wallHeight = 80f;
    public bool sitOnGround = true;

    [Header("Prefabs / Visual")]
    public GameObject wallPrefab;
    public GameObject exitPrefab;
    public bool addOuterBorder = true;
    public Transform mazeParent; // opcional: dónde anidar las paredes
    public bool generateOnStart = true;

    [Header("Objetos del laberinto")]
    public GameObject pila;
    public GameObject antiPila;
    public GameObject mapaItem;
    public GameObject teleporter;
    [Range(0f, 1f)] public float pilaSpawnChance;
    [Range(0f, 1f)] public float mapaItemSpawnChance;
    [Range(0f, 1f)] public float antiPilaSpawnChance;
    [Range(0f, 1f)] public float teleporterSpawnChance;
    float itemExtraYOffset = 0.5f;

    [Header("Trampas del laberinto")]
    public GameObject Pinches;
    public GameObject Lanzas;
    public GameObject SalidaFalsa;
    [Range(0f, 1f)] public float pinchesSpawnChance;    
    [Range(0f, 1f)] public float lanzasSpawnChance;
    public bool placeFakeWall;
    public float trapExtraYOffset = 0;

    [Header("Algoritmo (Growing Tree)")]
    [Range(0f, 1f)]
    public float randomPickProbability = 0.25f;
    // 0 = siempre toma la celda más nueva (estilo DFS, pasillos largos)
    // 1 = siempre toma una celda al azar de la lista (más “random”)

    [Tooltip("Usar una semilla fija (0 = desactivado)")]
    public int fixedSeed = 0;
    private List<Transform> spawnedWalls = new List<Transform>();
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

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            Generate();
        }
    }
    void Start()
    {
        if (generateOnStart) 
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

        spawnedWalls.Clear();
        BuildMazeGeometry();

        if (placeFakeWall)
        {
            PlaceFakeWall();
        }
        PlaceSpecialWall();
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

        float yCenter = baseY + (sitOnGround ? wallHeight * 0.5f : 0f);

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Vector3 cellCenter = origin + new Vector3((x + 0.5f) * cellSize, yCenter, (y + 0.5f) * cellSize);

                if(pila != null && rng.NextDouble() < pilaSpawnChance)
                {
                    Vector3 pos = new Vector3(cellCenter.x, baseY, cellCenter.z);
                    var item = Instantiate(pila, pos, Quaternion.identity, parent);
                    PlaceOnFloor(item.transform, baseY, itemExtraYOffset);
                }

                if (mapaItem != null && rng.NextDouble() < mapaItemSpawnChance) 
                {
                    Vector3 pos = new Vector3(cellCenter.x, baseY, cellCenter.z);
                    var item = Instantiate(mapaItem, pos, Quaternion.identity, parent);
                    PlaceOnFloor(item.transform, baseY, itemExtraYOffset);
                }

                if (antiPila != null && rng.NextDouble() < antiPilaSpawnChance)
                {
                    Vector3 pos = new Vector3(cellCenter.x, baseY, cellCenter.z);
                    var item = Instantiate(antiPila, pos, Quaternion.identity, parent);
                    PlaceOnFloor(item.transform, baseY, itemExtraYOffset);
                }

                if (teleporter != null && rng.NextDouble() < teleporterSpawnChance)
                {
                    Vector3 pos = new Vector3(cellCenter.x, baseY, cellCenter.z);
                    var item = Instantiate(teleporter, pos, Quaternion.identity, parent);
                    PlaceOnFloor(item.transform, baseY);
                }

                if (Pinches != null && rng.NextDouble() < pinchesSpawnChance)
                {
                    Vector3 pos = new Vector3(cellCenter.x, baseY, cellCenter.z);
                    var item = Instantiate(Pinches, pos, Quaternion.identity, parent);
                    PlaceOnFloor(item.transform, baseY);
                }

                if (Lanzas != null && rng.NextDouble() < pinchesSpawnChance)
                {
                    Vector3 pos = new Vector3(cellCenter.x, baseY, cellCenter.z);
                    var item = Instantiate(Lanzas, pos, Quaternion.identity, parent);
                    PlaceOnFloor(item.transform, trapExtraYOffset);
                }

                if (y < height - 1 && grid[x, y].walls[(int)Dir.N])
                {
                    Vector3 pos = cellCenter + new Vector3(0f, 0f, +cellSize * 0.5f);
                    SpawnWall(pos, new Vector3(cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
                }

                // Este (solo si no es la última columna; el borde lo pone addOuterBorder)
                if (x < width - 1 && grid[x, y].walls[(int)Dir.E])
                {
                    Vector3 pos = cellCenter + new Vector3(+cellSize * 0.5f, 0f, 0f);
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
            SpawnWall(new Vector3((min.x + max.x) * 0.5f, yCenter, max.z),
                new Vector3(width * cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
            // Sur
            SpawnWall(new Vector3((min.x + max.x) * 0.5f, yCenter, min.z),
                new Vector3(width * cellSize, wallHeight, wallThickness), Quaternion.identity, parent);
            // Este
            SpawnWall(new Vector3(max.x, yCenter, (min.z + max.z) * 0.5f),
                new Vector3(wallThickness, wallHeight, height * cellSize), Quaternion.identity, parent);
            // Oeste
            SpawnWall(new Vector3(min.x, yCenter, (min.z + max.z) * 0.5f),
                new Vector3(wallThickness, wallHeight, height * cellSize), Quaternion.identity, parent);
        }
    }

    private void SpawnWall(Vector3 position, Vector3 scale, Quaternion rot, Transform parent)
    {
        if (wallPrefab == null && exitPrefab == null) 
        { 
            return; 
        }        

        GameObject prefabToUse = wallPrefab != null ? wallPrefab : exitPrefab;   
        
        var go = Instantiate(prefabToUse, position, rot, parent);
        go.transform.localScale = scale;
        go.name = "Wall";

        spawnedWalls.Add(go.transform);
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
    private void PlaceSpecialWall()
    {
        if(exitPrefab == null)
        {
            return;
        }

        if(spawnedWalls.Count == 0)
        {
            return;
        }

        int idx = rng.Next(spawnedWalls.Count);
        Transform w = spawnedWalls[idx];

        Vector3 pos = w.position;
        Quaternion rot = w.rotation;
        Vector3 scale = w.localScale;
        Transform parent = w.parent;

        Destroy(w.gameObject);

        var special = Instantiate(exitPrefab, pos, rot, parent);
        special.transform.localScale = scale;
        special.name = "Exit";
    }

    private void PlaceFakeWall()
    {
        if (exitPrefab == null)
        {
            return;
        }

        if (spawnedWalls.Count == 0)
        {
            return;
        }

        int idx = rng.Next(spawnedWalls.Count);
        Transform w = spawnedWalls[idx];

        Vector3 pos = w.position;
        Quaternion rot = w.rotation;
        Vector3 scale = w.localScale;
        Transform parent = w.parent;

        Destroy(w.gameObject);

        var special = Instantiate(SalidaFalsa, pos, rot, parent);
        special.transform.localScale = scale;
        special.name = "Fake Exit";
    }

    private void PlaceOnFloor(Transform t, float floorY, float extraOffset = 0f)
    {
        if (TryGetWorldBounds(t, out var b))
        {
            float bottom = b.center.y - b.extents.y;   // y del “borde inferior” actual
            float delta = floorY + extraOffset - bottom; // cuánto hay que mover hacia arriba/abajo
            t.position += new Vector3(0f, delta, 0f);
        }
        else
        {
            // Fallback: si no hay Renderer ni Collider, asumimos pivot al centro (ajustá si querés)
            t.position = new Vector3(t.position.x, floorY + 0.5f + extraOffset, t.position.z);
        }
    }
    private bool TryGetWorldBounds(Transform t, out Bounds bounds)
    {
        var renderers = t.GetComponentsInChildren<Renderer>();
        if (renderers.Length > 0)
        {
            bounds = renderers[0].bounds;
            for (int i = 1; i < renderers.Length; i++)
                bounds.Encapsulate(renderers[i].bounds);
            return true;
        }

        var colliders = t.GetComponentsInChildren<Collider>();
        if (colliders.Length > 0)
        {
            bounds = colliders[0].bounds;
            for (int i = 1; i < colliders.Length; i++)
                bounds.Encapsulate(colliders[i].bounds);
            return true;
        }

        bounds = default; // <- ahora SIEMPRE queda asignada
        return false;
    }

    public Vector3 GetRandomSafePoint(float playerRadius, float extraMargin)
    {
        int x = rng.Next(width);
        int y = rng.Next(height);

        Vector3 origin = new Vector3(-(width * cellSize) * 0.5f, 0f, -(height * cellSize) * 0.5f);
        Vector3 cellCenter = origin + new Vector3((x + 0.5f) * cellSize, 0f, (y + 0.5f) * cellSize);

        float half = cellSize * 0.5f;
        float margin = playerRadius + extraMargin + wallThickness * 0.5f;
        float range = Mathf.Max(half - margin, 0.01f);

        float dx = (float)(rng.NextDouble() * 2 - 1) * range;
        float dz = (float)(rng.NextDouble() * 2 - 1 * range);

        return new Vector3(cellCenter.x + dx, baseY, cellCenter.z + dz);
    }
}
