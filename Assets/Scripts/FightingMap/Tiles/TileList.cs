using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class TileList : MonoBehaviour
{
    public GroundTile brick, grass, sand, burnt;
    public WaterTile water;
    public StatusList StatusList;
    public Tilemap tilemap;

    void Start()
    {
        // Brick Tile
        brick = ScriptableObject.CreateInstance<GroundTile>();
        Sprite brickSprite = Resources.Load<Sprite>("Tilemaps/Brick/brick_short_tile_iso");
        brick.sprite = brickSprite;
        brick.m_Preview = brickSprite;
        brick.name = "Brick";

        // Grass Tile
        grass = ScriptableObject.CreateInstance<GroundTile>();
        Sprite grassSprite = Resources.Load<Sprite>("Tilemaps/Grass/grass_elevated_tile_iso");
        grass.sprite = grassSprite;
        grass.m_Preview = grassSprite;
        grass.name = "Grass";

        // Sand Tile
        sand = ScriptableObject.CreateInstance<GroundTile>();
        Sprite sandSprite = Resources.Load<Sprite>("Tilemaps/Sand/sand_elevated_tile_iso");
        sand.sprite = sandSprite;
        sand.m_Preview = sandSprite;
        sand.name = "Sand";

        // Burnt Tile
        burnt = ScriptableObject.CreateInstance<GroundTile>();
        Sprite burntSprite = Resources.Load<Sprite>("Tilemaps/Burnt/burnt_tile_iso");
        burnt.sprite = burntSprite;
        burnt.m_Preview = burntSprite;
        burnt.name = "Burnt";

        // Water Tile
        water = ScriptableObject.CreateInstance<WaterTile>();
        Sprite waterSprite = Resources.Load<Sprite>("Tilemaps/Water/water_tile_full_iso");
        GameObject waterGO = Resources.Load<GameObject>("Tilemaps/Water/water_tile_cartoon_iso");
        water.sprite = null;
        water.m_Preview = waterSprite;
        water.defaultGO = waterGO;
        water.name = "Water";
        water.currentTilemap = tilemap;
        water.walkable = false;
        water.defaultWalkable = false;
        water.addStatus(new Status(StatusList.Wet));
    }
}
