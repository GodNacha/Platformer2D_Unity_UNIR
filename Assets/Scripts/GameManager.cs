using UnityEngine;
using TMPro;

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

        textScore2.text = "X " + player.coins;

        endGame = true;
        winUI.SetActive(true);

    }
}

