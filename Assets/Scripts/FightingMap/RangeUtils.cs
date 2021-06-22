using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;
using System;

public static class RangeUtils
{
    public static bool lineOfSight(
        Vector3Int playerPos,
        Vector3Int cellPos,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        List<Vector3Int> lineOfSightArray = new List<Vector3Int>(getLine(playerPos, cellPos));
        bool lineOS = true;
        lineOfSightArray.ForEach(a =>
        {
            GroundTile gt = (GroundTile)tilemap.GetTile(a);
            // Ground tile is null, cell contains an obstacle on its path, cell contains a tile blocking LoS
            if (gt == null || (obstacleList.ContainsKey(a) && a != cellPos) || (!gt.lineOfSight && a != cellPos))
            {
                lineOS = false;
            };
        });
        return lineOS;
    }

    public static List<Vector3Int> getLine(Vector3Int playerPos, Vector3Int cellPos)
    {
        List<Vector3Int> lineOfSightArray = new List<Vector3Int>();
        int dx = Mathf.Abs(cellPos.x - playerPos.x);
        int dy = Mathf.Abs(cellPos.y - playerPos.y);
        int sx = playerPos.x < cellPos.x ? 1 : -1;
        int sy = playerPos.y < cellPos.y ? 1 : -1;

        int err = dx - dy;

        while (true)
        {
            lineOfSightArray.Add(new Vector3Int(playerPos.x, playerPos.y, playerPos.z));
            if (playerPos.x == cellPos.x && playerPos.y == cellPos.y)
            {
                break;
            }
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                playerPos.x += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                playerPos.y += sy;
            }
        }
        return lineOfSightArray;
    }

    internal static IEnumerable<Vector3Int> getAreaCircleFull()
    {
        throw new NotImplementedException();
    }

    public static List<Vector3Int> getAreaCircleEmpty(Vector3Int cell, int area, Tilemap tilemap)
    {
        List<Vector3Int> areaList = new List<Vector3Int>(getAreaCircleFull(cell, area, tilemap));
        float highest = getFarthestSquareMD(areaList, cell);
        areaList = getSquareByMD(areaList, cell, highest);
        return areaList;
    }

    public static List<Vector3Int> getAreaCircleFull(Vector3Int cell, int area, Tilemap tilemap = null, int min = 0, List<Vector3Int> excludeList = null)
    {
        List<Vector3Int> areaList = new List<Vector3Int>() { cell };
        List<Vector3Int> adjList = new List<Vector3Int>(getAdjacentSquares(cell));
        HashSet<Vector3Int> nextList = new HashSet<Vector3Int>();

        for (int i = min; i < area; i++)
        {
            foreach (var c in adjList)
            {
                if (!areaList.Contains(c))
                {
                    if (excludeList == null || !excludeList.Contains(c))
                    {
                        // Add cell to returned list
                        areaList.Add(c);
                        // Add adjacent squares to next list
                        nextList.UnionWith(getAdjacentSquares(c));
                    }
                }
            }
            // Assign new adjacent squares
            adjList = nextList.ToList();
            // Clear next list
            nextList.Clear();
        }
        return areaList;
    }

    private static List<Vector3Int> getAdjacentSquares(Vector3Int cell)
    {
        Vector3Int up = new Vector3Int(cell.x, cell.y + 1, cell.z);
        Vector3Int down = new Vector3Int(cell.x, cell.y - 1, cell.z);
        Vector3Int left = new Vector3Int(cell.x - 1, cell.y, cell.z);
        Vector3Int right = new Vector3Int(cell.x + 1, cell.y, cell.z);

        return new List<Vector3Int>() { up, down, left, right };
    }

    public static List<Vector3Int> getAreaInLine(
        Vector3Int to,
        Vector3Int from,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap,
        bool uniqueCellArea = false
        )
    {
        List<Vector3Int> listCells = new List<Vector3Int>(getLine(from, to));
        listCells.Reverse();
        if (!uniqueCellArea)
        {
            return listCells;
        }

        List<Vector3Int> result = new List<Vector3Int>();
        // Remove existing cell in obstacleList
        listCells.ForEach(c =>
        {
            if (uniqueCellArea && !obstacleList.ContainsKey(c))
            {
                result.Add(c);
            }
        });
        return result;
    }

    public static List<Vector3Int> getAreaInLineFollowClick(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        Vector3Int current = new Vector3Int(to.x, to.y, to.z);
        List<Vector3Int> listCells = new List<Vector3Int>();

        while (current != from)
        {
            listCells.Add(current);
            current = getClosestNeighbour(current, from, tilemap);
        }

        listCells.Reverse();
        return listCells;
    }

    public static bool isWalkable(
        Vector3Int cell,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        GroundTile gt = (GroundTile)tilemap.GetTile(cell);
        return gt != null && !obstacleList.ContainsKey(cell) && gt.walkable;
    }

    public static Vector3Int getFarthestWalkableNeighbour(
        Vector3Int to,
        Vector3Int from,
        List<Vector3Int> area,
        Dictionary<Vector3Int, GameObject> obstacleList,
        Tilemap tilemap
        )
    {
        Vector3Int up = new Vector3Int(to.x, to.y + 1, to.z);
        Vector3Int down = new Vector3Int(to.x, to.y - 1, to.z);
        Vector3Int left = new Vector3Int(to.x - 1, to.y, to.z);
        Vector3Int right = new Vector3Int(to.x + 1, to.y, to.z);

        Dictionary<Vector3Int, float> neighbours = new Dictionary<Vector3Int, float>() {
            { up, getMDistance(up, from) },
            { down, getMDistance(down, from) },
            { left, getMDistance(left, from) },
            { right, getMDistance(right, from) }
        };

        Vector3Int farthest = neighbours.FirstOrDefault(x =>
        {
            return (x.Value == neighbours.Max(n => n.Value)) &&
            isWalkable(x.Key, obstacleList, tilemap) &&
            !area.Contains(x.Key);
        }).Key;

        if (!isWalkable(to, obstacleList, tilemap) || area.Contains(to))
        {
            return getFarthestWalkableNeighbour(farthest, from, area, obstacleList, tilemap);
        }
        return to;
    }

    private static Vector3Int getClosestNeighbour(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        Vector3Int up = new Vector3Int(to.x, to.y + 1, to.z);
        Vector3Int down = new Vector3Int(to.x, to.y - 1, to.z);
        Vector3Int left = new Vector3Int(to.x - 1, to.y, to.z);
        Vector3Int right = new Vector3Int(to.x + 1, to.y, to.z);
        Vector3Int topRight = new Vector3Int(to.x + 1, to.y + 1, to.z);
        Vector3Int topLeft = new Vector3Int(to.x - 1, to.y + 1, to.z);
        Vector3Int bottomRight = new Vector3Int(to.x + 1, to.y - 1, to.z);
        Vector3Int bottomLeft = new Vector3Int(to.x - 1, to.y - 1, to.z);

        Dictionary<Vector3Int, float> neighbours = new Dictionary<Vector3Int, float>() {
            { up, getMDistance(up, from) },
            { down, getMDistance(down, from) },
            { left, getMDistance(left, from) },
            { right, getMDistance(right, from) },
            { topRight, getMDistance(topRight, from) },
            { topLeft, getMDistance(topLeft, from) },
            { bottomRight, getMDistance(bottomRight, from) },
            {bottomLeft, getMDistance(bottomLeft, from) },
        };

        return neighbours.FirstOrDefault(x => x.Value == neighbours.Min(n => n.Value)).Key;
    }

    private static float getMDistance(Vector3Int to, Vector3Int from)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }

    public static int getNbFarthestAdjSquares(Tilemap tilemap, Vector3Int to, Vector3Int from)
    {
        List<Vector3Int> listSquare = new List<Vector3Int>(getAdjacentSquares(from));
        // Get highest value
        float highestD = getFarthestSquareMD(listSquare, to);
        int nbSquares = 0;
        // Get farthest number of squares in adj list
        foreach (var adj in listSquare)
        {
            float mDistance = getMDistance(to, adj);
            if (highestD == mDistance)
            {
                nbSquares += 1;
            }
        }
        return nbSquares;
    }

    private static float getFarthestSquareMD(List<Vector3Int> listSquare, Vector3Int to)
    {
        float highestD = 0f;
        foreach (var s in listSquare)
        {
            float mDistance = getMDistance(to, s);
            if (highestD < mDistance)
            {
                highestD = mDistance;
            }
        }
        return highestD;
    }

    private static List<Vector3Int> getSquareByMD(List<Vector3Int> listSquare, Vector3Int to, float mDistance)
    {
        List<Vector3Int> listRes = new List<Vector3Int>(listSquare);
        return listRes.FindAll(s =>
            {
                return getMDistance(to, s) == mDistance;
            }
        );
    }

    public static List<Vector3Int> AndresCircle(int xc, int yc, int r)
    {
        List<Vector3Int> ret = new List<Vector3Int>();

        int x = 0;
        int y = r;
        int d = r - 1;

        while (y >= x)
        {
            ret.Add(new Vector3Int(xc + x, yc + y, 0));
            ret.Add(new Vector3Int(xc + y, yc + x, 0));
            ret.Add(new Vector3Int(xc - x, yc + y, 0));
            ret.Add(new Vector3Int(xc - y, yc + x, 0));
            ret.Add(new Vector3Int(xc + x, yc - y, 0));
            ret.Add(new Vector3Int(xc + y, yc - x, 0));
            ret.Add(new Vector3Int(xc - x, yc - y, 0));
            ret.Add(new Vector3Int(xc - y, yc - x, 0));

            if (d >= 2 * x)
            {
                d -= 2 * x + 1;
                x++;
            }
            else if (d < 2 * (r - y))
            {
                d += 2 * y - 1;
                y--;
            }
            else
            {
                d += 2 * (y - x - 1);
                y--;
                x++;
            }
        }
        ret = ret.Distinct().ToList();
        ret = fillCircle(ret);
        return ret;
    }

    public static List<Vector3Int> fillCircle(List<Vector3Int> circle)
    {
        List<Vector3Int> filledCircle = new List<Vector3Int>(circle);

        if (circle.Count == 0)
        {
            return filledCircle;
        }

        int higherY = circle.Max(v => v.y);
        int lowerY = circle.Min(v => v.y);

        for (int y = lowerY + 1; y < higherY; y++)
        {
            List<Vector3Int> line = circle.FindAll(v => v.y == y);
            int higherX = line.Max(v => v.x);
            int lowerX = line.Min(v => v.x);

            for (int x = lowerX + 1; x < higherX; x++)
            {
                filledCircle.Add(new Vector3Int(x, y, 0));
            }
        }

        return filledCircle;
    }
}
