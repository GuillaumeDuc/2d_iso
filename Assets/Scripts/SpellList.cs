using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/SpellsPrefab/";

    public Spell Explosion,
        Icycle;
    private static GameObject ExplosionGO,
        IcycleGO;
    private string nameSpell;
    private RangeUtils RangeUtils;

    void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();

        // Explosion
        nameSpell = "Explosion";
        ExplosionGO = Resources.Load<GameObject>(PATH + nameSpell);
        Explosion = new Spell(ExplosionGO, nameSpell, 3, 6, 3);
        Explosion.getRangeList = getRangeInCircleFullPlayer;
        Explosion.getAreaList = getAreaInCircleFull;
        Explosion.animate = animateInCircleFull;
        Explosion.canCastOn = canCast;

        // Icycle
        nameSpell = "Icycle";
        IcycleGO = Resources.Load<GameObject>(PATH + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 10, 10, 1, false, 3);
        Icycle.getRangeList = getRangeInCircleFullPlayer;
        Icycle.getAreaList = getAreaSingleCell;
        Icycle.animate = animateOnCell;
        Icycle.canCastOn = canCast;
    }

    public bool canCast(Spell spell, List<Vector3Int> range, Vector3Int cell, Tilemap tilemap)
    {
        // Tile is empty
        if (!tilemap.HasTile(cell))
        {
            return false;
        }
        // No spell selected
        if (spell == null)
        {
            return false;
        }
        // Check line of sight
        if (spell.lineOfSight && !RangeUtils.lineOfSight(spell.casterPos, cell, tilemap))
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

    public bool canCast(Spell spell, Vector3Int cell, Tilemap tilemap)
    {
        return canCast(spell, spell.getRange(tilemap), cell, tilemap);
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
    public List<Vector3Int> getRangeInCircleFullPlayer(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(spell.casterPos, spell.range, tilemap);

        // Check for each square if spell can be cast
        listSquare.ForEach(s =>
        {
            // Spell is always in range though
            if (canCast(spell, listSquare, s, tilemap))
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
}
