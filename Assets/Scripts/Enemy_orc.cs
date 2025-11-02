using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 2f;

    void Start()
    {
        // Получаем путь
        waypoints = PathManager.Instance.GetWaypoints();
        // Стартуем с первой точки
        transform.position = waypoints[0].position;
    }

    void Update()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            // Двигаемся к текущей точке
            Transform target = waypoints[currentWaypointIndex];
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            // Если дошли до точки - переходим к следующей
            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
    }
}
