using UnityEngine;
using UnityEngine.Tilemaps;

public static class GenerateGround
{
    public static void fillMap(int width, int height, Tile tile, Tilemap tilemap)
    {
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                setGround(x, y, tile, tilemap);
            }
        }
    }

    public static void setGround(int x, int y, Tile tile, Tilemap tilemap)
    {
        if (tile is WaterTile)
        {
            WaterTile wt = ScriptableObject.CreateInstance<WaterTile>();
            wt.setTile((WaterTile)tile);
            Vector3Int pos = new Vector3Int(x, y, 0);
            tilemap.SetTile(pos, wt);

        }
        else
        {
            GroundTile gt = ScriptableObject.CreateInstance<GroundTile>();
            gt.setTile((GroundTile)tile);
            Vector3Int pos = new Vector3Int(x, y, 0);
            tilemap.SetTile(pos, gt);
        }
    }
}
