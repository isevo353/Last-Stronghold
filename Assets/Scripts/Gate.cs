using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    private SimpleHealthBar healthBar;
    private DefeatMenu defeatMenu;
    private EnemySpawner enemySpawner;

    void Start()
    {
        currentHealth = maxHealth;

        // пїЅпїЅпїЅпїЅпїЅпїЅпїЅ HealthBar пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ
        healthBar = GetComponentInChildren<SimpleHealthBar>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not found on Gate!");
        }
        else
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

<<<<<<< Updated upstream
	// пїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅ
=======
	enemySpawner = FindObjectOfType<EnemySpawner>();
	// Ќаходим меню поражениЯ в сцене
>>>>>>> Stashed changes
        defeatMenu = FindObjectOfType<DefeatMenu>();
        if (defeatMenu == null)
        {
            Debug.LogError("DefeatMenu пїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅ пїЅ пїЅпїЅпїЅпїЅпїЅ! пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅ пїЅпїЅ Canvas.");
        }

        Debug.Log("пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅ. HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("пїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅ: " + damage + ". пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ HP: " + currentHealth);

        // пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ HealthBar
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
        Debug.Log("?????? ??????????!");
        // HP ????? ? ???????????? пїЅ?????пїЅ; ???????/???? ?????? ????? DefeatMenu

        // ?????????? ???? ?????????
        if (defeatMenu != null)
        {
            defeatMenu.ShowDefeatMenu();
        }
        else
        {
            Debug.LogError("пїЅпїЅ пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅ пїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅпїЅ: defeatMenu пїЅпїЅпїЅпїЅпїЅ null!");
        }
	enemySpawner.ResetWaves();

        Destroy(gameObject);
    }

}