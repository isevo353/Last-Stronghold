using UnityEngine;
using UnityEngine.UI;

public class SimpleHealthBar : MonoBehaviour
{
    private Slider _slider;

    void Awake()
    {
        CacheSlider();
    }

    void CacheSlider()
    {
        if (_slider == null)
            _slider = GetComponentInChildren<Slider>();
    }

    public void UpdateHealth(float currentHealth, float maxHealth)
    {
        if (_slider == null) CacheSlider();
        if (_slider == null) return;

        _slider.maxValue = maxHealth;
        _slider.value = currentHealth;
    }
}
