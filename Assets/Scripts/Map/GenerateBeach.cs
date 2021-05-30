using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateBeach : MonoBehaviour
{
    public TileList TileList;
    public Tilemap tilemap;
    public int width, height, waterHeightMin, waterHeightMax;

    public void createBeach(int width, int height, int waterHeightMin, int waterHeightMax)
    {
        // Fill beach with sand 
        GenerateGround.fillMap(width, height, TileList.sand, tilemap);
        generateGrass(width, height);
        generateOcean(width, waterHeightMin, waterHeightMax);
    }

    private void generateOcean(int width, int waterHeightMin, int waterHeightMax)
    {
        int min = 0;
        int max = width;
        int randHeight = UnityEngine.Random.Range(waterHeightMin, waterHeightMax+1);
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

    private void generateGrass(int width, int height)
    {
        int currentHeight = height / 3;
        for (int i = 0; i < width; i++)
        {
            for (int j = currentHeight; j < height; j++)
            {
                GenerateGround.setGround(i, j, TileList.grass, tilemap);
            }
        }
        generateRandom(width, 2, currentHeight);
    }

    private void generateRandom(int width, int height, int pos)
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

    // Start is called before the first frame update
    void Start()
    {
        createBeach(width, height, waterHeightMin, waterHeightMax);
    }

    // Update is called once per frame
    void Update()
    {

    }
}
