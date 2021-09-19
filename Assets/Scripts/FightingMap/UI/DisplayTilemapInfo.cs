using UnityEngine.Tilemaps;
using UnityEngine.UI;
using UnityEngine;

public class DisplayTilemapInfo : MonoBehaviour
{
    public Text InfoPanel;
    private Vector3 previousMousePos = new Vector3Int();

    void Update()
    {
        Vector3 mousePos = Input.mousePosition;
        if (!mousePos.Equals(previousMousePos))
        {
            Vector2 worldPosition = Camera.main.ScreenToWorldPoint(mousePos);
            Vector3Int cellPosition = FightingSceneStore.tilemap.WorldToCell(worldPosition);
            GroundTile tile = (GroundTile)FightingSceneStore.tilemap.GetTile(cellPosition);
            if (tile != null)
            {
                string s = "Tile position :\n" + cellPosition + "\n";
                s += "Status List \n";
                tile.statusList?.ForEach(status =>
                {
                    s += status + " | " + "Permanent : " + status.permanentOnTile + "\n";
                });
                s += "Block sight : " + !tile.lineOfSight + "\n";
                s += "Block walk : " + !tile.walkable + "\n";
                InfoPanel.text = s;
            }
        }
    }
}
