using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class SpellRangeList
{
    public static List<Vector3Int> getRangeInCircleFull(
    Spell spell,
    Unit caster,
    Dictionary<Vector3Int, GameObject> obstacleList,
    Tilemap tilemap
    )
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get full circle
        List<Vector3Int> listSquare = RangeUtils.getAreaCircleFull(caster.position, spell.range, tilemap);

        // Check for each square if spell can be cast
        listSquare.ForEach(s =>
        {
            // Spell is always in range though
            if (spell.canCast(caster, listSquare, s, obstacleList, tilemap))
            {
                area.Add(s);
            }
        });

        return area;
    }

    public static List<Vector3Int> getRangeInLine(
    Spell spell,
    Unit caster,
    Dictionary<Vector3Int, GameObject> obstacleList,
    Tilemap tilemap
    )
    {
        List<Vector3Int> area = new List<Vector3Int>();
        // Get left, right, top, bottom line
        Vector3Int left = new Vector3Int(caster.position.x - spell.range, caster.position.y, caster.position.z);
        Vector3Int right = new Vector3Int(caster.position.x + spell.range, caster.position.y, caster.position.z);
        Vector3Int top = new Vector3Int(caster.position.x, caster.position.y + spell.range, caster.position.z);
        Vector3Int bottom = new Vector3Int(caster.position.x, caster.position.y - spell.range, caster.position.z);
        List<Vector3Int> listSquare = new List<Vector3Int>() { };
        listSquare.AddRange(RangeUtils.getAreaInLine(caster.position, left, 0, obstacleList, tilemap));
        listSquare.AddRange(RangeUtils.getAreaInLine(caster.position, right, 0, obstacleList, tilemap));
        listSquare.AddRange(RangeUtils.getAreaInLine(caster.position, top, 0, obstacleList, tilemap));
        listSquare.AddRange(RangeUtils.getAreaInLine(caster.position, bottom, 0, obstacleList, tilemap));

        // Check for each square if spell can be cast
        listSquare.ForEach(s =>
        {
            // Spell is always in range though
            if (spell.canCast(caster, listSquare, s, obstacleList, tilemap))
            {
                area.Add(s);
            }
        });

        return area;
    }
}
