using UnityEngine;
using UnityEngine.Events;


public class OnTrigger : MonoBehaviour
{
    [Header("Name Tag Object on Trigger")]
    public string targetTag;

    [Header("Trigger Settings")]
    [SerializeField] private bool triggerOnce = false; 

    [Header("Event After OnTriggered")]
    public UnityEvent onTriggerEnter;

    private Collider2D triggerCollider;

    private void Start()
    {
        triggerCollider = GetComponent<Collider2D>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            if (triggerOnce) //Se apaga el collider después de la primera vez que se activa, para evitar que se vuelva a activar si el jugador vuelve a entrar en el trigger.
            {
                triggerCollider.enabled = false;
            }

            TriggerAction();
        }
    }

    internal void TriggerAction()
    {
        onTriggerEnter?.Invoke(); 
    }
}
