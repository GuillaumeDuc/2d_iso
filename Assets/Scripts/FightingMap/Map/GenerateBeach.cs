using System.Collections.Generic;
using UnityEngine.Tilemaps;

public class GenerateBeach
{
    public static void createBeach(
        int width, 
        int height, 
        int waterHeightMin, 
        int waterHeightMax, 
        TileList TileList, 
        Tilemap tilemap
        )
    {
        // Fill beach with sand 
        GenerateGround.fillMap(width, height, TileList.sand, tilemap);
        generateGrass(width, height, TileList, tilemap);
        generateOcean(width, waterHeightMin, waterHeightMax, TileList, tilemap);
    }

    private static void generateOcean(int width, int waterHeightMin, int waterHeightMax, TileList TileList, Tilemap tilemap)
    {
        int min = 0;
        int max = width;
        int randHeight = UnityEngine.Random.Range(waterHeightMin, waterHeightMax + 1);
        for (int i = 0; i < randHeight; i++)
        {
            for (int j = min; j < max; j++)
            {
                GenerateGround.setGround(j, i, TileList.water, tilemap);
            }
            min += 1;
            max -= 1;
        }
    }

    private static void generateGrass(int width, int height, TileList TileList, Tilemap tilemap)
    {
        int currentHeight = height / 3;
        for (int i = 0; i < width; i++)
        {
            for (int j = currentHeight; j < height; j++)
            {
                GenerateGround.setGround(i, j, TileList.grass, tilemap);
            }
        }
        generateRandom(width, 2, currentHeight, TileList, tilemap);
    }

    private static void generateRandom(int width, int height, int pos, TileList TileList, Tilemap tilemap)
    {
        for (int i = 0; i < width; i++)
        {
            for (int j = pos; j < pos + height; j++)
            {
                int rndInd = UnityEngine.Random.Range(0, 2);
                List<GroundTile> listTile = new List<GroundTile>() { TileList.grass, TileList.sand };
                GroundTile tile = listTile[rndInd];
                GenerateGround.setGround(i, j, tile, tilemap);
            }
        }
    }
}
