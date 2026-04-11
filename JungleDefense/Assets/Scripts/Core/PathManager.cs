using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public Transform[] waypoints;

    private void Awake()
    {
        List<Transform> points = new List<Transform>();

        foreach (Transform child in transform)
        {
            points.Add(child);
        }

        waypoints = points.ToArray();
    }
}