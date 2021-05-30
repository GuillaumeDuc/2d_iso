using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GenerateWater
{
    public static void createLake(int width, int height, int numberLake, int depth, Tilemap tilemap, WaterTile water)
    {
        List<Vector3Int> listLake = generateLake(width, height, numberLake, depth);
        listLake.ForEach(cell =>
        {
            WaterTile tile = ScriptableObject.CreateInstance<WaterTile>();
            tile.setTile(water);
            tilemap.SetTile(cell, tile);
        });
    }

    public static List<Vector3Int> generateLake(int width, int height, int numberLake, int depth)
    {
        List<Vector3Int> waterList = new List<Vector3Int>();
        // Generate random point of spawn
        for (int i = 0; i < numberLake; i++)
        {
            Vector3Int randomPos = new Vector3Int(
                UnityEngine.Random.Range(0, width -1),
                UnityEngine.Random.Range(0, height -1),
                0
                );
            waterList.Add(randomPos);
        }

        for (int i = 0; i < depth; i++)
        {
            List<Vector3Int> neighbours = new List<Vector3Int>();
            foreach (var w in waterList)
            {
                Vector3Int rndCell = randomCell(w, width, height);
                if (isValid(rndCell, width, height, waterList, neighbours))
                {
                    neighbours.Add(rndCell);
                }
            }
            waterList = waterList.Concat(neighbours).ToList();
        }
        return waterList;
    }

    private static Vector3Int randomCell(Vector3Int cell, int width, int height)
    {
        Vector3Int up = new Vector3Int(cell.x, cell.y + 1, cell.z);
        Vector3Int down = new Vector3Int(cell.x, cell.y - 1, cell.z);
        Vector3Int left = new Vector3Int(cell.x - 1, cell.y, cell.z);
        Vector3Int right = new Vector3Int(cell.x + 1, cell.y, cell.z);

        List<Vector3Int> randomList = new List<Vector3Int>() { up, down, left, right };
        int rndInd = UnityEngine.Random.Range(1, randomList.Count);
        return randomList[rndInd];
    }

    private static bool isValid(Vector3Int cell, int width, int height, List<Vector3Int> waterList, List<Vector3Int> neighbours)
    {
        if (cell.x < width &&
            cell.y < height &&
            cell.x >= 0 &&
            cell.y >= 0 &&
            !waterList.Contains(cell) &&
            !neighbours.Contains(cell)
            )
        {
            return true;
        }
        return false;
    }
}
