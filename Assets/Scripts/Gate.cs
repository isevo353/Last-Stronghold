using UnityEngine;
using System.Collections;

public class Gate : MonoBehaviour
{
    [Header("Gate Settings")]
    public int maxHealth = 200;
    public int currentHealth;

    private SimpleHealthBar healthBar;
    private DefeatMenu defeatMenu;

    void Start()
    {
        currentHealth = maxHealth;

        // ������� HealthBar �������������
        healthBar = GetComponentInChildren<SimpleHealthBar>();
        if (healthBar == null)
        {
            Debug.LogError("HealthBar not found on Gate!");
        }
        else
        {
            healthBar.UpdateHealth(currentHealth, maxHealth);
        }

	// ������� ���� ��������� � �����
        defeatMenu = FindObjectOfType<DefeatMenu>();
        if (defeatMenu == null)
        {
            Debug.LogError("DefeatMenu �� ������ � �����! �������� ��� �� Canvas.");
        }

        Debug.Log("������ �������. HP: " + currentHealth);
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        Debug.Log("������ �������� ����: " + damage + ". �������� HP: " + currentHealth);

        // ��������� HealthBar
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
        // HP ????? ? ???????????? �?????�; ???????/???? ?????? ????? DefeatMenu

        // ?????????? ???? ?????????
        if (defeatMenu != null)
        {
            defeatMenu.ShowDefeatMenu();
        }
        else
        {
            Debug.LogError("�� ���� �������� ���� ���������: defeatMenu ����� null!");
        }

        Destroy(gameObject);
    }

}