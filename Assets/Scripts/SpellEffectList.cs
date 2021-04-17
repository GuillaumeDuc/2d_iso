using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class SpellEffectList : MonoBehaviour
{
    private Tile obstacleTile;

    public SpellEffect
        CreateObstacle,
        PushFromPlayer
        ;

    private RangeUtils RangeUtils;

    void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();

        // Get obstacles
        obstacleTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile");

        // Create Obstacle
        CreateObstacle = new SpellEffect();
        CreateObstacle.applyEffectAction = createObstacleEffect;

        // Push Outside Area of effect
        PushFromPlayer = new SpellEffect();
        PushFromPlayer.applyEffectAction = pushFromPlayerEffect;
    }

    public void createObstacleEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        spell.getArea(obstacleList, tilemap).ForEach(c =>
        {
            try { obstacleList.Add(c, spell.spellGO); }
            catch { }
        });
    }

    public void pushFromPlayerEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = spell.getArea(obstacleList, tilemap);
        // Move every ennemies and players one cell away
        area.ForEach(a =>
        {
            // Players
            movePlayer(spell, playerList, a, area, obstacleList, tilemap);
            // Enemies
            movePlayer(spell, enemyList, a, area, obstacleList, tilemap);
        });
    }

    private string printList(List<Vector3Int> list)
    {
        string res = "";
        list.ForEach(a =>
        {
            res += a + "\n";
        });
        return res;
    }

    private string printDict(Dictionary<Unit, GameObject> dict)
    {
        string res = "";
        foreach (var d in dict)
        {
            res += d + "\n";
        }
        return res;
    }

    private void movePlayer(
        Spell spell,
        Dictionary<Unit, GameObject> dict,
        Vector3Int cell,
        List<Vector3Int> area,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Unit unitPlayer = getUnitFromPos(dict, cell);
        if (unitPlayer != null)
        {
            GameObject playerGO = dict[unitPlayer];
            dict.Remove(unitPlayer);
            unitPlayer.position = RangeUtils.getFarthestWalkableNeighbour(cell, spell.casterPos, area, obstacleList, tilemap);
            dict.Add(unitPlayer, playerGO);

            moveGameObject(playerGO, unitPlayer.position, tilemap);
        }
    }

    private void moveGameObject(GameObject gameObject, Vector3Int cell, Tilemap tilemap)
    {
        // Move GameObjects
        Vector2 pos = new Vector2(tilemap.CellToWorld(cell).x, tilemap.CellToWorld(cell).y + 0.2f);
        gameObject.transform.position = pos;
    }

    private Unit getUnitFromPos(Dictionary<Unit, GameObject> dict, Vector3Int cell)
    {
        foreach (var d in dict)
        {
            if (d.Key.position == cell)
            {
                return d.Key;
            }
        }
        return null;
    }
}
