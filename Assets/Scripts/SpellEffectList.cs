using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

public class SpellEffectList : MonoBehaviour
{
    public StatusList StatusList;

    private Tile obstacleTile;

    public SpellEffect
        CreateObstacle,
        PushFromPlayer,
        Fire,
        Freeze
        ;

    private RangeUtils RangeUtils;

    void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();

        // Get obstacles
        obstacleTile = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile");

        // Create Obstacle
        CreateObstacle = new SpellEffect("CreateObstacle");
        CreateObstacle.applyEffectAction = createObstacleEffect;

        // Push Outside Area of effect
        PushFromPlayer = new SpellEffect("PushFromPlayer");
        PushFromPlayer.applyEffectAction = pushFromPlayerEffect;

        // Apply Fire in a given area
        Fire = new SpellEffect("FireEffect");
        // Apply status to players
        Fire.statusList.Add(new Status(StatusList.Fire));
        // Apply status to tiles
        Fire.statusTileList.Add(new Status(StatusList.Fire));
        Fire.applyEffectAction = fireEffect;

        // Apply freeze in a given area
        Freeze = new SpellEffect("FreezeEffect");
        Freeze.statusList.Add(new Status(StatusList.Freeze));
        Freeze.statusTileList.Add(new Status(StatusList.Freeze));
        Freeze.applyEffectAction = freezeEffect;
    }

    public void fireEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Dictionary<Unit, GameObject> allCharacters = playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value);
        List<Vector3Int> area = spell.getArea(obstacleList, tilemap);
        // Check multiple effect can stack on same cell on a single cast
        if (!spellEffect.cumul)
        {
            area = area.Distinct().ToList();
        }

        area.ForEach(cell =>
        {
            // Apply status to obstacleList
            // obstacleList[cell];

            // Apply status to characters
            Unit character = getUnitFromPos(allCharacters, cell);
            if (character != null)
            {
                spellEffect.statusList.ForEach(status =>
                {
                    character.addStatus(new Status(status));
                });
            }
            // Apply status to tiles
            GroundTile tile = (GroundTile)tilemap.GetTile(cell);
            if (tile != null)
            {
                tile.isOnFire = true;
                spellEffect.statusTileList.ForEach(status =>
                {
                    tile.addStatus(new Status(status));
                });
            }
        });
    }

    public void freezeEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Dictionary<Unit, GameObject> allCharacters = playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value);
        List<Vector3Int> area = spell.getArea(obstacleList, tilemap);
        // Check multiple effect can stack on same cell on a single cast
        if (!spellEffect.cumul)
        {
            area = area.Distinct().ToList();
        }

        area.ForEach(cell =>
        {
            // Apply status to obstacleList
            // obstacleList[cell];

            // Apply status to characters
            Unit character = getUnitFromPos(allCharacters, cell);
            if (character != null)
            {
                spellEffect.statusList.ForEach(status =>
                {
                    character.addStatus(new Status(status));
                });
            }
            // Apply status to tiles
            GroundTile tile = (GroundTile)tilemap.GetTile(cell);
            if (tile != null)
            {
                tile.isFreeze = true;
                spellEffect.statusTileList.ForEach(status =>
                {
                    tile.addStatus(new Status(status));
                });
            }
        });
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
            unitPlayer.position = RangeUtils.getFarthestWalkableNeighbour(
                cell,
                spell.casterPos,
                area, obstacleList,
                tilemap: tilemap
            );
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
