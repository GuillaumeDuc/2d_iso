using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

public class SpellEffectList : MonoBehaviour
{
    public StatusList StatusList;

    public TileList TileList;

    public SpellEffect
        CreateObstacle,
        PushFromPlayer,
        Fire,
        Freeze,
        Teleport,
        BurnTile
        ;

    void Start()
    {
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
        Fire.applyEffectAction = applyEffect;

        // Apply freeze in a given area
        Freeze = new SpellEffect("FreezeEffect");
        Freeze.statusList.Add(new Status(StatusList.Freeze));
        Freeze.statusTileList.Add(new Status(StatusList.Freeze));
        Freeze.applyEffectAction = applyEffect;

        // Teleport from a point to another point
        Teleport = new SpellEffect("TeleportEffect");
        Teleport.applyEffectAction = teleportPlayerEffect;

        // Change Tile
        BurnTile = new SpellEffect("ChangeTileEffect");
        BurnTile.applyEffectAction = burnTileEffect;
    }

    public void burnTileEffect(
        Spell spell,
        Unit caster,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = spell.getArea(caster, obstacleList, tilemap);

        area.ForEach(a =>
        {
            TileBase t = tilemap.GetTile(a);
            if (t != null && !(t is WaterTile))
            {
                GroundTile burnt = ScriptableObject.CreateInstance<GroundTile>();
                burnt.setTile(TileList.burnt);
                tilemap.SetTile(a, burnt);
            }
        });
    }

    public void applyEffect(
        Spell spell,
        Unit caster,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Dictionary<Unit, GameObject> allCharacters = playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value);
        List<Vector3Int> area = spell.getArea(caster, obstacleList, tilemap);
        // Check multiple effect can stack on same cell on a single cast
        if (!spellEffect.cumul)
        {
            area = area.Distinct().ToList();
        }

        area.ForEach(cell =>
        {
            // Apply status to obstacleList
            GameObject obstacleGO = null;
            try { obstacleGO = obstacleList[cell]; }
            catch { }
            if (obstacleGO != null)
            {
                Obstacle obstacle = obstacleGO.GetComponent<Obstacle>();
                spellEffect.statusList.ForEach(status =>
                {
                    obstacle.addStatus(new Status(status));
                });
            }

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
                spellEffect.statusTileList.ForEach(status =>
                {
                    tile.addStatus(new Status(status));
                });
            }
        });
    }

    public void createObstacleEffect(
        Spell spell,
        Unit caster,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = spell.getArea(caster, obstacleList, tilemap);
        // Fill Obstacles
        area.ForEach(c =>
        {
            try
            {
                obstacleList.Add(c, spell.spellGO);
            }
            catch { }
        });
        // Instantiate obstacle
        StartCoroutine(animateOnOneCell(spell, area, tilemap));
    }

    IEnumerator animateOnOneCell(
        Spell spell,
        List<Vector3Int> area,
        Tilemap tilemap
        )
    {
        foreach (var c in area)
        {
            yield return new WaitForSeconds(0.2f);
            Vector2 worldPos = tilemap.CellToWorld(c);
            // Instantiate animation
            Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        }
    }

    public void pushFromPlayerEffect(
        Spell spell,
        Unit caster,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = spell.getArea(caster, obstacleList, tilemap);
        // Move every ennemies and players one cell away
        area.ForEach(a =>
        {
            // Players
            movePlayer(spell, caster, playerList, a, area, obstacleList, tilemap);
            // Enemies
            movePlayer(spell, caster, enemyList, a, area, obstacleList, tilemap);
        });
    }

    private void teleportPlayerEffect(
        Spell spell,
        Unit caster,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        KeyValuePair<Unit, GameObject> player = getUnitFromPlayersAndEnemies(playerList, enemyList, caster);
        Vector3 originalScale = player.Value.transform.localScale;
        spell.spellPos.ForEach(pos =>
        {
            // Make player disappear
            StartCoroutine(scaleGO(player.Value, originalScale.x, 0f, originalScale));
            // Get cell to world position
            Vector3 cellPos = tilemap.CellToWorld(pos);
            cellPos.y += 0.2f;
            // Move unit to position
            player.Key.position = pos;
            // Move gameobject to position
            player.Value.transform.position = cellPos;
            // Make player reappear
            StartCoroutine(scaleGO(player.Value, 0f, originalScale.x, originalScale));
        });
    }

    IEnumerator scaleGO(GameObject go, float from, float to, Vector3 originalScale)
    {
        float i = from;
        while (!i.Equals(to))
        {
            yield return new WaitForSeconds(0.01f);
            go.transform.localScale = new Vector3(i, originalScale.y, originalScale.z);
            if (i < to)
            {
                // Add one tenth of the goal
                float step = to / 10;
                i = (float)System.Math.Round(i + step, 3);
            }
            else
            {
                float step = from / 10;
                i = (float)System.Math.Round(i - step, 3);
            }
        }
        go.transform.localScale = new Vector3(to, originalScale.y, originalScale.z);
    }

    private void movePlayer(
    Spell spell,
    Unit caster,
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
                caster.position,
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

    private KeyValuePair<Unit, GameObject> getUnitFromPlayersAndEnemies(
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Unit unit
        )
    {
        Dictionary<Unit, GameObject> allList = new Dictionary<Unit, GameObject>(
            playerList
            .Concat(enemyList)
            .ToDictionary(x => x.Key, x => x.Value)
        );
        return allList.First(a => a.Key == unit);
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
