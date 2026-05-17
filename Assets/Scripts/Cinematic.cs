using JetBrains.Annotations;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class Cinematic : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputActionReference pauseKey;

    [Header("UI Reference")]
    [SerializeField] GameObject pausePanel;

    [Header("Timeline Reference")]
    [SerializeField] PlayableDirector cinematicDirector;

    [Header("Animators")]
    public Animator[] animators;

    private bool canPause = false;
    private bool isPaused = false;
    private ScenesManager scenesManager;

    private AudioSource audioSource;

    [Header("Audio")]
    public AudioClip pauseClip;

    private void Awake()
    {
        pauseKey.action.performed += OnPause;
        scenesManager = FindAnyObjectByType<ScenesManager>();
        cinematicDirector = GetComponent<PlayableDirector>();
        audioSource = GetComponent<AudioSource>();

        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel"); // Asegura que se asigna el panel de pausa correctamente
        }

        pausePanel.SetActive(false); // Asegura que el panel de pausa esté desactivado al iniciar el juego

        Time.timeScale = 1;
    }

    void OnPause(InputAction.CallbackContext context)
    {
        if (isPaused == true)
        {
            Resume();
        }
        else
        {
            PauseGame();
        }
    }

    public void Resume() //Opcion para UI
    {
        isPaused = false;        
        Time.timeScale = 1; //Reanuda el juego.
        pausePanel.SetActive(false); //Desactiva el panel de pausa al reanudar el juego
        Cursor.visible = false;
        cinematicDirector.Play(); // Reanuda la cinemática

        foreach (Animator anim in animators)
            anim.enabled = true;

        audioSource.PlayOneShot(pauseClip); // Reproduce el sonido de pausa al pausar el juego

    }

    public void PauseGame()
    {      
        if (scenesManager.isTransitioning || !canPause) return; // Evita pausar si hay una transición en curso.
        isPaused = true;
        Time.timeScale = 0; //Pausa el juego (Físicas y tal)
        pausePanel.SetActive(true); //Activa el panel de pausa al pausar el juego
        Cursor.visible = true;
        cinematicDirector.Pause(); // Pausa la cinemática

        foreach (Animator anim in animators)
            anim.enabled = false;

        audioSource.PlayOneShot(pauseClip); // Reproduce el sonido de pausa al pausar el juego
    }

    public void NoPauses()
    {
        canPause = false;
    }

    public void YesPause()
    {
        canPause = true;
    }
}
