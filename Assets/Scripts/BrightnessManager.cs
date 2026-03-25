using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    private Image darkOverlay;
    private const string BRIGHTNESS_KEY = "Brightness";
    private float currentBrightness = 1f;

    void Awake()
    {
        // СИНГЛТОН - СОХРАНЯЕТСЯ МЕЖДУ СЦЕНАМИ
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ЗАГРУЖАЕМ СОХРАНЁННУЮ ЯРКОСТЬ
        currentBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);

        // СОЗДАЁМ ЗАТЕМНЯЮЩИЙ СЛОЙ ПОСЛЕ ЗАГРУЗКИ СЦЕНЫ
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;

        Debug.Log($"BrightnessManager создан. Яркость: {currentBrightness}");
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        // ПРИ ЗАГРУЗКЕ НОВОЙ СЦЕНЫ - СОЗДАЁМ СЛОЙ ЗАНОВО
        CreateDarkOverlay();
        ApplyBrightness(currentBrightness);
        Debug.Log($"Сцена {scene.name} загружена, применяем яркость");
    }

    void CreateDarkOverlay()
    {
        // НАХОДИМ CANVAS В НОВОЙ СЦЕНЕ
        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning("Canvas не найден, ждём...");
            return;
        }

        // УДАЛЯЕМ СТАРЫЙ СЛОЙ, ЕСЛИ ЕСТЬ
        GameObject oldOverlay = GameObject.Find("DarkOverlay");
        if (oldOverlay != null) Destroy(oldOverlay);

        // СОЗДАЁМ НОВЫЙ СЛОЙ
        GameObject overlayObj = new GameObject("DarkOverlay");
        overlayObj.transform.SetParent(canvas.transform, false);

        darkOverlay = overlayObj.AddComponent<Image>();
        darkOverlay.color = new Color(0, 0, 0, 0);
        darkOverlay.raycastTarget = false; // ЧТОБЫ НЕ БЛОКИРОВАТЬ КНОПКИ

        // РАСТЯГИВАЕМ НА ВЕСЬ ЭКРАН
        RectTransform rect = darkOverlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        // СТАВИМ ПОВЕРХ ВСЕГО
        darkOverlay.transform.SetAsLastSibling();

        Debug.Log("Затемняющий слой создан в сцене");
    }

    public void SetBrightness(float value)
    {
        currentBrightness = Mathf.Clamp01(value);

        // СОХРАНЯЕМ
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, currentBrightness);
        PlayerPrefs.Save();

        // ПРИМЕНЯЕМ
        ApplyBrightness(currentBrightness);

        Debug.Log($"Яркость установлена: {currentBrightness}");
    }

    void ApplyBrightness(float value)
    {
        if (darkOverlay != null)
        {
            float alpha = (1 - value) * 0.8f;
            darkOverlay.color = new Color(0, 0, 0, alpha);
        }
    }

    public float GetBrightness()
    {
        return currentBrightness;
    }

    void OnDestroy()
    {
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}