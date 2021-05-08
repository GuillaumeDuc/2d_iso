using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/SpellsPrefab/";

    public SpellEffectList SpellEffectList;

    public Spell
        Explosion,
        Icycle,
        Sandwall
        ;

    private static GameObject
        ExplosionGO,
        IcycleGO,
        SandwallGO
        ;

    private string nameSpell;

    private RangeUtils RangeUtils;

    void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();

        // Explosion
        nameSpell = "Explosion";
        ExplosionGO = Resources.Load<GameObject>(PATH + nameSpell);
        // GameObject, name, damage, range, area, line of sight, click nb, unique cell area
        Explosion = new Spell(ExplosionGO, nameSpell, 20, 8, 2, true, 2);
        Explosion.getRangeList = getRangeInCircleFullPlayer;
        Explosion.getAreaList = getAreaInCircleFull;
        Explosion.animate = animateInCircleFull;
        Explosion.canCastOn = canCast;
        // Effects
        Explosion.spellEffectList.Add(SpellEffectList.Fire);
        // Damage
        Explosion.doDamageAction = doDamage;

        // Icycle
        nameSpell = "Icycle";
        IcycleGO = Resources.Load<GameObject>(PATH + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 30, 6, 1, false, 3);
        Icycle.getRangeList = getRangeInCircleFullPlayer;
        Icycle.getAreaList = getAreaSingleCell;
        Icycle.animate = animateOnCell;
        Icycle.canCastOn = canCast;
        // Effects
        Icycle.spellEffectList.Add(SpellEffectList.Freeze);
        // Damage
        Icycle.doDamageAction = doDamage;

        // Sandwall
        nameSpell = "Sandwall";
        SandwallGO = Resources.Load<GameObject>(PATH + nameSpell);
        Sandwall = new Spell(SandwallGO, nameSpell, 0, 10, 1, false, 2, true);
        Sandwall.getRangeList = getRangeInCircleFullPlayer;
        Sandwall.getAreaList = getAreaInLineBetweenCells;
        Sandwall.animate = animateInLineBetweenCells;
        Sandwall.canCastOn = canCast;
        //Add effect on spell
        Sandwall.spellEffectList.Add(SpellEffectList.PushFromPlayer);
        Sandwall.spellEffectList.Add(SpellEffectList.CreateObstacle);
        // Damage
        Sandwall.doDamageAction = doDamage;
    }

    public bool canCast(
        Spell spell,
        List<Vector3Int> range,
        Vector3Int cell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        // Tile is empty
        if (!tilemap.HasTile(cell))
        {
            return false;
        }
        // Tile contains an obstacle
        if (obstacleList.ContainsKey(cell))
        {
            return false;
        }
        // No spell selected
        if (spell == null)
        {
            return false;
        }
        // Check line of sight
        if (spell.lineOfSight && !RangeUtils.lineOfSight(spell.casterPos, cell, obstacleList))
        {
            return false;
        }
        // Check if cell is in range
        if (!range.Contains(cell))
        {
            return false;
        }
        return true;
    }

    public bool canCast(
        Spell spell,
        Vector3Int cell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        return canCast(spell, spell.getRange(obstacleList, tilemap), cell, obstacleList, tilemap);
    }

    // Animation
    public void animateInCircleFull(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = getAreaInCircleFull(spell, obstacleList, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, obstacleList, tilemap));
    }

    public void animateInLine(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = getAreaInLine(spell, obstacleList, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, obstacleList, tilemap));
    }

    public void animateInLineBetweenCells(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = getAreaInLineBetweenCells(spell, obstacleList, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, obstacleList, tilemap));
    }

    IEnumerator multipleAnimateOnCell(
        List<Vector3Int> listCells,
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        foreach (var c in listCells)
        {
            yield return new WaitForSeconds(0.1f);
            animateOnCell(spell, c, obstacleList, tilemap);
        }
    }

    public void animateOnCell(
        Spell spell,
        Vector3Int to,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Vector2 worldPos = tilemap.CellToWorld(to);
        // Instantiate animation
        Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        // Refresh tile
        tilemap.RefreshTile(to);
    }

    public void animateOnCell(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> listCells = getAreaSingleCell(spell, obstacleList, tilemap);

        listCells.ForEach(s =>
        {
            Vector2 worldPos = tilemap.CellToWorld(s);
            // Instantiate animation
            Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
            // Refresh tile
            tilemap.RefreshTile(s);
        });
    }

    // Range 
    public List<Vector3Int> getRangeInCircleFullPlayer(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(spell.casterPos, spell.range, tilemap);

        // Check for each square if spell can be cast
        listSquare.ForEach(s =>
        {
            // Spell is always in range though
            if (canCast(spell, listSquare, s, obstacleList, tilemap))
            {
                area.Add(s);
            }
        });

        return area;
    }

    // Area of Effect
    public List<Vector3Int> getAreaInCircleFull(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaCircleFull(s, spell.area, tilemap)).ToList();
        });
        return area;
    }

    public List<Vector3Int> getAreaInLine(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaInLine(spell.casterPos, s, obstacleList, tilemap, spell.uniqueCellArea)).ToList();
        });
        return area;
    }

    public List<Vector3Int> getAreaSingleCell(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        return new List<Vector3Int>(spell.spellPos);
    }

    public List<Vector3Int> getAreaInLineBetweenCells(
        Spell spell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        for (int i = 0; i < spell.spellPos.Count() - 1; i++)
        {
            area = area.Concat(RangeUtils.getAreaInLine(spell.spellPos[i], spell.spellPos[i + 1], obstacleList, tilemap, spell.uniqueCellArea)).ToList();
        }
        return area;
    }

    // Damage
    public void doDamage(
        Spell spell,
        Dictionary<Unit, GameObject> playerList,
        Dictionary<Unit, GameObject> enemyList,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> areaSpell = spell.getArea(obstacleList, tilemap);
        // Friends take damage
        foreach (var s in playerList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(s.Key.position)).Count();
            if (selectedNb > 0)
            {
                s.Key.takeDamage(spell.damage * selectedNb);
            }
        }
        // Enemies take damage
        foreach (var s in enemyList)
        {
            // Count number of case selected in area
            int selectedNb = areaSpell.Where(x => x.Equals(s.Key.position)).Count();
            if (selectedNb > 0)
            {
                s.Key.takeDamage(spell.damage * selectedNb);
            }
        }
    }
}