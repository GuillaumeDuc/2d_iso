using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class SpellAreaList
{
    public static List<Vector3Int> getAreaInCircleFull(
    Spell spell,
    Vector3Int target,
    Unit caster,
    Dictionary<Vector3Int, GameObject> obstacleList,
    Tilemap tilemap
    )
    {
        List<Vector3Int> circle = RangeUtils.getAreaCircleFull(target, spell.area, tilemap);

        if (spell.burst)
        {
            circle = burst(target, caster, circle, obstacleList, tilemap);
        }
        return circle;
    }

    public static List<Vector3Int> getAreaInLine(
        Spell spell,
        Vector3Int target,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        area = area.Concat(RangeUtils.getAreaInLine(caster.position, target, obstacleList, tilemap, spell.uniqueCellArea)).ToList();
        return area;
    }

    public static List<Vector3Int> getAreaInLineBetweenCells(
        Spell spell,
        Vector3Int target,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        int index = caster.selectedSpellPos.IndexOf(target);
        if (index - 1 < 0)
        {
            return new List<Vector3Int>();
        }
        Vector3Int previousPos = caster.selectedSpellPos[index - 1];
        List<Vector3Int> area = RangeUtils.getAreaInLine(
            previousPos,
            target,
            obstacleList,
            tilemap,
            spell.uniqueCellArea
        );
        return area;
    }

    public static List<Vector3Int> getAreaAndresCircle(
        Spell spell,
        Vector3Int target,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> circle = RangeUtils.AndresCircle(target.x, target.y, spell.area);

        if (spell.burst)
        {
            circle = burst(target, caster, circle, obstacleList, tilemap);
        }

        return circle;
    }

    private static List<Vector3Int> burst(
        Vector3Int current,
        Unit caster,
        List<Vector3Int> circle,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        // If cell selected equals an obstacle move spellpos one cell toward player
        if (obstacleList.ContainsKey(current))
        {
            current = RangeUtils.getClosestNeighbour(current, caster.position, tilemap);
        }

        List<Vector3Int> newCircle = new List<Vector3Int>();
        circle.ForEach(v =>
        {
            if (RangeUtils.lineOfSight(current, v, obstacleList, tilemap))
            {
                newCircle.Add(v);
            }
        });
        return newCircle;
    }
}
