using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class TileList : MonoBehaviour
{
    public GroundTile brick;
    public WaterTile water;
    public StatusList StatusList;
    public Tilemap tilemap;

    void Start()
    {
        // Brick Tile
        brick = Resources.Load<GroundTile>("ground");
        Sprite brickSprite = Resources.Load<Sprite>("Tilemaps/Brick/brick_short_tile_iso");
        brick.m_Sprite = brickSprite;
        brick.m_Preview = brickSprite;
        brick.name = "Brick";

        // Water Tile
        water = ScriptableObject.CreateInstance<WaterTile>();
        Sprite waterSprite = Resources.Load<Sprite>("Tilemaps/Water/water_tile_full_iso");
        GameObject waterGO = Resources.Load<GameObject>("Tilemaps/Water/water_tile_cartoon_iso");
        water.m_Sprite = null;
        water.m_Preview = waterSprite;
        water.defaultGO = waterGO;
        water.name = "Water";
        water.currentTilemap = tilemap;
        water.addStatus(new Status(StatusList.Wet));
    }
}
