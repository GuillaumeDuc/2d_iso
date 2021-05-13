using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileList : MonoBehaviour
{
    public GroundTile brick, water;
    void Start()
    {
        // Brick Tile
        brick = Resources.Load<GroundTile>("ground");
        Sprite brickSprite = Resources.Load<Sprite>("Tilemaps/Brick/brick_short_tile_iso");
        brick.m_Sprite = brickSprite;
        brick.m_Preview = brickSprite;
        brick.name = "Brick";

        // Water Tile
        water = ScriptableObject.CreateInstance<GroundTile>();
        Sprite waterSprite = Resources.Load<Sprite>("Tilemaps/Water/water_tile_full_iso");
        GameObject waterGO = Resources.Load<GameObject>("Tilemaps/Water/water_tile_iso");
        water.m_Sprite = null;
        water.m_Preview = waterSprite;
        water.tileGO = waterGO;
        water.name =  "Water";
    }
}
