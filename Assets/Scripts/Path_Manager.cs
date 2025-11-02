using System.Collections.Generic;
using UnityEngine;

public class PathManager : MonoBehaviour
{
    public static PathManager Instance;

    [SerializeField] private List<Transform> waypoints = new List<Transform>();

    void Awake()
    {
        Instance = this;
        CollectWaypoints();
    }

    void CollectWaypoints()
    {
        waypoints.Clear();
        foreach (Transform child in transform)
        {
            if (child.name.StartsWith("Waypoint"))
            {
                waypoints.Add(child);
            }
        }

        // —ортируем по имени чтобы Waypoint_1, Waypoint_2 и т.д.
        waypoints.Sort((a, b) => a.name.CompareTo(b.name));
    }

    public List<Transform> GetWaypoints()
    {
        return waypoints;
    }


    void OnDrawGizmos()
    {
        if (waypoints.Count == 0) CollectWaypoints();

        Gizmos.color = Color.red;
        for (int i = 0; i < waypoints.Count - 1; i++)
        {
            if (waypoints[i] != null && waypoints[i + 1] != null)
                Gizmos.DrawLine(waypoints[i].position, waypoints[i + 1].position);
        }
    }
}
