using TMPro;
using UnityEngine;
using static UnityEditor.PlayerSettings.SplashScreen;

public class GameManager : MonoBehaviour
{
    [Header("UI")]
    public GameObject gameOverUI;
    public GameObject winUI;
    public TextMeshProUGUI textScore1;
    public TextMeshProUGUI textScore2;

    [Header("References")]
    PlayerController player;

    [SerializeField] public bool endGame = false;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();

        gameOverUI.SetActive(false);
        winUI.SetActive(false);
    }

    public void GameOver()
    {
        textScore1.text = "X " + player.coins; // Muestra la puntuación final del jugador en el UI de Game Over

        endGame = true;
        gameOverUI.SetActive(true);

        player.enabled = false; // Desactiva el script del jugador para evitar que siga moviéndose o atacando      

    }

    public void Win()
    {
        player.OnDisable(); 
        player.NoMovement(); // Evita que el jugador se mueva después de ganar, para que no siga moviéndose después de la animación de victoria

        textScore2.text = "X " + player.coins;

        endGame = true;
        winUI.SetActive(true);

    }
}

