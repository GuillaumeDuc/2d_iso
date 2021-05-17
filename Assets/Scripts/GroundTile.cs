using UnityEngine;
using System.Collections;
using UnityEngine.Tilemaps;
using UnityEditor;
using System.Collections.Generic;
using System.Linq;

public class GroundTile : Tile
{
    public Sprite m_Sprite;
    public Sprite m_Preview;
    public Sprite[] animatedSprites;
    public GameObject tileGO;
    public List<Status> statusList;

    public virtual void setTile(GroundTile gt)
    {
        m_Sprite = gt.m_Sprite;
        m_Preview = gt.m_Preview;
        animatedSprites = gt.animatedSprites;
        tileGO = gt.tileGO;
        // create new status list
        if (gt.statusList != null)
        {
            statusList = getNewStatusList(gt.statusList);
        }
    }

    public List<Status> getNewStatusList(List<Status> statusList)
    {
        List<Status> res = new List<Status>();
        statusList.ForEach(a => { res.Add(new Status(a)); });
        return res;
    }

    public void addStatus(Status status)
    {
        if (statusList == null)
        {
            statusList = new List<Status>();
        }
        Status newStatus = new Status(status);
        statusList = newStatus.addStatusToList(statusList);
        // Change GameObject
        statusList.ForEach(s =>
        {
            tileGO = s.tileGO;
        });
        // If list empty, no GameObject
        if (!statusList.Any())
        {
            tileGO = null;
        }
    }

    public bool containsOnlyPermanentStatus(List<Status> statusList)
    {
        bool contains = true;
        statusList.ForEach(s=> {
            if (!s.permanentOnTile)
            {
                contains = false;
            }
        });
        return contains;
    }

    public void updateStatus()
    {
        if (statusList != null)
        {
            List<Status> newStatusList = new List<Status>();
            statusList.ForEach(s =>
            {
                bool continueStatus = (s.updateStatus() || s.permanentOnTile);
                if (continueStatus)
                {
                    newStatusList.Add(s);
                }
            });
            statusList = newStatusList;
            // If list empty or contains only permanent status, no GameObject
            if (!statusList.Any() || containsOnlyPermanentStatus(statusList))
            {
                tileGO = null;
            }
        }
    }

    public override void RefreshTile(Vector3Int position, ITilemap tilemap)
    {
        base.RefreshTile(position, tilemap);
    }

    public override bool GetTileAnimationData(Vector3Int position, ITilemap tilemap, ref TileAnimationData tileAnimationData)
    {
        if (animatedSprites != null && animatedSprites.Length > 0)
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
        tileData.gameObject = tileGO;
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
