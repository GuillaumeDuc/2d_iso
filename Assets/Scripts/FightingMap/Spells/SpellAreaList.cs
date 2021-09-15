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
        List<Vector3Int> area = new List<Vector3Int>(RangeUtils.getAreaInLine(caster.position, target, spell.area, obstacleList, tilemap, spell.uniqueCellArea));

        // Get the direction of the collision
        Vector3 direction = target - caster.position;
        // Remove area behind caster
        List<Vector3Int> removeList = area.FindAll(a =>
        {
            // Right / Left
            if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
            {
                // Right
                if (direction.x > 0)
                {
                    return a.x <= caster.position.x;
                }
                // Left
                if (direction.x < 0)
                {
                    return a.x >= caster.position.x;
                }
                return false;
            }
            else
            {
                if (direction.y > 0)
                {
                    return a.y <= caster.position.y;
                }
                if (direction.y < 0)
                {
                    return a.y >= caster.position.y;
                }
                return false;
            }
        });

        // Remove list
        area = area.Except(removeList).ToList();

        if (spell.burst)
        {
            area = burst(target, caster, area, obstacleList, tilemap);
        }
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
            spell.area,
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

    public static List<Vector3Int> getAreaInLineHorizontal(
        Spell spell,
        Vector3Int target,
        Unit caster,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> line = new List<Vector3Int>();

        // Get the direction of the collision
        Vector3 direction = target - caster.position;
        // Right / Left
        if (Mathf.Abs(direction.x) > Mathf.Abs(direction.y))
        {
            line.AddRange(RangeUtils.getAreaInLine(new Vector3Int(target.x, target.y - spell.area, target.z), new Vector3Int(target.x, target.y + spell.area, target.z), 0, obstacleList, tilemap, spell.uniqueCellArea));
            // Add to
            line.Add(new Vector3Int(target.x, target.y - spell.area, target.z));
        }
        else
        {
            line.AddRange(RangeUtils.getAreaInLine(new Vector3Int(target.x - spell.area, target.y, target.z), new Vector3Int(target.x + spell.area, target.y, target.z), 0, obstacleList, tilemap, spell.uniqueCellArea));
            line.Add(new Vector3Int(target.x - spell.area, target.y, target.z));
        }

        if (spell.burst)
        {
            line = burst(target, caster, line, obstacleList, tilemap);
        }

        return line;
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
            if (RangeUtils.lineOfWalk(current, v, obstacleList, tilemap))
            {
                newCircle.Add(v);
            }
        });
        return newCircle;
    }
}
