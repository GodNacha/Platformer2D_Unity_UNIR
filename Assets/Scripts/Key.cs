using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class Key : MonoBehaviour
{
    [Header("References")]
    [SerializeField] TextMeshProUGUI feedbackText; // Referencia al texto de feedback
    [SerializeField] Animator animFeedbackText; // Referencia al texto de feedback

    private PlayerInventory inventory;

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip keyPickupSound;

    void Start()
    {
        animFeedbackText = GameObject.Find("CanvasHeadPlayer").GetComponent<Animator>();
        feedbackText = GameObject.Find("FeedbackText").GetComponent<TextMeshProUGUI>();

        inventory = FindAnyObjectByType<PlayerInventory>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            if (inventory != null)
            {
                audioSource.PlayOneShot(keyPickupSound);
                inventory.hasKey = true;

                feedbackText.text = "I got the key!";
                animFeedbackText.SetTrigger("ShowText");

                Debug.Log("Llave recogida");

                Destroy(gameObject);
            }
        }
    }


}
