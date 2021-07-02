using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class GenerateWater
{
    public static void createLake(int width, int height, int numberLake, int maxWidthLake, Tilemap tilemap, WaterTile water)
    {
        List<Vector3Int> listLake = generateLake(width, height, numberLake, maxWidthLake);
        listLake.ForEach(cell =>
        {
            GenerateGround.setGround(cell.x, cell.y, water, tilemap);
        });
    }

    public static List<Vector3Int> generateLake(int width, int height, int numberLake, int depth)
    {
        List<Vector3Int> waterList = new List<Vector3Int>();
        // Generate random point of spawn
        for (int i = 0; i < numberLake; i++)
        {
            Vector3Int randomPos = new Vector3Int(
                UnityEngine.Random.Range(0, width),
                UnityEngine.Random.Range(0, height),
                0
                );
            if (!waterList.Contains(randomPos))
            {
                waterList.Add(randomPos);
            }
        }

        for (int i = 0; i < depth; i++)
        {
            List<Vector3Int> neighbours = new List<Vector3Int>();
            for (int j = 0; j < waterList.Count; j++)
            {
                Vector3Int rndCell = randomCell(waterList[j], width, height);
                if (isValid(rndCell, width, height, neighbours, waterList))
                {
                    neighbours.Add(rndCell);
                }
            }
            if (neighbours.Any())
            {
                waterList = waterList.Concat(neighbours).ToList();
            }
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
        int rndInd = UnityEngine.Random.Range(0, randomList.Count);
        return randomList[rndInd];
    }

    private static bool isValid(Vector3Int cell, int width, int height, List<Vector3Int> neighbours, List<Vector3Int> waterList)
    {
        if (cell.x < width &&
            cell.y < height &&
            cell.x >= 0 &&
            cell.y >= 0 &&
            !neighbours.Contains(cell) &&
            !waterList.Contains(cell)
            )
        {
            return true;
        }
        return false;
    }
}
