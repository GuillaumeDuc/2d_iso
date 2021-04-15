using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/SpellsPrefab/";

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
        Explosion = new Spell(ExplosionGO, nameSpell, 20, 8, 5, true, 2);
        Explosion.getRangeList = getRangeInCircleFullPlayer;
        Explosion.getAreaList = getAreaInCircleFull;
        Explosion.animate = animateInCircleFull;
        Explosion.canCastOn = canCast;

        // Icycle
        nameSpell = "Icycle";
        IcycleGO = Resources.Load<GameObject>(PATH + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 30, 6, 1, false, 3);
        Icycle.getRangeList = getRangeInCircleFullPlayer;
        Icycle.getAreaList = getAreaSingleCell;
        Icycle.animate = animateOnCell;
        Icycle.canCastOn = canCast;

        // Sandwall
        nameSpell = "Sandwall";
        SandwallGO = Resources.Load<GameObject>(PATH + nameSpell);
        Sandwall = new Spell(SandwallGO, nameSpell, 0, 7, 1, true, 2);
        Sandwall.getRangeList = getRangeInCircleFullPlayer;
        Sandwall.getAreaList = getAreaInLineBetweenCells;
        Sandwall.animate = animateInLineBetweenCells;
        Sandwall.canCastOn = canCast;
    }

    public bool canCast(Spell spell, List<Vector3Int> range, Vector3Int cell, Tilemap tilemap, Tilemap obstacles)
    {
        // Tile is empty
        if (!tilemap.HasTile(cell))
        {
            return false;
        }
        // Tile contains an obstacle
        if (obstacles.HasTile(cell))
        {
            return false;
        }
        // No spell selected
        if (spell == null)
        {
            return false;
        }
        // Check line of sight
        if (spell.lineOfSight && !RangeUtils.lineOfSight(spell.casterPos, cell, obstacles))
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

    public bool canCast(Spell spell, Vector3Int cell, Tilemap tilemap, Tilemap obstacles)
    {
        return canCast(spell, spell.getRange(tilemap, obstacles), cell, tilemap, obstacles);
    }

    // Animation
    public void animateInCircleFull(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaInCircleFull(spell, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, tilemap));
    }

    public void animateInLine(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaInLine(spell, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, tilemap));
    }

    public void animateInLineBetweenCells(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaInLineBetweenCells(spell, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, tilemap));
    }

    IEnumerator multipleAnimateOnCell(List<Vector3Int> listCells, Spell spell, Tilemap tilemap)
    {
        foreach (var c in listCells)
        {
            yield return new WaitForSeconds(0.1f);
            animateOnCell(spell, c, tilemap);
        }
    }

    public void animateOnCell(Spell spell, Vector3Int to, Tilemap tilemap)
    {
        Vector2 worldPos = tilemap.CellToWorld(to);
        Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
    }

    public void animateOnCell(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaSingleCell(spell, tilemap);

        listCells.ForEach(s =>
        {
            Vector2 worldPos = tilemap.CellToWorld(s);
            Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
        });
    }

    // Range 
    public List<Vector3Int> getRangeInCircleFullPlayer(Spell spell, Tilemap tilemap, Tilemap obstacles)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(spell.casterPos, spell.range, tilemap);

        // Check for each square if spell can be cast
        listSquare.ForEach(s =>
        {
            // Spell is always in range though
            if (canCast(spell, listSquare, s, tilemap, obstacles))
            {
                area.Add(s);
            }
        });
        return area;
    }

    // Area of Effect
    public List<Vector3Int> getAreaInCircleFull(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaCircleFull(s, spell.area, tilemap)).ToList();
        });
        return area;
    }

    public List<Vector3Int> getAreaInLine(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaInLine(spell.casterPos, s, tilemap)).ToList();
        });
        return area;
    }

    public List<Vector3Int> getAreaSingleCell(Spell spell, Tilemap tilemap)
    {
        return new List<Vector3Int>(spell.spellPos);
    }

    public List<Vector3Int> getAreaInLineBetweenCells(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> area = new List<Vector3Int>();

        for (int i = 0; i < spell.spellPos.Count() - 1; i++)
        {
            area = area.Concat(RangeUtils.getAreaInLine(spell.spellPos[i], spell.spellPos[i + 1], tilemap)).ToList();
        }
        return area;
    }
}
