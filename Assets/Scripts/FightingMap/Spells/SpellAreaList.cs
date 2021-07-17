using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public static class SpellAreaList
{
    public static List<Vector3Int> getAreaInCircleFull(
    Spell spell,
    Unit caster,
    Dictionary<Vector3Int, GameObject> obstacleList,
    Tilemap tilemap
    )
    {
        List<Vector3Int> circle = RangeUtils.getAreaCircleFull(spell.position, spell.area, tilemap);

        if (spell.burst)
        {
            circle = burst(spell.position, caster, circle, obstacleList, tilemap);
        }
        return circle;
    }

    public static List<Vector3Int> getAreaInLine(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        area = area.Concat(RangeUtils.getAreaInLine(caster.position, spell.position, obstacleList, tilemap, spell.uniqueCellArea)).ToList();
        return area;
    }

    public static List<Vector3Int> getAreaInLineBetweenCells(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();
        /*
        for (int i = 0; i < spell.spellPos.Count() - 1; i++)
        {
            area = area.Concat(
                RangeUtils.getAreaInLine(
                    spell.spellPos[i],
                    spell.spellPos[i + 1],
                    obstacleList,
                    tilemap,
                    spell.uniqueCellArea
                    )
                ).ToList();
        }*/
        return area;
    }

    public static List<Vector3Int> getAreaAndresCircle(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> circle = RangeUtils.AndresCircle(spell.position.x, spell.position.y, spell.area);

        if (spell.burst)
        {
            circle = burst(spell.position, caster, circle, obstacleList, tilemap);
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