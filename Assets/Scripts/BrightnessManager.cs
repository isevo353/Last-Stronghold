using UnityEngine;
using UnityEngine.UI;

public class BrightnessManager : MonoBehaviour
{
    public static BrightnessManager Instance { get; private set; }

    private Image darkOverlay;
    private const string BRIGHTNESS_KEY = "Brightness";
    private const string FIRST_LAUNCH_KEY = "FirstLaunch";
    private float currentBrightness = 1f;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // ===== ПРИНУДИТЕЛЬНЫЙ СБРОС ДЛЯ ТЕСТА =====
        PlayerPrefs.DeleteKey(FIRST_LAUNCH_KEY);
        PlayerPrefs.DeleteKey(BRIGHTNESS_KEY);
        // =========================================

        // ПРОВЕРЯЕМ ПЕРВЫЙ ЛИ ЭТО ЗАПУСК
        if (!PlayerPrefs.HasKey(FIRST_LAUNCH_KEY))
        {
            Debug.Log("Первый запуск! Устанавливаю настройки по умолчанию");

            // СТАВИМ ЯРКОСТЬ ПО УМОЛЧАНИЮ = 1
            currentBrightness = 1f;
            PlayerPrefs.SetFloat(BRIGHTNESS_KEY, currentBrightness);

            // СОХРАНЯЕМ, ЧТО ПЕРВЫЙ ЗАПУСК УЖЕ БЫЛ
            PlayerPrefs.SetInt(FIRST_LAUNCH_KEY, 1);
            PlayerPrefs.Save();
        }
        else
        {
            // ЗАГРУЖАЕМ СОХРАНЁННУЮ ЯРКОСТЬ
            currentBrightness = PlayerPrefs.GetFloat(BRIGHTNESS_KEY, 1f);
            Debug.Log($"Загружена сохранённая яркость: {currentBrightness}");
        }

        Debug.Log($"BrightnessManager создан. Яркость: {currentBrightness}");

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
        Invoke(nameof(CreateDarkOverlay), 0.1f);
    }

    void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"Сцена загружена: {scene.name}");
        darkOverlay = null;
        Invoke(nameof(CreateDarkOverlay), 0.1f);
        Invoke(nameof(CreateDarkOverlay), 0.3f);
    }

    void CreateDarkOverlay()
    {
        if (darkOverlay != null)
        {
            ApplyBrightness();
            return;
        }

        GameObject existingOverlay = GameObject.Find("DarkOverlay");
        if (existingOverlay != null)
        {
            darkOverlay = existingOverlay.GetComponent<Image>();
            if (darkOverlay != null)
            {
                Debug.Log("Найден существующий слой");
                ApplyBrightness();
                return;
            }
        }

        Canvas canvas = FindFirstObjectByType<Canvas>();
        if (canvas == null)
        {
            Debug.LogWarning($"Canvas не найден");
            return;
        }

        if (canvas.renderMode != RenderMode.ScreenSpaceOverlay)
        {
            canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        }

        GameObject overlayObj = new GameObject("DarkOverlay");
        overlayObj.transform.SetParent(canvas.transform, false);

        darkOverlay = overlayObj.AddComponent<Image>();
        darkOverlay.raycastTarget = false;

        RectTransform rect = darkOverlay.GetComponent<RectTransform>();
        rect.anchorMin = Vector2.zero;
        rect.anchorMax = Vector2.one;
        rect.offsetMin = Vector2.zero;
        rect.offsetMax = Vector2.zero;

        darkOverlay.transform.SetAsLastSibling();
        ApplyBrightness();

        Debug.Log("Затемняющий слой создан");
    }

    public void SetBrightness(float value)
    {
        currentBrightness = Mathf.Clamp01(value);
        PlayerPrefs.SetFloat(BRIGHTNESS_KEY, currentBrightness);
        PlayerPrefs.Save();
        ApplyBrightness();
        Debug.Log($"Яркость: {currentBrightness * 100}%");
    }

    void ApplyBrightness()
    {
        if (darkOverlay != null)
        {
            float alpha = (1f - currentBrightness) * 0.8f;
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