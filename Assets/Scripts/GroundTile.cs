using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;

public class GroundTile : Tile
{
    public Sprite[] animatedSprites;
    public Sprite m_Preview;

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        if (animatedSprites.Length > 0)
        {
            tileAnimationData.animatedSprites = animatedSprites;
            tileAnimationData.animationSpeed = 1;
            return true;
        }
        return false;
    }

    public override void GetTileData(Vector3Int position, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = animatedSprites[0];
        base.GetTileData(position, tilemap, ref tileData);
    }
#if UNITY_EDITOR
    [MenuItem("Assets/Create/GroundTile")]
    public static void CreateBrickTile()
    {
        string path = EditorUtility.SaveFilePanelInProject("Save Ground Tile", "GroundTile", "Asset", "Save Ground Tile", "Assets");
        if (path == "")
            return;
        AssetDatabase.CreateAsset(ScriptableObject.CreateInstance<GroundTile>(), path);
    }
#endif
}
