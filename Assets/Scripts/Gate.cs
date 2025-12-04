using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    private SimpleHealthBar healthBar;
    public DefeatMenu defeatMenu;

    void Start()
    {
        currentHealth = maxHealth;

        // Ќаходим HealthBar автоматически
        healthBar = GetComponentInChildren<SimpleHealthBar>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not found on Gate!");
        }
        else
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }
	
	defeatMenu = FindObjectOfType<DefeatMenu>();
        if (defeatMenu == null)
        {
            Debug.LogError("DefeatMenu не найден в сцене! Добавьте его на Canvas.");
        }

        Debug.Log("¬орота созданы. HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("¬орота получили урон: " + damage + ". ќсталось HP: " + currentHealth);

        // ќбновл€ем HealthBar
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
        Debug.Log("¬орота уничтожены!");
	
	// Показываем меню поражения
        if (defeatMenu != null)
        {
            defeatMenu.ShowDefeatMenu();
        }
        else
        {
            Debug.LogError("Не могу показать меню поражения: defeatMenu равен null!");
        }

        if (GameManager.Instance != null) 
            GameManager.Instance.TakeLives(1);

	
	Destroy(gameObject);
    }

}