//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    private GameObject _storyDonePanel;
    private const int StoryLevelCount = 6;

    void Start()
    {
        if (SceneManager.GetActiveScene().name != "StoryMenuScene") return;

        EnsureExtendedStoryButtons();
        UpdateBonusButtonState();

        if (AreAllStoryLevelsCompleted())
        {
            _storyDonePanel = BuildStoryCompletedPanel();
            _storyDonePanel.SetActive(true);
        }
    }

 
    public void PlayGame()
    {
        Debug.Log("Play Game");
        Time.timeScale = 1f;
        SceneManager.LoadScene("GameScene");

    }


    public void OpenStoryMenu()
    {
        Debug.Log("Открытие меню сюжета");
        Time.timeScale = 1f;
        SceneManager.LoadScene("videoscene");
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

    public void LoadLevel4()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level4Scene");
    }

    public void LoadLevel5()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level5Scene");
    }

    public void LoadLevel6()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Level6Scene");
    }

    public void LoadBonusLevel7()
    {
        if (!AreFirstSixStoryLevelsCompleted())
            return;

        Time.timeScale = 1f;
        SceneManager.LoadScene("BonusLevel7Scene");
    }

    public void BackToMenu()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }

   
    public void OpenSettings()
    {
        Debug.Log("Открытие настроек");
        Time.timeScale = 1f;
        SceneManager.LoadScene("SettingsScene");
    }

    public void QuitGame()
    {
        Debug.Log("Quit Game");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }

    bool AreAllStoryLevelsCompleted()
    {
        return AreFirstSixStoryLevelsCompleted()
            && PlayerPrefs.GetInt("StoryLevelCompleted_BonusLevel7Scene", 0) == 1;
    }

    bool AreFirstSixStoryLevelsCompleted()
    {
        for (int i = 1; i <= StoryLevelCount; i++)
        {
            if (PlayerPrefs.GetInt($"StoryLevelCompleted_Level{i}Scene", 0) != 1)
                return false;
        }
        return true;
    }

    void EnsureExtendedStoryButtons()
    {
        Transform panel = FindButtonPanel();
        if (panel == null) return;

        CreateLevelButtonIfMissing(panel, "ButtonLVL4", "4 УРОВЕНЬ", new Vector2(-220f, -40f), LoadLevel4);
        CreateLevelButtonIfMissing(panel, "ButtonLVL5", "5 УРОВЕНЬ", new Vector2(0f, -40f), LoadLevel5);
        CreateLevelButtonIfMissing(panel, "ButtonLVL6", "6 УРОВЕНЬ", new Vector2(220f, -40f), LoadLevel6);
        CreateLevelButtonIfMissing(panel, "ButtonBONUS7", "БОНУС 7", new Vector2(0f, -140f), LoadBonusLevel7);
    }

    void UpdateBonusButtonState()
    {
        GameObject bonus = GameObject.Find("ButtonBONUS7");
        if (bonus == null) return;

        bool unlocked = AreFirstSixStoryLevelsCompleted();
        bonus.SetActive(unlocked);

        Button bonusButton = bonus.GetComponent<Button>();
        if (bonusButton != null)
            bonusButton.interactable = unlocked;
    }

    Transform FindButtonPanel()
    {
        GameObject panel = GameObject.Find("mainmenupanel");
        return panel != null ? panel.transform : null;
    }

    void CreateLevelButtonIfMissing(Transform parent, string objectName, string label, Vector2 anchoredPosition, UnityEngine.Events.UnityAction onClick)
    {
        if (parent.Find(objectName) != null) return;

        Transform template = parent.Find("ButtonLVL1");
        if (template == null) return;

        GameObject clone = Instantiate(template.gameObject, parent);
        clone.name = objectName;

        RectTransform rect = clone.GetComponent<RectTransform>();
        if (rect != null)
            rect.anchoredPosition = anchoredPosition;

        Text text = clone.GetComponentInChildren<Text>();
        if (text != null)
            text.text = label;

        Button button = clone.GetComponent<Button>();
        if (button != null)
        {
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(onClick);
        }
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