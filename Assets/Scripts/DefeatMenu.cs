using UnityEngine;
using UnityEngine.SceneManagement;

public class DefeatMenu : MonoBehaviour
{

    public GameObject defeatMenuPanel;
    
    void Start()
    {
       
        if (defeatMenuPanel != null)
        {
            defeatMenuPanel.SetActive(false);
        }
        else
        {
            Debug.LogError("DefeatMenuPanel  null!");
        }
    }
    
    
    
   
    public void RestartLevel()
    {
        Debug.Log("RestartLevel");
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
    
    
    public void GoToMainMenu()
    {
        Debug.Log("GoToMainMenu");
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenuScene");
    }
    
   
    public void QuitGame()
    {
        Debug.Log("QuitGame");
        
        #if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
        #else
        Application.Quit();
        #endif
    }
    
    
    public void ShowDefeatMenu()
    {
        Debug.Log("ShowDefeatMenu");
        
     
        Time.timeScale = 0f;
        
       
        if (defeatMenuPanel != null)
        {
            defeatMenuPanel.SetActive(true);
        }
        else
        {
            Debug.LogError("defeatMenuPanel  null!");
        }
    }
}