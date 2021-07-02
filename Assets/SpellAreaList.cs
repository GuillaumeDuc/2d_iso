using System.Collections;
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
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaCircleFull(s, spell.area, tilemap)).ToList();
        });
        return area;
    }

    public static List<Vector3Int> getAreaInLine(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            area = area.Concat(RangeUtils.getAreaInLine(caster.position, s, obstacleList, tilemap, spell.uniqueCellArea)).ToList();
        });
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
        }
        return area;
    }

    public static List<Vector3Int> getAreaAndresCircle(
        Spell spell,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> area = new List<Vector3Int>();

        spell.spellPos.ForEach(s =>
        {
            Vector3Int current = s;
            List<Vector3Int> circle = RangeUtils.AndresCircle(current.x, current.y, spell.area);

            if (spell.burst)
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
                circle = newCircle;
            }
            area = area.Concat(circle).ToList();
        });

        return area;
    }
}
