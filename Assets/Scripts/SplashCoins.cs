using UnityEngine;

public class SplashCoins : MonoBehaviour
{
    [Header("Coin References")]
    [SerializeField] private GameObject coinPrefab;
    [SerializeField] private Transform spawnPoint;

    [Header("Coin Amount")]
    [SerializeField] private int coinAmount = 15;

    [Header("Force")]
    [SerializeField] private float minUpForce = 1.5f;
    [SerializeField] private float maxUpForce = 3f;
    [SerializeField] private float horizontalForce = 4f;   

    public void Splash()
    {
        for (int i = 0; i < coinAmount; i++)
        {
            GameObject coin = Instantiate(coinPrefab, spawnPoint.position, Quaternion.identity); //Instnacia la moneda en la posición del spawnPoint

            Rigidbody2D rb = coin.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                float randomUpForce = Random.Range(minUpForce, maxUpForce);

                Vector2 randomForce = new Vector2(Random.Range(-horizontalForce, horizontalForce), randomUpForce);
              
                rb.AddForce(randomForce, ForceMode2D.Impulse);
            }
        }
    }
}
