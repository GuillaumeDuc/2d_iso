using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomRectangle : MonoBehaviour
{
    Vector2 upLeft, upRight, bottomLeft, bottomRight;

    void Start()
    {
        // Bottom points
        List<Vector2> points = new List<Vector2>() {
            bottomLeft,
            new Vector2(bottomLeft.x, bottomLeft.y - 0.05f),
            new Vector2(bottomRight.x, bottomRight.y - 0.05f),
            bottomRight,
        };
        List<Vector2> bezierList = new List<Vector2>();

        Vector2 tmp;
        for (float t = 0; t <= 1; t += 0.01f)
        {
            tmp = getCasteljauPoint(points.Count - 1, 0, t, points);
            bezierList.Add(tmp);
        }

        // Top points
        List<Vector2> pointsTop = new List<Vector2>() {
            upRight,
            new Vector2(upRight.x, upRight.y + 0.05f),
            new Vector2(upLeft.x, upLeft.y + 0.05f),
            upLeft,
        };

        for (float t = 0; t <= 1; t += 0.01f)
        {
            tmp = getCasteljauPoint(pointsTop.Count - 1, 0, t, pointsTop);
            bezierList.Add(tmp);
        }

        // Use the triangulator to get indices for creating triangles
        Triangulator tr = new Triangulator(bezierList.ToArray());
        int[] indices = tr.Triangulate();

        // Create the Vector3 vertices
        Vector3[] vertices = new Vector3[bezierList.Count];
        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = new Vector3(bezierList[i].x, bezierList[i].y, 0);
        }

        // Create the mesh
        Mesh msh = new Mesh();
        msh.vertices = vertices;
        msh.triangles = indices;
        msh.RecalculateNormals();
        msh.RecalculateBounds();

        // Set up game object with mesh;
        gameObject.AddComponent(typeof(MeshRenderer));
        MeshFilter filter = gameObject.AddComponent(typeof(MeshFilter)) as MeshFilter;
        filter.mesh = msh;
        Renderer renderer = GetComponent<MeshRenderer>();
        Material mat = Resources.Load("SelectionMap/pathMat") as Material;
        renderer.material = mat;
    }

    public void setRectangle(Vector2 upLeft, Vector2 upRight, Vector2 bottomLeft, Vector2 bottomRight)
    {
        this.upLeft = upLeft;
        this.upRight = upRight;
        this.bottomLeft = bottomLeft;
        this.bottomRight = bottomRight;
    }

    private Vector2 getCasteljauPoint(int r, int i, float t, List<Vector2> points)
    {
        if (r == 0) return points[i];

        Vector2 p1 = getCasteljauPoint(r - 1, i, t, points);
        Vector2 p2 = getCasteljauPoint(r - 1, i + 1, t, points);

        return new Vector2(((1 - t) * p1.x + t * p2.x), ((1 - t) * p1.y + t * p2.y));
    }
}
