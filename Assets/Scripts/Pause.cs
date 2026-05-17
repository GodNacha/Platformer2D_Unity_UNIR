using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.InputSystem;

public class Pause : MonoBehaviour
{
    [Header("Inputs")]  
    [SerializeField] InputActionReference pauseKey;

    [Header("UI Reference")]
    [SerializeField] GameObject pausePanel;

    private bool isPaused = false;
    private GameManager gameManager;
    private PlayerController playerController;
    private ScenesManager scenesManager;

    [Header("Audio Refrences")]
    public AudioClip pauseClip;

    private AudioSource audioSource;


    private void Awake()
    {
        pauseKey.action.performed += OnPause;
        playerController = FindAnyObjectByType<PlayerController>();
        gameManager = FindAnyObjectByType<GameManager>();
        scenesManager = FindAnyObjectByType<ScenesManager>();
        audioSource = GetComponent<AudioSource>();

        if (pausePanel == null)
        {
            pausePanel = GameObject.Find("PausePanel"); // Asegura que se asigna el panel de pausa correctamente
        }
     
        pausePanel.SetActive(false); // Asegura que el panel de pausa esté desactivado al iniciar el juego
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
        playerController.OnEnable(); //Se activan los controles
        audioSource.PlayOneShot(pauseClip); // Reproduce el sonido de pausa al pausar el juego
    }


    public void PauseGame()
    {
        if (gameManager.endGame || scenesManager.isTransitioning) return; // Evita pausar el juego si ya ha terminado


        isPaused = true;
        Time.timeScale = 0; //Pausa el juego.
        pausePanel.SetActive(true); //Activa el panel de pausa al pausar el juego
        Cursor.visible = true;
        playerController.OnDisable(); //Se desactivan los controles

        audioSource.PlayOneShot(pauseClip); // Reproduce el sonido de pausa al pausar el juego

    }



}
