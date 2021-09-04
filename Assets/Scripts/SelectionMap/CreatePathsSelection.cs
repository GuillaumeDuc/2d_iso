using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Positions
{
    public Vector2 upLeft, bottomLeft, upRight, bottomRight;
    public Positions(Vector2 upLeft, Vector2 bottomLeft, Vector2 upRight, Vector2 bottomRight)
    {
        this.upLeft = upLeft;
        this.bottomLeft = bottomLeft;
        this.upRight = upRight;
        this.bottomRight = bottomRight;
    }
}

public class CreatePathsSelection : MonoBehaviour
{
    public void createPath(List<LocationPoint> list, int radius)
    {
        List<LocationPoint> path = getPath(list, radius);
        foreach (var p in path)
        {
            p.nextLocations.ForEach(v =>
            {
                createPathBetweenPoints(p, v);
            });
        }
    }

    private List<LocationPoint> getPath(List<LocationPoint> list, int radius)
    {
        List<LocationPoint> newList = new List<LocationPoint>();
        list.ForEach(v =>
        {
            // Set next points
            v.nextLocations = getNextPoints(list, v.gameObject.transform.position, radius);
            // Set previous points
            v.nextLocations.ForEach(a =>
            {
                a.previousLocations.Add(v);
            });
            newList.Add(v);
        });
        return newList;
    }

    private List<LocationPoint> getNextPoints(List<LocationPoint> list, Vector2 v, int radius)
    {
        List<LocationPoint> nextPoints = new List<LocationPoint>();
        list.ForEach(check =>
        {
            if (check.gameObject.transform.position.x > v.x)
            {
                // Get closest points in radius
                float sqrDst = (v - (Vector2)check.gameObject.transform.position).sqrMagnitude;
                if (sqrDst < radius * radius * 2)
                {
                    nextPoints.Add(check);
                }
            }
        });
        return nextPoints;
    }

    private void createPathBetweenPoints(LocationPoint v1, LocationPoint v2)
    {
        float height = 0.05f;
        float pieces = 4;
        List<Positions> list = breakInPieces(v1.gameObject.transform.position, v2.gameObject.transform.position, height, pieces);

        list.ForEach(pos =>
        {
            GameObject rectangle = new GameObject("rectangle");
            RandomRectangle rRectangle = rectangle.AddComponent<RandomRectangle>();

            rRectangle.setRectangle(pos.upLeft, pos.upRight, pos.bottomLeft, pos.bottomRight);
            GameObject go = Instantiate(rectangle, v1.gameObject.transform.position, Quaternion.identity);
            Destroy(go);
        });
    }

    private Vector2 rotatePointAroundPivot(Vector2 point, Vector2 pivot, Vector3 angles)
    {
        Vector2 dir = point - pivot;
        dir = Quaternion.Euler(angles) * dir;
        point = dir + pivot;
        return point;
    }

    List<Positions> breakInPieces(Vector2 v1, Vector2 v2, float height, float pieces)
    {
        List<Positions> listPos = new List<Positions>();

        // +2 pieces total to get 2 space step
        float lengthX = (v1.x - v2.x) / (pieces + 2);
        float lengthY = (v1.y - v2.y) / (pieces + 2);

        // Total space step is equal 2 pieces of a length
        float stepX = (lengthX * 2 / (pieces + 1));
        float stepY = (lengthY * 2 / (pieces + 1));

        Vector2 current = new Vector2(v1.x, v1.y);

        for (int i = 0; i < pieces; i++)
        {
            current.x = current.x -= stepX;
            current.y = current.y -= stepY;

            Vector2 upLeft = new Vector2(current.x, current.y + height);
            Vector2 bottomLeft = new Vector2(current.x, current.y - height);

            Vector3 vectorToTarget = v2 - current;
            float angle = Mathf.Atan2(vectorToTarget.y, vectorToTarget.x) * Mathf.Rad2Deg;
            upLeft = rotatePointAroundPivot(upLeft, current, new Vector3(0, 0, angle));
            bottomLeft = rotatePointAroundPivot(bottomLeft, current, new Vector3(0, 0, angle));

            current.x = current.x -= lengthX;
            current.y = current.y -= lengthY;

            Vector2 upRight = new Vector2(current.x, current.y + height);
            Vector2 bottomRight = new Vector2(current.x, current.y - height);
            upRight = rotatePointAroundPivot(upRight, current, new Vector3(0, 0, angle));
            bottomRight = rotatePointAroundPivot(bottomRight, current, new Vector3(0, 0, angle));

            listPos.Add(new Positions(upLeft, bottomLeft, upRight, bottomRight));
        }
        return listPos;
    }
}
