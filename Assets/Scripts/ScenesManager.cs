using System.Collections;
using Unity.VectorGraphics;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScenesManager : MonoBehaviour
{
    [Header ("Fade Settings")]
    public GameObject fadePanel;
    public float delayFadeOut = 2f;
    public float fadeDuration = 2f;

    private CanvasGroup fadeCanvasGroup;

    private void Awake()
    {
        if (fadePanel == null)
        {
            fadeCanvasGroup = GameObject.Find("FadePanel").GetComponent<CanvasGroup>();
        }
        else
        {
            fadeCanvasGroup = fadePanel.GetComponent<CanvasGroup>();
        }

        if (SceneManager.GetActiveScene().name == "Menu")
        {
            Cursor.visible = true; //Hace visible el cursor en el menú principal
        }
        else
        {
            Cursor.visible = false; //Hace invisible el cursor en el resto de escenas
        }
    }

    void Start()
    {              

        fadeCanvasGroup.alpha = 0f; //Setear la opacidad/Alpha en 0 cada vez que se inicia el juego para evitar problemas

        fadeCanvasGroup.blocksRaycasts = true; //Bloquea los botones al comenzar

        StartCoroutine(FadeIn());


    }

    public void LoadScene(string sceneName)
    {
        StartCoroutine(FadeOut(sceneName));
    }

    public void QuitGame()
    {
        Application.Quit();
    }
   
    public IEnumerator FadeIn()
    {
        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(1f, 0f, elapsedTime / fadeDuration);
            elapsedTime += Time.deltaTime; 
            yield return null;
        }
        fadeCanvasGroup.alpha = 0f; 
        fadeCanvasGroup.blocksRaycasts = false; 

    }

    public IEnumerator FadeOut(string sceneName)
    {
        fadeCanvasGroup.blocksRaycasts = true; 

        yield return new WaitForSeconds(delayFadeOut); //Espera un tiempo antes de iniciar el fade out, para que el jugador pueda ver la animación.

        float elapsedTime = 0f;
        while (elapsedTime < fadeDuration)
        {
            fadeCanvasGroup.alpha = Mathf.Lerp(0f, 1f, elapsedTime / fadeDuration);
            elapsedTime += Time.unscaledDeltaTime; //UnsacledDeltaTime para que el fade out no se detenga con el timepo del juego pausado.
            yield return null;
        }
        fadeCanvasGroup.alpha = 1f; 
        SceneManager.LoadScene(sceneName);

    }
    
}
