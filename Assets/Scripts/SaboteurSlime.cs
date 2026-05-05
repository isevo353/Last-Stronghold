using System.Collections;
using UnityEngine;

[RequireComponent(typeof(TestEnemy))]
public class SaboteurSlime : MonoBehaviour
{
    [Header("Sabotage")]
    public float sabotageRadius = 1.2f;
    public float stopDuration = 0.2f;
    public bool stopDuringSabotage = true;

    private TestEnemy _enemy;
    private bool _hasSabotaged = false;

    void Awake()
    {
        _enemy = GetComponent<TestEnemy>();
    }

    void Update()
    {
        // Если уже саботировал - ничего не делаем
        if (_hasSabotaged)
            return;

        // Ищем ближайшую башню в радиусе
        Tower target = FindTargetTower();
        if (target == null)
            return;

        // Останавливаем движение слизня (если нужно)
        if (stopDuringSabotage && _enemy != null)
        {
            _enemy.SetMovementEnabled(false);
            StartCoroutine(ResumeMoveAfter(stopDuration));
        }

        // САБОТАЖ БАШНИ (используем твой метод из Tower)
        target.DowngradeOrDestroyByEnemy();

        _hasSabotaged = true;

        // СЛИЗЕНЬ УМИРАЕТ ПОСЛЕ САБОТАЖА
        Die();
    }

    Tower FindTargetTower()
    {
        // Находим все башни через FindObjectsOfType (так как у тебя нет статического списка)
        Tower[] allTowers = FindObjectsOfType<Tower>();

        if (allTowers == null || allTowers.Length == 0)
            return null;

        float bestDist = Mathf.Infinity;
        Tower best = null;
        Vector2 position = transform.position;

        foreach (Tower tower in allTowers)
        {
            if (tower == null)
                continue;

            float distance = Vector2.Distance(position, tower.transform.position);

            // Проверяем, что башня в радиусе и это ближайшая
            if (distance <= sabotageRadius && distance < bestDist)
            {
                bestDist = distance;
                best = tower;
            }
        }

        return best;
    }

    IEnumerator ResumeMoveAfter(float delay)
    {
        if (delay > 0f)
            yield return new WaitForSeconds(delay);

        if (_enemy != null && !_hasSabotaged) // Если ещё жив - возобновляем движение
            _enemy.SetMovementEnabled(true);
    }

    private void Die()
    {
        if (_enemy != null)
        {
            // Используем существующий метод Die() из TestEnemy
            _enemy.Die();
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void OnDrawGizmosSelected()
    {
        if (sabotageRadius <= 0f) return;
        Gizmos.color = new Color(0.6f, 0.2f, 0.8f, 0.8f);
        Gizmos.DrawWireSphere(transform.position, sabotageRadius);
    }
}