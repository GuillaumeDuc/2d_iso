using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlacePointsSelection : MonoBehaviour
{
    public static List<LocationPoint> generatePoint(int radius, int rejection, int width, int height)
    {
        GameObject circle = Resources.Load<GameObject>("SelectionMap/LocationPoint");
        List<Vector2> list = poissonDisk(radius, rejection, width, height);
        List<LocationPoint> locations = new List<LocationPoint>();

        list.ForEach(a =>
        {
            GameObject go = Instantiate(circle, a, Quaternion.identity);
            // set position
            go.GetComponent<LocationPoint>().position = go.transform.position;
            locations.Add(go.GetComponent<LocationPoint>());
        });

        return locations;
    }

    private static List<Vector2> poissonDisk(int radius, int rejection, int width, int height)
    {
        float cellSize = radius / Mathf.Sqrt(2);

        int[,] grid = new int[Mathf.CeilToInt(width / cellSize), Mathf.CeilToInt(height / cellSize)];
        Vector2 spawn = new Vector2(0.5f, height / 2);
        // Add finish point
        Vector2 finish = new Vector2(width - 0.5f, height / 2);
        List<Vector2> points = new List<Vector2>() { spawn, finish };
        List<Vector2> spawnPoints = new List<Vector2>() { spawn };

        while (spawnPoints.Count > 0)
        {
            int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
            Vector2 spawnCenter = spawnPoints[spawnIndex];
            bool candidateAccepted = false;

            for (int i = 0; i < rejection; i++)
            {
                float angle = UnityEngine.Random.value * Mathf.PI * 2;
                Vector2 dir = new Vector2(Mathf.Sin(angle), Mathf.Cos(angle));
                Vector2 candidate = spawnCenter + dir * UnityEngine.Random.Range(radius, 2 * radius);
                if (isValid(candidate, width, height, cellSize, radius, points, grid))
                {
                    points.Add(candidate);
                    spawnPoints.Add(candidate);
                    grid[(int)(candidate.x / cellSize), (int)(candidate.y / cellSize)] = points.Count;
                    candidateAccepted = true;
                    break;
                }
            }
            if (!candidateAccepted)
            {
                spawnPoints.RemoveAt(spawnIndex);
            }
        }
        return points;
    }

    private static bool isValid(Vector2 candidate, int width, int height, float cellSize, float radius, List<Vector2> points, int[,] grid)
    {
        if (candidate.x >= 2 &&
            candidate.x < width - 2 &&
            candidate.y >= 1 &&
            candidate.y < height - 1)
        {
            int cellX = (int)(candidate.x / cellSize);
            int cellY = (int)(candidate.y / cellSize);
            int searchStartX = Mathf.Max(0, cellX - 2);
            int searchEndX = Mathf.Min(cellX + 2, grid.GetLength(0) - 1);
            int searchStartY = Mathf.Max(0, cellY - 2);
            int searchEndY = Mathf.Min(cellY + 2, grid.GetLength(1) - 1);

            for (int x = searchStartX; x <= searchEndX; x++)
            {
                for (int y = searchStartY; y <= searchEndY; y++)
                {
                    int pointIndex = grid[x, y] - 1;
                    if (pointIndex != -1)
                    {
                        float sqrDst = (candidate - points[pointIndex]).sqrMagnitude;
                        if (sqrDst < radius * radius)
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }
        return false;
    }
}
