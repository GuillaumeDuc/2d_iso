using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;
using System.Linq;

public class SpellEffectList : MonoBehaviour
{
    public StatusList StatusList;

    public TileList TileList;


    public static SpellEffect PushFromPlayer = new SpellEffect("PushFromPlayer"),
        Fire = new SpellEffect("FireEffect"),
        Freeze = new SpellEffect("FreezeEffect"),
        Teleport = new SpellEffect("TeleportEffect"),
        BurnTile = new SpellEffect("BurnTileEffect"),
        FireBurst = new SpellEffect("FireBurstEffect"),
        WaterEffect = new SpellEffect("WaterEffect");

    public static List<SpellEffect> spellEffects = new List<SpellEffect>() {
        PushFromPlayer,
        Fire,
        Freeze,
        Teleport,
        BurnTile,
        FireBurst,
        WaterEffect
    };

    void Start()
    {
        // Push Outside Area of effect
        PushFromPlayer.applyEffectAction = pushFromPlayerEffect;

        // Apply Fire in a given area
        // Apply status to players
        Fire.statusList.Add(new Status(StatusList.Fire));
        // Apply status to tiles
        Fire.statusTileList.Add(new Status(StatusList.Fire));
        Fire.applyEffectAction = applyEffect;

        // Apply freeze in a given area
        Freeze.statusList.Add(new Status(StatusList.Freeze));
        Freeze.statusTileList.Add(new Status(StatusList.Freeze));
        Freeze.applyEffectAction = applyEffect;

        // Teleport from a point to another point
        Teleport.applyEffectAction = teleportPlayerEffect;

        // Change Tile
        BurnTile.applyEffectAction = burnTileEffect;

        // Get all fire status, remove them and deal damage
        FireBurst.applyEffectAction = fireBurstEffect;

        // Water effect when in water
        WaterEffect.statusList.Add(new Status(StatusList.Wet));
        WaterEffect.applyEffectAction = waterEffect;
    }

    public void waterEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        applyEffect(spell, spellEffect, playerList, enemyList, obstacleList, tilemap);
        GameObject WaterObstacle = Resources.Load<GameObject>("Obstacles/Water/WaterObstacle");
        List<Vector3Int> area = spell.getArea(spell.position, spell.caster, obstacleList, tilemap);
        area.ForEach(cell =>
        {
            createObstacle(WaterObstacle, cell, obstacleList, tilemap);
        });
    }

    public void createObstacle(GameObject obstacle, Vector3Int pos, Dictionary<Vector3Int, GameObject> obstacleList, Tilemap tilemap)
    {
        GameObject newObstacle = null;
        try
        {
            newObstacle = Instantiate(obstacle, tilemap.CellToWorld(pos), Quaternion.identity);
            FightingSceneStore.obstacleList.Add(pos, newObstacle);
        }
        catch
        {
            Destroy(newObstacle);
        }
    }

    public void fireBurstEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = spell.getArea(spell.position, spell.caster, obstacleList, tilemap);
        int count = 0;
        // Count Fire Status
        area.ForEach(a =>
        {
            // Status Tile
            GroundTile tile = (GroundTile)tilemap.GetTile(a);
            if (tile != null && tile.statusList != null)
            {
                // Find Temperature equals or greater than Fire
                Status statusOnTile = tile.statusList.Find(status => status.Equals(StatusList.Fire) && status.weight >= StatusList.Fire.weight);
                if (statusOnTile != null)
                {
                    // Count Damage && delete Fire effect
                    count += statusOnTile.weight;
                    tile.removeStatus(statusOnTile);
                    tilemap.RefreshTile(a);
                }
            }
            // Status Units
            Unit character = getUnitFromPos(playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value), a);
            if (character != null)
            {
                // Find Temperature equals or greater than Fire
                Status statusOnUnit = character.statusList.Find(status => status.Equals(StatusList.Fire) && status.weight >= StatusList.Fire.weight);
                if (statusOnUnit != null)
                {
                    // Count Damage && delete Fire effect
                    count += statusOnUnit.weight;
                    character.statusList.Remove(statusOnUnit);
                }
            }
            // Status Obstacles
            GameObject obstacleGO = null;
            try { obstacleGO = obstacleList[a]; }
            catch { }
            if (obstacleGO != null)
            {
                Obstacle obstacle = obstacleGO.GetComponent<Obstacle>();
                // Find Temperature equals or greater than Fire
                Status statusOnObstacle = obstacle.statusList.Find(status => status.Equals(StatusList.Fire) && status.weight >= StatusList.Fire.weight);
                if (statusOnObstacle != null)
                {
                    // Count Damage && delete Fire effect
                    count += statusOnObstacle.weight;
                    obstacle.statusList.Remove(statusOnObstacle);
                }
            }
        });
        // Change damage
        spell.damage += count;
    }

    public void burnTileEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = spell.getArea(spell.position, spell.caster, obstacleList, tilemap);

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
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Dictionary<Unit, GameObject> allCharacters = playerList.Concat(enemyList).ToDictionary(x => x.Key, x => x.Value);
        List<Vector3Int> area = spell.getArea(spell.position, spell.caster, obstacleList, tilemap);
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
            tilemap.RefreshTile(cell);
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
        List<Vector3Int> area = spell.getArea(spell.position, spell.caster, obstacleList, tilemap);
        // Move every ennemies and players one cell away
        area.ForEach(a =>
        {
            // Players
            movePlayer(spell, playerList, a, area, obstacleList, tilemap);
            // Enemies
            movePlayer(spell, enemyList, a, area, obstacleList, tilemap);
        });
    }

    private void teleportPlayerEffect(
        Spell spell,
        SpellEffect spellEffect,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        KeyValuePair<Unit, GameObject> player = getUnitFromPlayersAndEnemies(playerList, enemyList, spell.caster);
        Vector3 originalScale = player.Value.transform.localScale;

        // Make player disappear
        StartCoroutine(scaleGO(player.Value, originalScale.x, 0f, originalScale));
        // Get cell to world position
        Vector3 cellPos = tilemap.CellToWorld(spell.position);
        cellPos.y += 0.2f;
        // Move unit to position
        player.Key.position = spell.position;
        // Move gameobject to position
        player.Value.transform.position = cellPos;
        // Make player reappear
        StartCoroutine(scaleGO(player.Value, 0f, originalScale.x, originalScale));
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
                spell.caster.position,
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
