using UnityEngine;
using System.Collections;

public class Coin : MonoBehaviour
{
    PlayerController player;

    [Header("Coin Settings")]
    public bool isSpecialCoin = false; //Esto es para diferenciar entre las monedas normales y las monedas especiales.

    [Header("Special Coin")]
    [Tooltip("Duración de la invulnerabilidad de las monedas que Spawnean en un cofre/objeto. Asi se evita que el jugador las agarre apenas salen, permitiendo que se vea la animación")]
    public float invulnerabilityDuration = 0.3f; //Duración de la invulnerabilidad para las monedas especiales, se puede ajustar a lo que se quiera

    private bool isInvulnerable = false; 

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();

        if (isSpecialCoin)
        {
            isInvulnerable = true; 

            StartCoroutine(Invulneravility());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isInvulnerable)
        {
            player.AddScoreCoin();
            Destroy(gameObject);
        }

        if (collision.CompareTag("DeadZone"))
        {
            Destroy(gameObject);
        }
    }

    IEnumerator Invulneravility()
    {
        yield return new WaitForSeconds(invulnerabilityDuration); //Duración de la invulnerabilidad, se puede ajustar a lo que se quiera
        isInvulnerable = false;
    }
}


