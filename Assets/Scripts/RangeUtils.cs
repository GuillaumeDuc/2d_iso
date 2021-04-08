using UnityEngine.Tilemaps;
using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System.Linq;

public class RangeUtils
{
    public bool lineOfSight(Vector3Int playerPos, Vector3Int cellPos, Tilemap tilemap)
    {
        List<Vector3Int> lineOfSightArray = new List<Vector3Int>(getLine(playerPos, cellPos));
        bool lineOS = true;
        lineOfSightArray.ForEach(a =>
        {
            if (!tilemap.HasTile(a))
            {
                lineOS = false;
            };
        });
        return lineOS;
    }

    public List<Vector3Int> getLine(Vector3Int playerPos, Vector3Int cellPos)
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

    public List<Vector3Int> getAreaCircleEmpty(Vector3Int cell, int area, Tilemap tilemap)
    {
        List<Vector3Int> areaList = new List<Vector3Int>(getAreaCircleFull(cell, area, tilemap));
        float highest = getFarthestSquareMD(areaList, cell);
        areaList = getSquareByMD(areaList, cell, highest);
        return areaList;
    }

    public List<Vector3Int> getAreaCircleFull(Vector3Int cell, int area, Tilemap tilemap)
    {
        List<Vector3Int> areaList = new List<Vector3Int>() { cell };
        List<Vector3Int> adjList = new List<Vector3Int>(getAdjacentSquares(cell));
        HashSet<Vector3Int> nextList = new HashSet<Vector3Int>();

        for (int i = 0; i < area; i++)
        {
            foreach (var c in adjList)
            {
                if (!areaList.Contains(c))
                {
                    // Add cell to returned list
                    areaList.Add(c);
                    // Add adjacent squares to next list
                    nextList.UnionWith(getAdjacentSquares(c));
                }
            }
            // Assign new adjacent squares
            adjList = nextList.ToList();
            // Clear next list
            nextList.Clear();
        }
        return areaList;
    }

    private List<Vector3Int> getAdjacentSquares(Vector3Int cell)
    {
        Vector3Int up = new Vector3Int(cell.x, cell.y + 1, cell.z);
        Vector3Int down = new Vector3Int(cell.x, cell.y - 1, cell.z);
        Vector3Int left = new Vector3Int(cell.x - 1, cell.y, cell.z);
        Vector3Int right = new Vector3Int(cell.x + 1, cell.y, cell.z);

        return new List<Vector3Int>() { up, down, left, right };
    }

    public List<Vector3Int> getAreaInLine(Vector3Int to, Vector3Int from, Tilemap tilemap)
    {
        List<Vector3Int> listCells = new List<Vector3Int>(getLine(from, to));
        return listCells;
    }

    public List<Vector3Int> getAreaInLineFollowClick(Vector3Int to, Vector3Int from, Tilemap tilemap)
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

    private Vector3Int getClosestNeighbour(Vector3Int to, Vector3Int from, Tilemap tilemap)
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

    private float getMDistance(Vector3Int to, Vector3Int from)
    {
        return Mathf.Abs(to.x - from.x) + Mathf.Abs(to.y - from.y);
    }

    public void removeCells(Tilemap tilemap)
    {
        foreach (var a in tilemap.cellBounds.allPositionsWithin)
        {
            tilemap.SetTile(a, null);
        }
    }

    public void setTileOnTilemap(List<Vector3Int> listSquare, Tile tile, Tilemap tilemap)
    {
        listSquare.ForEach(s =>
        {
                tilemap.SetTile(s, tile);
        });
    }

    public int getNbFarthestAdjSquares(Tilemap tilemap, Vector3Int to, Vector3Int from)
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

    private float getFarthestSquareMD(List<Vector3Int> listSquare, Vector3Int to)
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

    private List<Vector3Int> getSquareByMD(List<Vector3Int> listSquare, Vector3Int to, float mDistance)
    {
        List<Vector3Int> listRes = new List<Vector3Int>(listSquare);
        return listRes.FindAll(s =>
            {
                return getMDistance(to, s) == mDistance;
            }
        );
    }
}
