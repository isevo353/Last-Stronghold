using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Slider brightnessSlider;
    public Slider musicSlider;
    public Button backButton;

    private const string MUSIC_KEY = "MusicVolume";

    void Start()
    {
        // ЗАГРУЖАЕМ ЯРКОСТЬ ИЗ BrightnessManager
        if (BrightnessManager.Instance != null)
        {
            float savedBrightness = BrightnessManager.Instance.GetBrightness();

            if (brightnessSlider != null)
            {
                brightnessSlider.minValue = 0f;
                brightnessSlider.maxValue = 1f;
                brightnessSlider.value = savedBrightness;
                brightnessSlider.onValueChanged.AddListener(OnBrightnessChange);
            }
        }
        else
        {
            Debug.LogError("BrightnessManager не найден! Добавьте его в сцену.");
        }

        // ЗАГРУЖАЕМ МУЗЫКУ
        float savedMusic = PlayerPrefs.GetFloat(MUSIC_KEY, 0.7f);
        if (musicSlider != null)
        {
            musicSlider.minValue = 0f;
            musicSlider.maxValue = 1f;
            musicSlider.value = savedMusic;
            musicSlider.onValueChanged.AddListener(OnMusicChange);
            OnMusicChange(savedMusic);
        }

        // КНОПКА НАЗАД
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMenu);
        }
    }

    void OnBrightnessChange(float value)
    {
        // МЕНЯЕМ ЯРКОСТЬ ЧЕРЕЗ МЕНЕДЖЕР
        if (BrightnessManager.Instance != null)
        {
            BrightnessManager.Instance.SetBrightness(value);
        }
    }

    void OnMusicChange(float value)
    {
        if (MusicManager.Instance != null)
        {
            MusicManager.Instance.SetVolume(value);
        }

        PlayerPrefs.SetFloat(MUSIC_KEY, value);
        PlayerPrefs.Save();
    }

    void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}