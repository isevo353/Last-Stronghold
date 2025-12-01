using UnityEngine;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        Debug.Log("Ворота созданы. HP: " + currentHealth);
    }

    // Ворота получают урон от врагов
    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("Ворота получили урон: " + damage + ". Осталось HP: " + currentHealth);

        // Эффект при получении урона
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
        Debug.Log("Ворота уничтожены!");
        if (GameManager.Instance != null) 
            GameManager.Instance.TakeLives(1);
        Destroy(gameObject);
    }

}