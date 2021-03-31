using System.Collections;
using System.Collections.Generic;
using UnityEngine.Tilemaps;
using UnityEngine;

public class Square
{
    public Vector3Int pos;
    public int cost;
    public float heuristic;
    public Square previousSquare = null;

    public Square(Vector3Int pos)
    {
        this.initSquare(pos);
    }

    public Square()
    {
        this.initSquare(new Vector3Int());
    }

    public Square(Square s)
    {
        this.initSquare(s.pos, s.cost, s.heuristic, s.previousSquare);
    }

    void initSquare(Vector3Int pos, int cost = 0, float heuristic = 0, Square previousSquare = null)
    {
        this.pos = pos;
        this.cost = cost;
        this.heuristic = heuristic;
    }

    public override bool Equals(System.Object obj)
    {
        //Check for null and compare run-time types.
        if ((obj == null) || !this.GetType().Equals(obj.GetType()))
        {
            return false;
        }
        else
        {
            Square s = (Square)obj;
            return pos == s.pos;
        }
    }

    public override int GetHashCode()
    {
        return ((int) pos.x << 2) ^ (int) pos.y;
    }

    public override string ToString()
    {
        return "Square " + pos.x + " : " + pos.y + "\n" +
            "heuristic " + heuristic + " - cost " + cost + "\n";
    }
}

public class MoveSystem : MonoBehaviour
{
    public Tilemap cellsGrid;

    public void moveCharacter(GameObject player, Vector3Int posDest, Tilemap tilemap)
    {
        // Get Position from player
        Transform playerTransform = player.GetComponent<Transform>();
        Vector3Int posPlayer = tilemap.WorldToCell(playerTransform.position);
        Square start = new Square(posPlayer);

        Square dest = new Square(posDest);

        // A* algorithm
        List<Square> path = getPathCharacter(start, dest, tilemap);

        setTilePath(tilemap, path, dest);
        // move player from 1 to 1 square
        StartCoroutine(Move(playerTransform, path, tilemap));
    }

    private List<Square> getPathCharacter(Square start, Square dest, Tilemap tilemap)
    {
        List<Square> openList = new List<Square>();
        List<Square> closedList = new List<Square>();
        Square currentSquare = start;

        // add original position to list
        openList.Add(start);

        // if list not empty
        while (openList.Count != 0)
        {   
            // get square with the lowest distance
            Square lowestSquare = getSquareLowestScore(openList);

            // override current square
            currentSquare = lowestSquare;

            // remove from list
            openList.RemoveAll(s => s.Equals(currentSquare));

            if (currentSquare.Equals(dest))
            {
                return reconstructPath(currentSquare, new List<Square>());
            }

            // get list of walkable adjacent square
            List<Square> adjacentSquares = getAdjacentSquares(currentSquare, tilemap);

            foreach (var a in adjacentSquares)
            {
                // if it does not exist in closedList or there is an existing square in openList with lowest cost
                if (!closedList.Contains(a) && !openList.Exists(s => s == a && s.cost < a.cost))
                {
                    a.cost = getCostToMove(currentSquare, a, tilemap);
                    a.heuristic = getMDistance(a, dest) + a.cost;
                    a.previousSquare = currentSquare;
                    if (!openList.Contains(a))
                    {
                        openList.Add(a);
                    }
                }
            }

            closedList.Add(currentSquare);
        }

        return new List<Square>();
    }

    private IEnumerator Move(Transform playerTransform, List<Square> path, Tilemap tilemap)
    {
        foreach (var s in path)
        {
            yield return new WaitForSeconds(0.1f);
            Vector2 pos = new Vector2(tilemap.CellToWorld(s.pos).x, tilemap.CellToWorld(s.pos).y + 0.2f);
            Debug.Log("ok : " + pos);
            playerTransform.position = pos;
        }
    }

    private Square getSquareLowestScore(List<Square> openList)
    {
        float res = 0;
        Square lowestSquare = new Square();
        foreach (var s in openList)
        {
            // square with lowest cost from openList
            if (res == 0 || s.heuristic < res)
            {
                res = s.heuristic;
                lowestSquare = s;
            }
        }
        return lowestSquare;
    }

    private float getMDistance(Square currentSquare, Square destination)
    {
        return Mathf.Abs(currentSquare.pos.x - destination.pos.x) + Mathf.Abs(currentSquare.pos.y - destination.pos.y);
    }

    private int getCostToMove(Square currentSquare, Square neighbour, Tilemap tilemap)
    {
        //ToDo overwrite tile object to get a custom tile with square cost
        int total = 1;

        total += currentSquare.cost;

        return total;
    }

    private List<Square> getAdjacentSquares(Square currentSquare, Tilemap tilemap)
    {
        //ToDo check if cell is unwalkable or objects blocking the way

        List<Square> adj = new List<Square>();

        Vector3Int up = new Vector3Int(currentSquare.pos.x, currentSquare.pos.y + 1, currentSquare.pos.z);
        if (tilemap.HasTile(up))
        {
            adj.Add(new Square(up));
        }

        Vector3Int down = new Vector3Int(currentSquare.pos.x, currentSquare.pos.y - 1, currentSquare.pos.z);
        if (tilemap.HasTile(down))
        {
            adj.Add(new Square(down));
        }

        Vector3Int left = new Vector3Int(currentSquare.pos.x - 1, currentSquare.pos.y, currentSquare.pos.z);
        if (tilemap.HasTile(left))
        {
            adj.Add(new Square(left));
        }

        Vector3Int right = new Vector3Int(currentSquare.pos.x + 1, currentSquare.pos.y, currentSquare.pos.z);
        if (tilemap.HasTile(right))
        {
            adj.Add(new Square(right));
        }
        return adj;
    }

    private List<Square> reconstructPath(Square current, List<Square> path)
    {
        if (current == null)
        {
            // reverse to get start to finish
            path.Reverse();
            return path;
        }

        path.Add(current);
        return reconstructPath(current.previousSquare, path);
    }

    private void setTilePath(Tilemap tilemap, List<Square> path, Square dest)
    {
        Tile transparent = Resources.Load<Tile>("Tilemaps/CellsGrid/grid_transparent_tile_iso");

        foreach (var s in path)
        {
            cellsGrid.SetTile(s.pos, transparent);
        }
    }
}
