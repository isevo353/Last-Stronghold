//using System.Diagnostics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuController : MonoBehaviour
{
    // ����� ��� ������ "������"
    public void PlayGame()
    {
        Debug.Log("�������� ������� �����");
        SceneManager.LoadScene("GameScene"); // ��� ����� ������� �����
    }

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