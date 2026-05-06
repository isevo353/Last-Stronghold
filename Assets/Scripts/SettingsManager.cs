using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class SettingsMenu : MonoBehaviour
{
    public Slider brightnessSlider;
    public Slider musicSlider;
    public Button backButton;

    void Start()
    {
        // ЯРКОСТЬ
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

        // МУЗЫКА
        if (MusicManager.Instance != null)
        {
            float savedMusic = MusicManager.Instance.GetVolume();

            if (musicSlider != null)
            {
                musicSlider.minValue = 0f;
                musicSlider.maxValue = 1f;
                musicSlider.value = savedMusic;
                musicSlider.onValueChanged.AddListener(OnMusicChange);
            }
        }

        // КНОПКА НАЗАД
        if (backButton != null)
        {
            backButton.onClick.AddListener(BackToMenu);
        }
    }

    void OnBrightnessChange(float value)
    {
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
    }

    void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
}