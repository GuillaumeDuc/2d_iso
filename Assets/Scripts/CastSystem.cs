using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

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

    public bool canCast(Spell spell, Vector3Int cellPosition, Vector3Int playerPos, List<Vector3Int> range, Tilemap tilemap)
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
            return false;
        }
        // Check if click is in range
        if (!range.Contains(cellPosition))
        {
            return false;
        }
        return true;
    }

    public CastState cast(Spell spell, Unit player, Vector3Int cellClicked, CastState currentState, Tilemap tilemap, Tilemap cellsGrid)
    {
        RangeUtils.removeCells(cellsGrid);
        if (currentState == CastState.SHOW_AREA)
        {
            spell.casterPos = player.position;

            if (!canCast(spell, cellClicked, spell.casterPos, spell.getRange(tilemap), tilemap)){
                return CastState.DEFAULT;
            }

            spell.spellPos.Add(cellClicked);

            showArea(spell.getArea(tilemap), cellsGrid, new Color(0.9f, 0.1f, 0.1f, 0.5f));

            if (!(spell.spellPos.Count() == spell.clickNb))
            {
                showArea(spell.getRange(tilemap), cellsGrid);
            }

            if (spell.spellPos.Count() == spell.clickNb)
            {
                return CastState.CAST_SPELL;
            }
        }

        if (currentState == CastState.CAST_SPELL)
        {
            if (canCast(spell, cellClicked, spell.casterPos, spell.getArea(tilemap), tilemap))
            {
                castSpell(spell, player, tilemap);
            }
            spell.spellPos.Clear();
            return CastState.DEFAULT;
        }

        return currentState;
    }

    public void castSpell(Spell spell, Unit player, Tilemap tilemap)
    {
        spell.playAnimation(tilemap);
        // Todo : damage & effects
    }

    public void showArea(List<Vector3Int> area, Tilemap cellsGrid)
    {
        RangeUtils.setTileOnTilemap(area, transparent, cellsGrid);
    }

    public void showArea(List<Vector3Int> area, Tilemap cellsGrid, Color color)
    {
        transparent.color = color;
        RangeUtils.setTileOnTilemap(area, transparent, cellsGrid);
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
