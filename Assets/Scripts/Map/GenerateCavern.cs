using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GenerateCavern : MonoBehaviour
{
    public Tilemap tilemap;
    public TileList TileList;
    public int width, height, percentWater;

    private int currentWidth, currentHeight, currentPW;

    private void createMapCavern(int width, int height, float percentWater)
    {
        randomFillMap(width, height, percentWater);
        makeCaverns(width, height);
    }

    private void randomFillMap(int width, int height, float percentWater)
    {
        int mapMiddle = 0;
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                mapMiddle = height / 2;
                if (row == mapMiddle)
                {
                    setGround(column, row);
                }
                else
                {
                    if (percentWater >= UnityEngine.Random.Range(1, 101))
                    {
                        setWall(column, row);
                    }
                    else
                    {
                        setGround(column, row);
                    }
                }

            }
        }
    }

    private void makeCaverns(int width, int height)
    {
        for (var row = 0; row <= height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                placeWallLogic(width, height, column, row);
            }
        }
    }

    private void placeWallLogic(int width, int height, int x, int y)
    {
        int numWalls = getAdjacentWalls(x, y, 1, 1, width, height);

        if (isWall(x, y, width, height))
        {
            if (numWalls >= 4)
            {
                setWall(x, y);
            }
            if (numWalls < 2)
            {
                setGround(x, y);
            }
        }
        else
        {
            if (numWalls >= 5)
            {
                setWall(x, y);
            }
            else
            {
                setGround(x, y);
            }
        }
    }

    private int getAdjacentWalls(int x, int y, int scopeX, int scopeY, int width, int height)
    {
        int startX = x - scopeX,
          startY = y - scopeY,
          endX = x + scopeX,
          endY = y + scopeY;
        int iX = startX,
          iY = startY;
        int wallCounter = 0;
        for (iY = startY; iY <= endY; iY++)
        {
            for (iX = startX; iX <= endX; iX++)
            {
                if (!(iX == x && iY == y))
                {
                    if (isWall(iX, iY, width, height))
                    {
                        wallCounter += 1;
                    }
                }
            }
        }
        return wallCounter;
    }

    private bool isWall(int x, int y, int width, int height)
    {
        if (isOutOfBounds(x, y, width, height))
        {
            return true;
        }
        if (tilemap.GetTile(new Vector3Int(x, y, 0)) == null)
        {
            return true;
        }
        if (tilemap.GetTile(new Vector3Int(x, y, 0)) != null)
        {
            return false;
        }
        return false;
    }

    private bool isOutOfBounds(int x, int y, int width, int height)
    {
        if (x < 0 || y < 0)
        {
            return true;
        }
        else if (x > width - 1 || y > height - 1)
        {
            return true;
        }
        return false;
    }

    private void fillWithWater(int width, int height)
    {
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                if (tilemap.GetTile(new Vector3Int(column, row, 0)) == null)
                {
                    setWater(column, row);
                }

            }
        }
    }

    private void setWall(int x, int y)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, null);
    }

    private void setGround(int x, int y)
    {
        GroundTile tile = ScriptableObject.CreateInstance<GroundTile>();
        tile.setTile(TileList.brick);
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
    }

    private void setWater(int x, int y)
    {
        WaterTile tile = ScriptableObject.CreateInstance<WaterTile>();
        tile.setTile(TileList.water);
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
    }

    void Start()
    {
        createMapCavern(width, height, percentWater);
        fillWithWater(width, height);
        currentWidth = width;
        currentHeight = height;
        currentPW = percentWater;
    }

    void Update()
    {
        if (currentWidth != width || currentHeight != height || currentPW != percentWater)
        {
            createMapCavern(width, height, percentWater);
            fillWithWater(width, height);
            currentWidth = width;
            currentHeight = height;
            currentPW = percentWater;
        }
    }
}
