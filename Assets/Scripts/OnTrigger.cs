using UnityEngine;
using UnityEngine.Events;


public class OnTrigger : MonoBehaviour
{
    [Header("Name Tag Object on Trigger")]
    public string targetTag;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = false;
    [SerializeField] private bool useOnTriggerStay = false; //Si se activa, el evento se disparará cada frame que el jugador esté dentro del trigger, en lugar de solo una vez al entrar.

    [Header("Event After OnTriggered")]
    public UnityEvent onTriggerAction;

    private Collider2D triggerCollider;

    private void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag) && !useOnTriggerStay)
        {
            if (triggerOnce) //Se apaga el collider después de la primera vez que se activa, para evitar que se vuelva a activar si el jugador vuelve a entrar en el trigger.
            {
                triggerCollider.enabled = false;
            }

            TriggerAction();
        }
    }

    public void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag) && useOnTriggerStay)
        {
            TriggerAction();
            Debug.Log("Trigger Stay Activated"); // Debug para verificar que el evento se dispara cada frame mientras el jugador esté dentro del trigger
        }
    }

    internal void TriggerAction()
    {
        onTriggerAction?.Invoke(); 
    }
}
