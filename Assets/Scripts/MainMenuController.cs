//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject _storyDonePanel;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "StoryMenuScene") return;

        if (AreAllStoryLevelsCompleted())
        {
            _storyDonePanel = BuildStoryCompletedPanel();
            _storyDonePanel.SetActive(true);
        }
    }

    // ����� ��� ������ "������" / "����������� �����"
    public void PlayGame()
    {
        Debug.Log("�������� ������� �����");
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene"); // ��� ����� ������� �����
    }

    // ����� ��� ������ "������"
    public void OpenStoryMenu()
    {
        Debug.Log("Открытие меню сюжета");
        Time.timeScale = 1f;
        SceneManager.LoadScene("StoryMenuScene");
    }

    public void LoadLevel1()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level1Scene");
    }

    public void LoadLevel2()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level2Scene");
    }

    public void LoadLevel3()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level3Scene");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

    // ����� ��� ������ "���������"
    public void OpenSettings()
    {
        Debug.Log("Открытие настроек");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SettingsScene");
    }

    // ����� ��� ������ "�����"
    public void QuitGame()
    {
        Debug.Log("����� �� ����");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    bool AreAllStoryLevelsCompleted()
    {
        return PlayerPrefs.GetInt("StoryLevelCompleted_Level1Scene", 0) == 1
            && PlayerPrefs.GetInt("StoryLevelCompleted_Level2Scene", 0) == 1
            && PlayerPrefs.GetInt("StoryLevelCompleted_Level3Scene", 0) == 1;
    }

    GameObject BuildStoryCompletedPanel()
    {
        Canvas canvas = FindObjectOfType<Canvas>();
        if (canvas == null)
        {
            Debug.LogError("[MainMenuController] Canvas not found in StoryMenuScene.");
            return new GameObject("StoryCompletedPanel_Fallback");
        }

        Transform existing = canvas.transform.Find("StoryCompletedPanel");
        if (existing != null)
        {
            return existing.gameObject;
        }

        GameObject panel = new GameObject("StoryCompletedPanel", typeof(RectTransform), typeof(Image));
        panel.transform.SetParent(canvas.transform, false);

        RectTransform panelRect = panel.GetComponent<RectTransform>();
        panelRect.anchorMin = new Vector2(0.5f, 1f);
        panelRect.anchorMax = new Vector2(0.5f, 1f);
        panelRect.pivot = new Vector2(0.5f, 1f);
        panelRect.anchoredPosition = new Vector2(0, -20);
        panelRect.sizeDelta = new Vector2(520, 90);

        Image panelImage = panel.GetComponent<Image>();
        panelImage.color = new Color(0f, 0.35f, 0f, 0.75f);

        GameObject textObject = new GameObject("StoryCompletedText", typeof(RectTransform), typeof(Text));
        textObject.transform.SetParent(panel.transform, false);

        RectTransform textRect = textObject.GetComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(500, 70);

        Text text = textObject.GetComponent<Text>();
        text.text = "ПРОЙДЕНО СЮЖЕТ";
        text.font = Resources.GetBuiltinResource<Font>("Arial.ttf");
        text.fontSize = 42;
        text.fontStyle = FontStyle.Bold;
        text.alignment = TextAnchor.MiddleCenter;
        text.color = Color.white;

        return panel;
    }

}