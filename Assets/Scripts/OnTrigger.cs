using UnityEngine;
using UnityEngine.Events;


public class OnTrigger : MonoBehaviour
{
    [Header("Name Tag Object on Trigger")]
    public string targetTag;

    [Header("Event After OnTriggered")]
    public UnityEvent onTriggerEnter;

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag(targetTag))
        {
            TriggerAction();
        }
    }

    internal void TriggerAction()
    {
        onTriggerEnter?.Invoke(); 
    }
}
