using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBar : MonoBehaviour
{
    private Slider slider;

    void Start()
    {
        // Автоматически находим Slider на этом объекте
        slider = GetComponentInChildren<Slider>();
        if (slider == null)
        {
            Debug.LogError("Slider not found on " + gameObject.name);
        }
    }

    public void UpdateHealth(int currentHealth, int maxHealth)
    {
        if (slider != null)
        {
            slider.value = currentHealth;
            slider.maxValue = maxHealth;
        }
    }
}
