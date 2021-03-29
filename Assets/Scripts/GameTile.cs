using UnityEngine;
using UnityEngine.Tilemaps;

public class TileBrick : TileBase
{
    public Sprite spriteA;
    public Sprite spriteB;

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        bool evenCell = Mathf.Abs(position.y + position.x) % 2 > 0;
        tileData.sprite = evenCell ? spriteA : spriteB;
    }
}