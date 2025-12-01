using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuController : MonoBehaviour
{
    // Ссылка на панель меню паузы
    public GameObject pauseMenuPanel;
    
    // Переменная для отслеживания состояния паузы
    private bool isGamePaused = false;
    
    void Start()
    {
        // Убедимся, что игра не на паузе при старте
        Time.timeScale = 1f;
        
        // Скрываем панель при старте (на всякий случай)
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("Панель паузы не назначена! Перетащите её в инспекторе.");
        }
        
        Debug.Log("Меню паузы готово. Нажмите ESC для паузы.");
    }
    
    void Update()
    {
        // Проверяем нажатие ESC каждый кадр
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Debug.Log("Клавиша ESC нажата");
            
            if (isGamePaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }
    
    // Включить паузу
    public void PauseGame()
    {
        Debug.Log("Игра поставлена на паузу");
        
        isGamePaused = true;
        Time.timeScale = 0f; // Останавливаем время игры
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(true); // Показываем меню
        }
        
        // Можно также отключить звуки или другие системы
    }
    
    // Выключить паузу
    public void ResumeGame()
    {
        Debug.Log("Игра продолжена");
        
        isGamePaused = false;
        Time.timeScale = 1f; // Возвращаем нормальное время
        
        if (pauseMenuPanel != null)
        {
            pauseMenuPanel.SetActive(false); // Скрываем меню
        }
    }
    
    // Загрузить главное меню
    public void LoadMainMenu()
    {
        Debug.Log("Загрузка главного меню");
        
        Time.timeScale = 1f; // Важно: сбросить время перед загрузкой новой сцены
        SceneManager.LoadScene("MainMenuScene"); // Замените на имя вашей сцены с главным меню
    }
    
    // Перезапустить уровень
    public void RestartLevel()
    {
        Debug.Log("Перезапуск уровня");
        
        Time.timeScale = 1f;
        // Загружаем текущую сцену заново
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    // Выйти из игры
    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        
        // В редакторе Unity
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}