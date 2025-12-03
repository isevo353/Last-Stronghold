using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestEnemy : MonoBehaviour
{
    [Header("Movement")]
    private List<Transform> waypoints;
    private int currentWaypointIndex = 0;
    public float speed = 2f;
    public float startDelay = 0f; 

    [Header("Combat")]
    public int maxHealth = 100;
    public int currentHealth;
    public SimpleHealthBar healthBar;

    [Header("Attack")]
    public int damageToGate = 20;
    public float attackCooldown = 1f;
    private Gate targetGate;
    private float lastAttackTime;
    private bool isAttacking = false;
    private bool canMove = true;

    [Header("Visual Effects")]
    public GameObject damageTextPrefab; // Префаб текста урона


    void Start()
    {
        // Задержка старта
        if (startDelay > 0)
        {
            canMove = false;
            StartCoroutine(StartDelay());
        }

        // Движение
        waypoints = PathManager.Instance.GetWaypoints();
        if (waypoints != null && waypoints.Count > 0)
        {
            transform.position = waypoints[0].position;
        }

        // Здоровье
        currentHealth = maxHealth;
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        // Коллайдер
        if (GetComponent<Collider2D>() == null)
        {
            gameObject.AddComponent<CircleCollider2D>().radius = 0.5f;
        }

        // Слой
        gameObject.layer = LayerMask.NameToLayer("Enemy");
    }

    IEnumerator StartDelay()
    {
        yield return new WaitForSeconds(startDelay);
        canMove = true;
    }

    void Update()
    {
        if (!canMove) return;
        Move();
    }

    void Move()
    {
        if (waypoints == null || currentWaypointIndex >= waypoints.Count)
        {
            FindGate();
            return;
        }

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

    void FindGate()
    {
        if (isAttacking) return;

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

    void FixedUpdate()
    {
        if (isAttacking && targetGate != null)
        {
            AttackGate();
        }
    }

    void AttackGate()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            targetGate.TakeDamage(damageToGate);
            lastAttackTime = Time.time;
        }
    }

    public void TakeDamage(int damage)
    {
        if (currentHealth <= 0) return;

        currentHealth -= damage;
        Debug.Log("ОРК получил " + damage + " урона! Осталось: " + currentHealth);

        StartCoroutine(DamageFlash()); 

        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        if (currentHealth <= 0)
        {
            Die();
        }
        // СОЗДАНИЕ ТЕКСТА УРОНА:
        if (damageTextPrefab != null)
        {
            GameObject textObj = Instantiate(damageTextPrefab, transform);
            textObj.transform.localPosition = new Vector3(0, 1.5f, 0); // Над головой
            textObj.GetComponent<DamagePopup>().SetDamage(damage);
        }
    }

    void Die()
    {
        Debug.Log("Орк умер!");

        // Отключаем всё
        Collider2D col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        if (healthBar != null)
            healthBar.gameObject.SetActive(false);

        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null) sprite.enabled = false;

        // Деньги
        if (GameManager.Instance != null)
            GameManager.Instance.AddMoney(5);

        Destroy(gameObject, 0.2f);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, 1f);
    }
System.Collections.IEnumerator DamageFlash()
    {
        SpriteRenderer sprite = GetComponent<SpriteRenderer>();
        if (sprite != null)
        {
            Color original = sprite.color;
            sprite.color = Color.red;
            yield return new WaitForSeconds(0.1f);
            sprite.color = original;
        }
    }
}