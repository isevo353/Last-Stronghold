using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBar : MonoBehaviour
{
    private Slider slider;
    private Image fill;

    void Start()
    {
        slider = GetComponentInChildren<Slider>();
        fill = slider?.fillRect?.GetComponent<Image>();

        if (slider == null)
            Debug.LogError("Не найден Slider на HealthBar!");
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (slider == null) return;

        slider.maxValue = maxHealth;
        slider.value = currentHealth;

    }
}
