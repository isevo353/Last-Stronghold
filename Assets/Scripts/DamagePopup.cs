using UnityEngine;
using UnityEngine.UI;

public class DamagePopup : MonoBehaviour
{
    public Text damageText;
    public float moveSpeed = 1f;
    public float lifeTime = 1f;
    
    private float timer = 0f;
    private Vector3 startPosition;
    
    void Start()
    {
        startPosition = transform.localPosition;
    }
    
    void Update()
    {
        timer += Time.deltaTime;
        
        // Поднимаем вверх
        transform.localPosition = startPosition + new Vector3(0, moveSpeed * timer, 0);
        
        // Плавно исчезаем
        Color color = damageText.color;
        color.a = 1f - (timer / lifeTime);
        damageText.color = color;
        
        // Уничтожаем когда время вышло
        if (timer >= lifeTime)
            Destroy(gameObject);
    }
    
    public void SetDamage(int damage)
    {
        if (damageText != null)
            damageText.text = "-" + damage;
    }
}
