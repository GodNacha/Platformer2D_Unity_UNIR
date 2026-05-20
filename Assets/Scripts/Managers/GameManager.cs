using TMPro;
using UnityEngine;


public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverUI;
    public GameObject winUI;
    public TextMeshProUGUI textScore1;
    public TextMeshProUGUI textScore2;
    public GameObject StatsPanel;

    [Header("References")]
    PlayerController player;

    [SerializeField] public bool endGame = false;

    [Header("Audio")]
    private AudioSource audioSource;
    private AudioSource musicAudioSource;
    public AudioClip gameOverClip;
    public AudioClip winClip;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();

        audioSource = GetComponent<AudioSource>();

        musicAudioSource = GameObject.Find("MusicAudio")?.GetComponent<AudioSource>();

        gameOverUI.SetActive(false);
        winUI.SetActive(false);
        StatsPanel.SetActive(true);

    }

    public void GameOver()
    {
        textScore1.text = "X " + player.coins; // Muestra la puntuación final del jugador en el UI de Game Over

        endGame = true;
        gameOverUI.SetActive(true);

        player.enabled = false; // Desactiva el script del jugador para evitar que siga moviéndose o atacando      

        Cursor.visible = true;

        audioSource.PlayOneShot(gameOverClip);

        if (musicAudioSource != null)
        {
            musicAudioSource.Stop(); // Detiene la música de fondo al perder
        }

        StatsPanel.SetActive(false); 
    }

    public void Win()
    {
        player.OnDisable(); 
        player.NoMovement(); // Evita que el jugador se mueva después de ganar, para que no siga moviéndose después de la animación de victoria

        textScore2.text = "X " + player.coins;

        endGame = true;
        winUI.SetActive(true);

        Cursor.visible = true;

        audioSource.PlayOneShot(winClip);

        if (musicAudioSource != null)
        {
            musicAudioSource.Stop(); // Detiene la música de fondo al perder
        }

        StatsPanel.SetActive(false); 


    }
}

