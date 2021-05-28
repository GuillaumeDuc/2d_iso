using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class WaterTile : GroundTile
{
    public Tilemap currentTilemap;
    public GameObject defaultGO;
    public GameObject previousTileGO;
    public string previousNameGO;

    public void setTile(WaterTile gt)
    {
        base.setTile(gt);
        currentTilemap = gt.currentTilemap;
        defaultGO = gt.defaultGO;
    }

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        if (tileGO != null)
        {
            tileData.gameObject = defaultGO;
            // Instantiate new GO when previous is different than current
            if (previousNameGO != tileGO.name)
            {
                if (previousTileGO != null)
                {
                    Destroy(previousTileGO);
                }
                Vector3 pos = currentTilemap.CellToWorld(location);
                previousTileGO = Instantiate(
                    tileGO,
                    new Vector3(pos.x, pos.y + 0.19f, pos.z),
                    Quaternion.identity
                );
                previousNameGO = tileGO.name;
            }
        }
        else
        {
            if (previousTileGO != null)
            {   
                // Reset
                Destroy(previousTileGO);
                previousTileGO = null;
                previousNameGO = "";
            }
            tileData.gameObject = defaultGO;
        }
    }

}
