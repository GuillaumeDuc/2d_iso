using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class SpellList : MonoBehaviour
{
    private const string PATH = "Spells/SpellsPrefab/";

    public Spell Explosion, Icycle;
    private static GameObject ExplosionGO, IcycleGO;
    private string nameSpell;
    private RangeUtils RangeUtils;

    void Start()
    {
        // Instantiate Utils to get area & calculations
        RangeUtils = new RangeUtils();

        // Explosion
        nameSpell = "Explosion";
        ExplosionGO = Resources.Load<GameObject>(PATH + nameSpell);
        Explosion = new Spell(ExplosionGO, nameSpell, 3, 10, 3);
        Explosion.animate = animateInCircleFull;
        Explosion.getAreaList = getAreaInCircleFull;

        // Icycle
        nameSpell = "Icycle";
        IcycleGO = Resources.Load<GameObject>(PATH + nameSpell);
        Icycle = new Spell(IcycleGO, nameSpell, 10, 12, 5, true, true);
        Icycle.animate = animateInLine;
        Icycle.getAreaList = getAreaInCircleFull;
    }

    // Animation

    public void animateOnCell(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        Vector2 worldPos = tilemap.CellToWorld(to);
        Instantiate(spell.spellGO, new Vector2(worldPos.x, worldPos.y + 0.2f), Quaternion.identity);
    }

    public void animateInLine(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        List<Vector3Int> listCells = getAreaInLine(spell, to, from, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, from, tilemap));
    }

    public void animateInCircleFull(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        List<Vector3Int> listCells = RangeUtils.getAreaCircleFull(to, spell.area, tilemap);

        StartCoroutine(multipleAnimateOnCell(listCells, spell, from, tilemap));
    }

    IEnumerator multipleAnimateOnCell(List<Vector3Int> listCells, Spell spell, Vector3Int from, Tilemap tilemap)
    {
        foreach (var c in listCells)
        {
            yield return new WaitForSeconds(0.1f);
            animateOnCell(spell, c, from, tilemap);
        }
    }

    // Area
    public List<Vector3Int> getAreaSingleCell(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        return new List<Vector3Int>() { to };
    }

    public List<Vector3Int> getAreaInCircleFull(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        return RangeUtils.getAreaCircleFull(to, spell.area, tilemap);
    }

    public List<Vector3Int> getAreaInLine(Spell spell, Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        return RangeUtils.getAreaInLine(to, from, tilemap);
    }
}
