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
}
