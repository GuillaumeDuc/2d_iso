using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileList : MonoBehaviour
{
    public GroundTile brick, grass;
    void Start()
    {
        // Brick Tile
        brick = Resources.Load<GroundTile>("ground");
        Sprite brickSprite = Resources.Load<Sprite>("Tilemaps/Brick/brick_short_tile_iso");
        brick.m_Sprite = brickSprite;
        brick.m_Preview = brickSprite;

        // Grass tile
        grass = Resources.Load<GroundTile>("ground");
    }
}
