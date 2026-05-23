using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance { get; private set; }

    public readonly List<Transform> waypoints = new List<Transform>();

    public Transform startPoint;
    public Transform endPoint;

    private const float NeighborDistance = 1.1f;
    private const float NeighborDistanceSqr = NeighborDistance * NeighborDistance;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public void SetPath(List<Vector3> positions)
    {
        ClearPath();

        List<Vector3> sortedPath = SortPath(positions);

        for (int i = 0; i < sortedPath.Count; i++)
        {
            GameObject point = new GameObject($"Waypoint_{i}");
            point.transform.position = sortedPath[i];
            point.transform.SetParent(transform);
            waypoints.Add(point.transform);
        }

        if (waypoints.Count == 0)
        {
            Debug.LogError("Path is empty. Add P cells to LevelData.");
            return;
        }

        startPoint = waypoints[0];
        endPoint = waypoints[waypoints.Count - 1];
    }

    public void ClearPath()
    {
        for (int i = waypoints.Count - 1; i >= 0; i--)
        {
            if (waypoints[i] != null)
            {
                Destroy(waypoints[i].gameObject);
            }
        }

        waypoints.Clear();
        startPoint = null;
        endPoint = null;
    }

    private List<Vector3> SortPath(List<Vector3> positions)
    {
        List<Vector3> result = new List<Vector3>();

        if (positions == null || positions.Count == 0)
        {
            Debug.LogError("Path is empty.");
            return result;
        }

        List<Vector3> remaining = new List<Vector3>(positions);
        Vector3 current = FindEndpoint(remaining);

        result.Add(current);
        remaining.Remove(current);

        while (remaining.Count > 0)
        {
            int nextIndex = FindNeighborIndex(current, remaining);

            if (nextIndex < 0)
            {
                Debug.LogError("Path is broken. Check that P cells are connected.");
                break;
            }

            current = remaining[nextIndex];
            result.Add(current);
            remaining.RemoveAt(nextIndex);
        }

        return result;
    }

    private Vector3 FindEndpoint(List<Vector3> positions)
    {
        foreach (Vector3 position in positions)
        {
            int neighbors = 0;

            foreach (Vector3 other in positions)
            {
                if (position == other)
                {
                    continue;
                }

                if ((position - other).sqrMagnitude < NeighborDistanceSqr)
                {
                    neighbors++;
                }
            }

            if (neighbors == 1)
            {
                return position;
            }
        }

        Debug.LogWarning("Path has no clear endpoint. Using first P cell as start.");
        return positions[0];
    }

    private int FindNeighborIndex(Vector3 current, List<Vector3> remaining)
    {
        for (int i = 0; i < remaining.Count; i++)
        {
            if ((current - remaining[i]).sqrMagnitude < NeighborDistanceSqr)
            {
                return i;
            }
        }

        return -1;
    }
}
