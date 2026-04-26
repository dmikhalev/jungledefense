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

        if (positions == null || positions.Count == 0)
        {
            Debug.LogError("Путь пуст!");
            return result;
        }

        // копия списка
        List<Vector3> remaining = new List<Vector3>(positions);

        Vector3 current = remaining[0];
        result.Add(current);
        remaining.Remove(current);

        while (remaining.Count > 0)
        {
            Vector3 next = Vector3.zero;
            bool found = false;

            foreach (var pos in remaining)
            {
                // ищем соседнюю клетку (очень важно)
                if (Vector3.Distance(current, pos) < 1.1f)
                {
                    next = pos;
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Debug.LogError("Путь разорван! Проверь P клетки");
                break;
            }

            result.Add(next);
            remaining.Remove(next);
            current = next;
        }

        return result;
    }
}