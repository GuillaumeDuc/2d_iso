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

        // Icycle
        nameSpell = "Icycle";
        IcycleGO = Resources.Load<GameObject>(PATH + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 10, 12, 5, true);
        Icycle.getRangeList = getRangeInCircleFullPlayer;
        Icycle.getAreaList = getAreaInLine;
        Icycle.animate = animateInLine;
    }

    // Animation
    public void animateInCircleFull(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaInCircleFull(spell, tilemap);

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

    public void animateInLine(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaInLine(spell, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, tilemap));
    }


    // Range 
    public List<Vector3Int> getRangeInCircleFullPlayer(Spell spell, Tilemap tilemap)
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(spell.casterPos, spell.range, tilemap);

        // Check for each square if there is line of sight
        listSquare.ForEach(s =>
        {
            if (RangeUtils.lineOfSight(spell.casterPos, s, tilemap))
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

    /*
    public List<Vector3Int> getAreaSingleCell(Spell spell, int i, Tilemap tilemap)
    {
        return new List<Vector3Int>() { spell.spellPos[i] };
    }
    */
}
