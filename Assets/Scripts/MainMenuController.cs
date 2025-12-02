using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // Метод для кнопки "Играть"
    public void PlayGame()
    {
        Debug.Log("Загрузка игровой сцены");
        SceneManager.LoadScene("GameScene"); // Имя вашей игровой сцены
    }
    
    // Метод для кнопки "Настройки"
    public void OpenSettings()
    {
        Debug.Log("Открытие настроек");
        // Здесь можно сделать панель настроек
    }
    
    // Метод для кнопки "Выход"
    public void QuitGame()
    {
        Debug.Log("Выход из игры");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
}