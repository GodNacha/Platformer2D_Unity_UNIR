using UnityEngine;
using UnityEngine.Events;

public class HurtCollider : MonoBehaviour
{
    public UnityEvent OnHitReceived; //Evento que se dispara cuando el HitCollider entra en contacto con un HurtCollider

    internal void NotifyHit(HitCollider hitCollider)
    {
        OnHitReceived?.Invoke(); //Invoca el evento OnHitReceived, lo que permite que otros scripts puedan suscribirse a este evento para reaccionar cuando el HitCollider recibe un golpe.
    }
}

