using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;

public class GroundTile : Tile
{
    public Sprite m_Sprite;
    public Sprite m_Preview;
    public Sprite[] animatedSprites;
    public GameObject fireEffect, freezeEffect;
    public List<Status> statusList;
    public bool isOnFire, isFreeze;

    public void setTile(GroundTile gt)
    {
        m_Sprite = gt.m_Sprite;
        m_Preview = gt.m_Preview;
        animatedSprites = gt.animatedSprites;
        fireEffect = gt.fireEffect;
        freezeEffect = gt.freezeEffect;
        statusList = gt.statusList;
        isOnFire = gt.isOnFire;
    }

    public void addStatus(Status status)
    {
        if (statusList == null)
        {
            statusList = new List<Status>();
        }
        statusList.Add(status);
    }

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

    public override void GetTileData(Vector3Int location, ITilemap tilemap, ref TileData tileData)
    {
        tileData.sprite = m_Sprite;
        if (isOnFire)
        {
            tileData.gameObject = fireEffect;
        }
        if (isFreeze)
        {
            tileData.gameObject = freezeEffect;
        }
        if (!isFreeze && !isOnFire)
        {
            tileData.gameObject = null;
        }
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
