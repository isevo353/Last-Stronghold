using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StoryLevelManager : MonoBehaviour
{
    public int maxWavesToWin = 20;

    private GameObject _victoryPanel;
    private bool _isVictory;
    private string _victoryTitle = "ПОБЕДА";
    private string _victorySubtitle = "Вы прошли 20 волн!";

    public static bool IsStoryLevelScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        return sceneName == "Level1Scene" || sceneName == "Level2Scene" || sceneName == "Level3Scene";
    }

    public bool CanStartWave(int currentWave)
    {
        return !_isVictory && currentWave < maxWavesToWin;
    }

    public bool TryHandleVictory(int currentWave)
    {
        if (_isVictory || currentWave < maxWavesToWin || !IsStoryLevelScene())
        {
            return false;
        }

        _isVictory = true;
        SaveStoryProgress();
        ShowVictoryPanel();
        return true;
    }

    void Awake()
    {
        ApplyCampaignSettings();
    }

    void ApplyCampaignSettings()
    {
        CampaignSettings settings = Resources.Load<CampaignSettings>("CampaignSettings");
        if (settings == null)
        {
            _victorySubtitle = $"Вы прошли {maxWavesToWin} волн!";
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        CampaignSettings.LevelSettings levelSettings = settings.GetSettingsForScene(sceneName);

        if (levelSettings != null)
        {
            maxWavesToWin = Mathf.Max(1, levelSettings.maxWavesToWin);
            _victoryTitle = string.IsNullOrEmpty(levelSettings.victoryTitle) ? settings.defaultVictoryTitle : levelSettings.victoryTitle;
            _victorySubtitle = string.IsNullOrEmpty(levelSettings.victorySubtitle) ? settings.defaultVictorySubtitle : levelSettings.victorySubtitle;
            return;
        }

        maxWavesToWin = Mathf.Max(1, settings.defaultMaxWavesToWin);
        _victoryTitle = settings.defaultVictoryTitle;
        _victorySubtitle = settings.defaultVictorySubtitle;
    }

    void SaveStoryProgress()
    {
        string sceneName = SceneManager.GetActiveScene().name;
        PlayerPrefs.SetInt($"StoryLevelCompleted_{sceneName}", 1);
        PlayerPrefs.Save();
    }

    void ShowVictoryPanel()
    {
        if (_victoryPanel == null)
        {
            _victoryPanel = BuildVictoryPanel();
        }

        _victoryPanel.SetActive(true);
        Time.timeScale = 0f;
    }

    GameObject BuildVictoryPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[StoryLevelManager] Canvas not found for victory panel.");
            return new GameObject("StoryVictoryPanel_Fallback");
        }

        GameObject panel = new GameObject("StoryVictoryPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 0.5f);
        panelRect.anchorMax = new Vector2(0.5f, 0.5f);
        panelRect.pivot = new Vector2(0.5f, 0.5f);
        panelRect.sizeDelta = new Vector2(460, 240);
        panelRect.anchoredPosition = Vector2.zero;

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0f, 0f, 0.8f);

        CreateText(panel.transform, _victoryTitle, 42, new Vector2(0, 70), new Vector2(420, 60), Color.yellow);
        CreateText(panel.transform, _victorySubtitle, 28, new Vector2(0, 20), new Vector2(420, 50), Color.white);

        CreateButton(panel.transform, "Заново", new Vector2(-110, -65), () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        CreateButton(panel.transform, "К сюжету", new Vector2(110, -65), () =>
        {
            Time.timeScale = 1f;
            SceneManager.LoadScene("StoryMenuScene");
        });

        return panel;
    }

    void CreateText(Transform parent, string value, int fontSize, Vector2 pos, Vector2 size, Color color)
    {
        GameObject go = new GameObject(value, typeof(RectTransform), typeof(Text));
        go.transform.SetParent(parent, false);

        RectTransform rect = go.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = size;

        Text text = go.GetComponent<Text>();
        text.text = value;
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = fontSize;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = color;
    }

    void CreateButton(Transform parent, string label, Vector2 pos, UnityEngine.Events.UnityAction onClick)
    {
        GameObject buttonObject = new GameObject(label + "Button", typeof(RectTransform), typeof(Image), typeof(Button));
        buttonObject.transform.SetParent(parent, false);

        RectTransform rect = buttonObject.GetComponent<RectTransform>();
        rect.anchorMin = new Vector2(0.5f, 0.5f);
        rect.anchorMax = new Vector2(0.5f, 0.5f);
        rect.pivot = new Vector2(0.5f, 0.5f);
        rect.anchoredPosition = pos;
        rect.sizeDelta = new Vector2(180, 45);

        Image image = buttonObject.GetComponent<Image>();
        image.color = new Color(1f, 1f, 1f, 0.95f);

        Button button = buttonObject.GetComponent<Button>();
        button.onClick.AddListener(onClick);

        CreateText(buttonObject.transform, label, 24, Vector2.zero, new Vector2(170, 40), Color.black);
    }
}
