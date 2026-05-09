using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class ScenesManager : MonoBehaviour
{
    public string mainMenuSceneName = "MainMenu";
    public string gameSceneName = "GameScene";

    public GameObject fadePanel;

    private CanvasGroup fadeCanvasGroup;


    void Start()
    {
      //  StartCoroutine(FadeIn());

        fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
    }

    public void LoadGame()
    {
        SceneManager.LoadScene(gameSceneName);
    }

    public void LoadMenu()
    {
        SceneManager.LoadScene(mainMenuSceneName);
    }

    /*
    public IEnumerator FadeIn()
    {
        // Implement fade-in effect here

    }

    public IEnumerator FadeOut()
    {
        // Implement fade-out effect here

    }
    */
}
