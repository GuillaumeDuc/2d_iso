using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;

public class TileList : MonoBehaviour
{
    public GroundTile brick, grass;
    // Start is called before the first frame update
    void Start()
    {
        // Tile Effects
        GameObject fireEffect = Resources.Load<GameObject>("TileEffects/Fire/FireEffect");
        GameObject freezeEffect = Resources.Load<GameObject>("TileEffects/Freeze/FreezeEffect");

        // Brick Tile
        brick = Resources.Load<GroundTile>("ground");
        Sprite brickSprite = Resources.Load<Sprite>("Tilemaps/Brick/brick_short_tile_iso");
        brick.m_Sprite = brickSprite;
        brick.m_Preview = brickSprite;
        brick.fireEffect = fireEffect;
        brick.freezeEffect = freezeEffect;

        // Grass tile
        grass = Resources.Load<GroundTile>("ground");
    }
}
