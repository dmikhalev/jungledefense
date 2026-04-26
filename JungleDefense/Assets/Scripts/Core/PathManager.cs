using UnityEngine;
using System.Collections.Generic;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;

    public List<Transform> waypoints = new List<Transform>();

    public Transform startPoint;
    public Transform endPoint;

    void Awake()
    {
        Instance = this;
    }

    public void SetPath(List<Vector3> positions)
    {
        waypoints.Clear();

        List<Vector3> sortedPath = SortPath(positions);

        for (int i = 0; i < sortedPath.Count; i++)
        {
            GameObject point = new GameObject("Waypoint_" + i);
            point.transform.position = sortedPath[i];
            waypoints.Add(point.transform);
        }

        startPoint = waypoints[0];
        endPoint = waypoints[waypoints.Count - 1];
    }

    List<Vector3> SortPath(List<Vector3> positions)
    {
        List<Vector3> result = new List<Vector3>();

        // начинаем с первой точки (например самой левой)
        Vector3 current = positions[0];
        result.Add(current);

        positions.Remove(current);

        while (positions.Count > 0)
        {
            Vector3 next = positions[0];
            float minDist = Vector3.Distance(current, next);

            foreach (var pos in positions)
            {
                float dist = Vector3.Distance(current, pos);
                if (dist < minDist)
                {
                    minDist = dist;
                    next = pos;
                }
            }

            result.Add(next);
            positions.Remove(next);
            current = next;
        }

        return result;
    }
}