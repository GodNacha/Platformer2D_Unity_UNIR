using UnityEngine;

public class Coin : MonoBehaviour
{
    PlayerController player;

    private void Awake()
    {
        player = FindAnyObjectByType<PlayerController>();
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player.AddScoreCoin();
            Destroy(gameObject);
        }
    }
}


