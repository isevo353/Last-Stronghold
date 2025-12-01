using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [Header("Movement")]
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 2f;
    public float startDelay = 0f; // ������� ������ ��� �������

    [Header("Combat")]
    public int maxHealth = 100;
    public int damageToGate = 20;
    public float attackCooldown = 1f;

    private int currentHealth;
    private Gate targetGate;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool canMove = false; // ������� ������ ��� �������

    void Start()
    {
        waypoints = PathManager.Instance.GetWaypoints();
        transform.position = waypoints[0].position;
        currentHealth = maxHealth;

        // ������� ������ ��� 4 �������
        if (startDelay > 0)
        {
            StartCoroutine(StartMovingAfterDelay());
        }
        else
        {
            canMove = true;
        }
    }

    // ������� ������ ���� �����
    System.Collections.IEnumerator StartMovingAfterDelay()
    {
        yield return new WaitForSeconds(startDelay);
        canMove = true;
    }

    void Update()
    {
        if (!canMove) return; // ������� ������ ��� ��������

        if (!isAttacking)
        {
            MoveAlongPath();
        }
        else
        {
            AttackGate();
        }
    }

    // �Ѩ ��������� ��� ���������
    void MoveAlongPath()
    {
        if (currentWaypointIndex < waypoints.Count)
        {
            Transform target = waypoints[currentWaypointIndex];
            transform.position = Vector2.MoveTowards(
                transform.position,
                target.position,
                speed * Time.deltaTime
            );

            if (Vector2.Distance(transform.position, target.position) < 0.1f)
            {
                currentWaypointIndex++;
            }
        }
        else
        {

            FindGateAtEnd();
        }
    }

    void FindGateAtEnd()
    {

        Collider2D[] colliders = Physics2D.OverlapCircleAll(transform.position, 1f);
        foreach (var collider in colliders)
        {
            Gate gate = collider.GetComponent<Gate>();
            if (gate != null)
            {
                targetGate = gate;
                isAttacking = true;
                return;
            }
        }
    }

    void AttackGate()
    {
        if (targetGate != null)
        {

            if (Time.time >= lastAttackTime + attackCooldown)
            {
                targetGate.TakeDamage(damageToGate);
                lastAttackTime = Time.time;
                Debug.Log("Враг атакует ворота! Урон: " + damageToGate);
            }
        }
        else
        {
            isAttacking = false;
        }
    }


    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return; // ← если уже мёртв, больше урона не берём

        currentHealth -= damage;
        Debug.Log("Враг получил урон: " + damage + ". Осталось HP: " + currentHealth);

        if (currentHealth <= 0)
        {
            Die();
        }
    }


    void Die()
    {
        Debug.Log("Враг мёртв!");

        gameObject.SetActive(false); // ← отключаем объект до удаления

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddMoney(5);
        }

        Destroy(gameObject);
    }

}
