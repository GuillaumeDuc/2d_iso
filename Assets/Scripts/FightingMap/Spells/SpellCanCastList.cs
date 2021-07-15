using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class SpellCanCastList
{

    public static bool canCast(
        Spell spell,
        Unit caster,
        List<Vector3Int> range,
        Vector3Int target,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        // Tile is empty
        if (!tilemap.HasTile(target))
        {
            return false;
        }
        // No spell selected
        if (spell == null)
        {
            return false;
        }
        // Check line of sight
        if (spell.lineOfSight &&
            !RangeUtils.lineOfSight(
                caster.position,
                target,
                obstacleList,
                tilemap
                )
            )
        {
            return false;
        }
        // Check if target is in range
        if (!range.Contains(target))
        {
            return false;
        }
        return true;
    }
}
