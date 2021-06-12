using UnityEngine;
using UnityEngine.Tilemaps;

public static class GenerateCavern
{
    public static void createMapCavern(int width, int height, float percentWater, GroundTile tile, WaterTile wt, Tilemap tilemap)
    {
        randomFillMap(width, height, percentWater, tile, tilemap);
        makeCaverns(width, height, tile, tilemap);
        fillWithWater(width, height, wt, tilemap);
    }

    private static void randomFillMap(int width, int height, float percentWater, Tile tile, Tilemap tilemap)
    {
        int mapMiddle = 0;
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                mapMiddle = height / 2;
                if (row == mapMiddle)
                {
                    setGround(column, row, tile, tilemap);
                }
                else
                {
                    if (percentWater >= UnityEngine.Random.Range(1, 101))
                    {
                        setWall(column, row, tilemap);
                    }
                    else
                    {
                        setGround(column, row, tile, tilemap);
                    }
                }

            }
        }
    }

    private static void makeCaverns(int width, int height, Tile tileType, Tilemap tilemap)
    {
        for (var row = 0; row <= height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                placeWallLogic(width, height, column, row, tileType, tilemap);
            }
        }
    }

    private static void placeWallLogic(int width, int height, int x, int y, Tile tileType, Tilemap tilemap)
    {
        int numWalls = getAdjacentWalls(x, y, 1, 1, width, height, tilemap);

        if (isWall(x, y, width, height, tilemap))
        {
            if (numWalls >= 4)
            {
                setWall(x, y, tilemap);
            }
            if (numWalls < 2)
            {
                setGround(x, y, tileType, tilemap);
            }
        }
        else
        {
            if (numWalls >= 5)
            {
                setWall(x, y, tilemap);
            }
            else
            {
                setGround(x, y, tileType, tilemap);
            }
        }
    }

    private static int getAdjacentWalls(int x, int y, int scopeX, int scopeY, int width, int height, Tilemap tilemap)
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
                    if (isWall(iX, iY, width, height, tilemap))
                    {
                        wallCounter += 1;
                    }
                }
            }
        }
        return wallCounter;
    }

    private static bool isWall(int x, int y, int width, int height, Tilemap tilemap)
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

    private static bool isOutOfBounds(int x, int y, int width, int height)
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

    private static void fillWithWater(int width, int height, WaterTile wt, Tilemap tilemap)
    {
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                if (tilemap.GetTile(new Vector3Int(column, row, 0)) == null)
                {
                    setWater(column, row, wt, tilemap);
                }
            }
        }
    }

    private static void refreshGround(int height, int width, Tile tileType, Tilemap tilemap)
    {
        for (var row = 0; row < height; row++)
        {
            for (var column = 0; column < width; column++)
            {
                GroundTile gt = (GroundTile)tilemap.GetTile(new Vector3Int(column, row, 0));
                bool isWater = gt is WaterTile;
                if (gt != null && !isWater)
                {
                    setGround(column, row, tileType, tilemap);
                }
            }
        }
    }

    private static void setWall(int x, int y, Tilemap tilemap)
    {
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, null);
    }

    private static void setGround(int x, int y, Tile TileType, Tilemap tilemap)
    {
        GroundTile tile = ScriptableObject.CreateInstance<GroundTile>();
        tile.setTile((GroundTile)TileType);
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
    }

    private static void setWater(int x, int y, WaterTile wt, Tilemap tilemap)
    {
        WaterTile tile = ScriptableObject.CreateInstance<WaterTile>();
        tile.setTile(wt);
        Vector3Int pos = new Vector3Int(x, y, 0);
        tilemap.SetTile(pos, tile);
    }
}
