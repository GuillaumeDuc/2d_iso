using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public enum TileType { SAND, BRICK, GRASS }

public class GenerateForest : MonoBehaviour
{
    public TileList TileList;
    public Tilemap tilemap;
    public GameObject Tree;
    public TileType TileType;
    public int width, height, spawnsTree, radius = 2, rejection = 10, spawnsWater, widthWater;

    RangeUtils RangeUtils = new RangeUtils();

    public List<Vector3Int> poissonDisk(int spawns, int radius, int rejection, int width, int height)
    {
        float cellSize = (int)radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(width / cellSize)];
        List<Vector3Int> points = new List<Vector3Int>();
        List<Vector3Int> spawnPoints = new List<Vector3Int>();

        // Generate random point of spawn
        for (int i = 0; i < spawns; i++)
        {
            Vector3Int randomPos = new Vector3Int(
                UnityEngine.Random.Range(0, width),
                UnityEngine.Random.Range(0, height),
                0
                );
            spawnPoints.Add(randomPos);
        }

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector3Int spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < rejection; i++)
            {
                // Spawn random point in circle
                Vector3Int candidate = getCandidate(spawnCenter, radius);
                if (isValid(candidate, width, height, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    public Vector3Int getCandidate(Vector3Int center, int radius)
    {
        List<Vector3Int> listCandidates = RangeUtils.getAreaCircleFull(center, radius * 2, tilemap, radius);
        int spawnIndex = UnityEngine.Random.Range(0, listCandidates.Count);
        Vector3Int candidate = listCandidates[spawnIndex];
        return candidate;
    }

    bool isValid(Vector3Int candidate, int width, int height, float cellSize, float radius, List<Vector3Int> points, int[,] grid)
    {
        if (candidate.x >= 0 && candidate.x < width && candidate.y >= 0 && candidate.y < height)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        int dist = RangeUtils.getLine(candidate, points[pointIndex]).Count - 1;
                        if (dist < radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }

    void randomGenerateForest(int spawns, int radius, int rejection, int width, int height)
    {
        List<Vector3Int> list = poissonDisk(spawns, radius, rejection, width, height);
        list.ForEach(a =>
        {
            GroundTile tile = (GroundTile)tilemap.GetTile(a);
            bool isWater = tile is WaterTile;
            if (!isWater)
            {
                Vector3 posWorld = tilemap.CellToWorld(a);
                posWorld.y += 0.2f;
                Instantiate(Tree, posWorld, Quaternion.identity);
            }
        });
    }

    void createMap(int width, int height, TileType TileType, Tilemap tilemap, TileList TileList)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                setGround(x, y, TileType, tilemap, TileList);
            }
        }
    }

    private static void setGround(int x, int y, TileType TileType, Tilemap tilemap, TileList TileList)
    {
        GroundTile tile = ScriptableObject.CreateInstance<GroundTile>();
        switch (TileType)
        {
            case TileType.BRICK:
                tile.setTile(TileList.brick);
                break;
            case TileType.SAND:
                tile.setTile(TileList.sand);
                break;
            default:
                tile.setTile(TileList.grass);
                break;
        }
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
    }

    // Start is called before the first frame update
    void Start()
    {
        RangeUtils = new RangeUtils();
        // GenerateCavern.createMapCavern(width, height, percentWater, TileType, tilemap, TileList);
        createMap(width, height, TileType, tilemap, TileList);
        GenerateWater.createLake(width, height, spawnsWater, widthWater, tilemap, TileList.water);
        randomGenerateForest(spawnsTree, radius, rejection, width, height);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
