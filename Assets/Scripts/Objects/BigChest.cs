using TMPro;
using UnityEngine;

public class BigChest : MonoBehaviour
{
    [Header("Chest Settings")]
    [SerializeField] bool isOpen = false;
    [SerializeField] TextMeshProUGUI feedbackText; // Referencia al texto de feedback
    [SerializeField] Animator feedbackTextAnimator;


    private PlayerInventory inventory;
    private Animator animChest;
    public Animator animTextChest;
    private SplashCoins splashCoins;
    private GameManager gameManager;
    private Collider2D chestCollider;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip negativeSound;

    private void Awake()
    {
        inventory = FindAnyObjectByType<PlayerInventory>();
        animChest = GetComponent<Animator>();
        splashCoins = GetComponentInChildren<SplashCoins>();
        gameManager = FindAnyObjectByType<GameManager>();
        chestCollider = GetComponent<Collider2D>();
        audioSource = GetComponent<AudioSource>();

        if (feedbackText == null)
        {
            Debug.LogWarning("No se ha asignado el TextMeshProUGUI para el feedback.");
        }

        if (feedbackTextAnimator == null)
        {
            Debug.LogWarning("No se ha asignado el Animator para el feedback.");
        }

        chestCollider.enabled = true; // Asegura que el collider del cofre esté habilitado al inicio
    }

    public void TryOpen()
    {
        if (isOpen) return;

        if (inventory != null && inventory.hasKey)
        {
            OpenChest();
        }
        else
        {
            Debug.Log("Te falta la llave");
            feedbackText.text = "I need the key!";

            feedbackTextAnimator.SetTrigger("ShowText");

            audioSource.Stop();
            audioSource.clip = negativeSound;
            audioSource.Play();
        }
    }

    void OpenChest()
    {
        isOpen = true;
        chestCollider.enabled = false;

        OpenAnimation();
        gameManager.Win();

        Debug.Log("Cofre abierto");


    }

    void OpenAnimation()
    {
        animChest.SetTrigger("Open");
        animTextChest.SetTrigger("Transition");
        splashCoins.Splash();

    }
}
