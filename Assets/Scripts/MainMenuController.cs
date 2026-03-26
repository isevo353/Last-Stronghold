//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // ����� ��� ������ "������" / "����������� �����"
    public void PlayGame()
    {
        Debug.Log("�������� ������� �����");
        SceneManager.LoadScene("GameScene"); // ��� ����� ������� �����
    }

    // ����� ��� ������ "������"
    public void OpenStoryMenu()
    {
        Debug.Log("Открытие меню сюжета");
        SceneManager.LoadScene("StoryMenuScene");
    }

    // Пока что все уровни ведут на одну и ту же сцену
    public void LoadLevel1() => SceneManager.LoadScene("GameScene");
    public void LoadLevel2() => SceneManager.LoadScene("GameScene");
    public void LoadLevel3() => SceneManager.LoadScene("GameScene");

    // ����� ��� ������ "���������"
    public void OpenSettings()
    {
        Debug.Log("Открытие настроек");
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
}