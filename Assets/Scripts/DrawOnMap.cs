using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class DrawOnMap : MonoBehaviour
{
    public TurnBasedSystem TurnBasedSystem;
    public Tilemap cellsGrid;

    private RangeUtils RangeUtils;
    private Tile transparentTile;
    public Color orange, white, red, black, blue, green;
    private float transparency = 0.5f;

    void Start()
    {
        RangeUtils = new RangeUtils();
        transparentTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile");
        orange = new Color(1, 0.5f, 0, transparency);
        white = new Color(1, 1, 1, transparency);
        red = new Color(0.9f, 0.1f, 0.1f, transparency);
        blue = new Color(0.4f, 1f, 1f, transparency);
        black = new Color(0f, 0f, 0f, transparency);
        green = new Color(0f, 0.9f, 0f, transparency);
    }

    public void drawCharactersPosition()
    {
        // Enemies in black
        drawPosition(TurnBasedSystem.enemyList, black);
        // Players in blue
        drawPosition(TurnBasedSystem.playerList, blue);
    }

    public void drawPosition(Dictionary<Unit, GameObject> list, Color color)
    {
        transparentTile.color = color;
        List<Vector3Int> posList = new List<Vector3Int>(
            list.Select(unit => unit.Key.position).ToList()
        );
        setTileOnTilemap(posList);
        // Back to white
        transparentTile.color = white;
    }

    public void showSpellSelection(List<Vector3Int> spellPos, List<Vector3Int> area)
    {
        resetMap();

        // Spell area of effect & clicks supersede character pos
        transparentTile.color = red;
        forceSetTile(area);

        transparentTile.color = orange;
        forceSetTile(spellPos);

        // Back to white
        transparentTile.color = white;
    }

    public void showRange(List<Vector3Int> area, bool remove = false)
    {
        showRange(area, white, remove);
    }

    public void showRange(List<Vector3Int> area, Color color, bool remove = false)
    {
        if (remove)
        {
            resetMap();
        }
        transparentTile.color = color;
        setTileOnTilemap(area);
        transparentTile.color = white;
    }

    public void showMovement(List<Vector3Int> area)
    {
        resetMap();
        transparentTile.color = green;
        forceSetTile(area);
        transparentTile.color = white;
    }

    public void showSpellArea(List<Vector3Int> area)
    {
        resetMap();

        transparentTile.color = red;
        // Spell area of effect supersede character pos
        forceSetTile(area);
        transparentTile.color = white;
    }

    public void setTileOnTilemap(List<Vector3Int> listSquare)
    {
        listSquare.ForEach(s =>
        {
            cellsGrid.SetTile(s, transparentTile);
        });
    }

    public void forceSetTile(List<Vector3Int> list)
    {
        list.ForEach(s =>
        {
            if (cellsGrid.GetTile(s) == null)
            {
                cellsGrid.SetTile(s, transparentTile);
            }
            else
            {
                cellsGrid.SetTile(s, null);
                cellsGrid.SetTile(s, transparentTile);
            }
        });
    }

    public void resetMap()
    {
        // Remove previous Tile
        removeAllTile();
        // Always redraw characters position
        drawCharactersPosition();
    }

    public void removeAllTile()
    {
        foreach (var a in cellsGrid.cellBounds.allPositionsWithin)
        {
            cellsGrid.SetTile(a, null);
        }
    }
}
