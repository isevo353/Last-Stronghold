using UnityEngine;

using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    private SimpleHealthBar healthBar;

    void Start()
    {
        currentHealth = maxHealth;

        // Находим HealthBar автоматически
        healthBar = GetComponentInChildren<SimpleHealthBar>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not found on Gate!");
        }
        else
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        Debug.Log("Ворота созданы. HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Ворота получили урон: " + damage + ". Осталось HP: " + currentHealth);

        // Обновляем HealthBar
        if (healthBar != null)
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

        StartCoroutine(DamageFlash());

        if (currentHealth <= 0)
        {
            DestroyGate();
        }
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

    void DestroyGate()
    {
        Debug.Log("Ворота разрушены!");
        Destroy(gameObject);
    }
}