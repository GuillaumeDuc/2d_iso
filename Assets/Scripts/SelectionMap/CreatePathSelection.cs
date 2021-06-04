using System.Collections;
using System.Collections.Generic;
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

public class CreatePathSelection : MonoBehaviour
{
    public void createPath(List<Vector2> list)
    {
        for (int i = 0; i < list.Count - 1; i++)
        {
            createPathBetweenPoints(list[i], list[i + 1]);
        }
    }

    private void createPathBetweenPoints(Vector2 v1, Vector2 v2)
    {
        float height = 0.05f;
        float pieces = 5;
        List<Positions> list = breakInPieces(v1, v2, height, pieces);

        list.ForEach(pos =>
        {
            GameObject rectangle = new GameObject("rectangle");
            RandomRectangle rRectangle = rectangle.AddComponent<RandomRectangle>();

            rRectangle.setRectangle(pos.upLeft, pos.upRight, pos.bottomLeft, pos.bottomRight);
            GameObject go = Instantiate(rectangle, v1, Quaternion.identity);
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

        float lengthX = (v1.x - v2.x) / pieces;
        float lengthY = (v1.y - v2.y) / pieces;

        float stepX = lengthX / pieces;
        float stepY = lengthY / pieces;

        Vector2 current = new Vector2(v1.x, v1.y);

        for (int i = 0; i < pieces - 1; i++)
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
