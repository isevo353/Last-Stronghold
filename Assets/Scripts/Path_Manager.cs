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
            // »щем все объекты с "Waypoint" в имени
            if (child.name.Contains("Waypoint"))
            {
                waypoints.Add(child);
            }
        }

        // —ортируем по имени
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
