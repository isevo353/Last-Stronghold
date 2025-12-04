using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatMenu : MonoBehaviour
{
    // Ссылка на панель меню поражения (перетащите в инспекторе)
    public GameObject defeatMenuPanel;
    
    void Start()
    {
        // Скрываем меню при старте
        if (defeatMenuPanel != null)
        {
            defeatMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("DefeatMenuPanel не назначен! Перетащите панель в инспекторе.");
        }
    }
    
    // ===== МЕТОДЫ ДЛЯ КНОПОК =====
    
    // Перезапуск уровня
    public void RestartLevel()
    {
        Debug.Log("Перезапуск уровня");
        Time.timeScale = 1f; // Возвращаем нормальную скорость игры
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // В главное меню
    public void GoToMainMenu()
    {
        Debug.Log("Возврат в главное меню");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
    
    // Выход из игры
    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    // Показываем меню поражения (публичный метод, можно вызвать из других скриптов)
    public void ShowDefeatMenu()
    {
        Debug.Log("ПОРАЖЕНИЕ!");
        
        // Останавливаем игру
        Time.timeScale = 0f;
        
        // Показываем меню
        if (defeatMenuPanel != null)
        {
            defeatMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("Не могу показать меню: defeatMenuPanel равен null!");
        }
    }
}