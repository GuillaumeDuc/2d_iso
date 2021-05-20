using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

public class CastSystem : MonoBehaviour
{
    public InfoScrollView EnemiesScrollView;

    public InfoScrollView PlayersScrollView;

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

    public CastState cast(
        Spell spell,
        Unit player,
        Vector3Int cellClicked,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        CastState currentState,
        Tilemap tilemap,
        Tilemap cellsGrid
        )
    {
        RangeUtils.removeCells(cellsGrid);
        if (currentState == CastState.SHOW_AREA)
        {
            if (!spell.canCast(player, cellClicked, obstacleList, tilemap))
            {
                spell.spellPos.Clear();
                return CastState.DEFAULT;
            }

            spell.spellPos.Add(cellClicked);

            // Orange color
            showArea(spell.spellPos, cellsGrid, new Color(1, 0.5f, 0, 0.5f));
            // Red color
            showArea(spell.getArea(obstacleList, tilemap), cellsGrid, new Color(0.9f, 0.1f, 0.1f, 0.5f));

            if (!(spell.spellPos.Count() == spell.clickNb))
            {
                showArea(spell.getRange(player, obstacleList, tilemap), cellsGrid);
            }

            if (spell.spellPos.Count() == spell.clickNb)
            {
                RangeUtils.removeCells(cellsGrid);
                showArea(spell.getArea(obstacleList, tilemap), cellsGrid, new Color(0.9f, 0.1f, 0.1f, 0.5f));
                return CastState.CAST_SPELL;
            }
        }

        if (currentState == CastState.CAST_SPELL)
        {
            // If spell area is clear & mana is enough
            if (spell.getArea(obstacleList, tilemap).Contains(cellClicked) && player.currentMana >= spell.manaCost)
            {
                castSpell(spell, player, playerList, enemyList, obstacleList, tilemap);
            }
            spell.spellPos.Clear();
            return CastState.DEFAULT;
        }

        return currentState;
    }

    public void castSpell(
        Spell spell,
        Unit player,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        spell.doDamage(playerList, enemyList, obstacleList, tilemap);
        spell.applyEffect(player, playerList, enemyList, obstacleList, tilemap);
        spell.playAnimation(obstacleList, tilemap);
        player.currentMana -= spell.manaCost;
        updateScrollViews(playerList, enemyList);
    }

    public void updateScrollViews(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList
        )
    {
        EnemiesScrollView.updateScrollView();
        PlayersScrollView.updateScrollView();
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
