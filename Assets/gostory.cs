using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class IntroWait : MonoBehaviour
{
    public int waitTime = 9; 

    void Start()
    {
        StartCoroutine(WaitForLevel());
    }

    private IEnumerator WaitForLevel() 
    {
        yield return new WaitForSeconds(waitTime);
        SceneManager.LoadScene("StoryMenuScene");
    }
}