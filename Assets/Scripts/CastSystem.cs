using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class CastSystem : MonoBehaviour
{
    private RangeUtils RangeUtils;

    private Tile threeSidesTile, threeSidesBottomTile, twoSidesLeftTile, twoSidesRightTile, transparent;

    private void Start()
    {
        RangeUtils = new RangeUtils();
        threeSidesTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_empty_three_sides_top_tile");
        threeSidesBottomTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_empty_three_sides_side_tile");
        twoSidesLeftTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_empty_two_sides_left_tile");
        twoSidesRightTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_empty_two_sides_right_tile");
        transparent = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile");
    }

    public bool canCast(Spell spell, Vector3Int cellPosition, Vector3Int playerPos, List<Vector3Int> area, Tilemap tilemap)
    {
        // Tile is empty
        if (!tilemap.HasTile(cellPosition))
        {
            return false;
        }
        // No spell selected
        if (spell == null)
        {
            return false;
        }
        // Check line of sight
        if (spell.lineOfSight && !RangeUtils.lineOfSight(playerPos, cellPosition, tilemap))
        {
            return false ;
        }
        // Check if click is in range
        if (!area.Contains(cellPosition))
        {
            return false;
        }
        return true;
    }

    public void castSpell(Spell selectedSpell, Vector3Int cellPosition, Vector3Int playerPos, Tilemap tilemap)
    {
        selectedSpell.playAnimation(cellPosition, playerPos, tilemap);
        // Todo : damage & effects
    }

    public void showSpellArea(Spell spell, Vector3Int to, Vector3Int from, Tilemap cellsGrid, Tilemap tilemap)
    {
        List<Vector3Int> area = spell.getArea(to, from, tilemap);
        // Red
        transparent.color = new Color(0.9f, 0.1f, 0.1f, 0.5f);
        RangeUtils.setTileOnTilemap(area, transparent, cellsGrid);
        // White
        transparent.color = new Color(1, 1, 1, 0.5f);
    }

    public void showSpellArea(Spell spell, Vector3Int to, Vector3Int from, Tilemap cellsGrid, Tilemap tilemap, Color color)
    {
        List<Vector3Int> area = spell.getArea(to, from, tilemap);
        // Red
        transparent.color = color;
        RangeUtils.setTileOnTilemap(area, transparent, cellsGrid);
        // White
        transparent.color = new Color(1, 1, 1, 0.5f);
    }

    public void showAreaSelected(Vector3Int clickedPos, Vector3Int cellPosition, Tilemap cellsGrid)
    {
        List<Vector3Int> line = RangeUtils.getLine(clickedPos, cellPosition);
        // Red
        transparent.color = new Color(0.9f, 0.1f, 0.1f, 0.5f);
        RangeUtils.setTileOnTilemap(line, transparent, cellsGrid);
        // White
        transparent.color = new Color(1, 1, 1, 0.5f);
    }

    // Todo : refacto
    public void showSpellRange(Spell spell, Vector3Int playerPos, Tilemap cellsGrid, Tilemap tilemap)
    {
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(playerPos, spell.range, cellsGrid);

        // Check for each square if there is line of sight
        listSquare.ForEach(s =>
        {
            if (RangeUtils.lineOfSight(playerPos, s, tilemap))
            {
                cellsGrid.SetTile(s, transparent);
            }
        });
    }

    public void showSpellRangeEmpty(Spell spell, Vector3Int playerPos, Tilemap tilemap)
    {
        // Get empty circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleEmpty(playerPos, spell.area, tilemap);

        // Check each square to orientate picture
        listSquare.ForEach(s =>
        {
            float x = s.x - playerPos.x;
            float y = s.y - playerPos.y;

            if (RangeUtils.getNbFarthestAdjSquares(tilemap, playerPos, s) == 3)
            {
                // Top
                if (x <= 0 && y > 0)
                {
                    tilemap.SetTile(s, threeSidesTile);
                }
                // Bottom
                else if (x <= 0 && y < 0)
                {
                    tilemap.SetTile(s, threeSidesTile);
                    tilemap.SetTransformMatrix(s, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180f)));
                }
                // Left
                else if (x < 0)
                {
                    tilemap.SetTile(s, threeSidesBottomTile);
                }
                // Right
                else
                {
                    tilemap.SetTile(s, threeSidesBottomTile);
                    tilemap.SetTransformMatrix(s, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180f)));
                }
            }
            else
            {
                // Bottom left 
                if (x < 0 && y < 0)
                {
                    tilemap.SetTile(s, twoSidesRightTile);
                    tilemap.SetTransformMatrix(s, Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180f)));
                }
                // Top Right
                else if (x > 0 && y > 0)
                {
                    tilemap.SetTile(s, twoSidesRightTile);
                }
                // Top left
                else if (x < 0)
                {
                    tilemap.SetTile(s, twoSidesLeftTile);
                }
                // Bottom right
                else
                {
                    tilemap.SetTile(s, twoSidesLeftTile);
                    tilemap.SetTransformMatrix(s, Matrix4x4.Rotate(Quaternion.Euler(0, 180f, 0)));
                }
            }
        });
    }


}
