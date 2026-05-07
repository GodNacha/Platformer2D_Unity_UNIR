using UnityEngine;
using UnityEngine.Events;

public class HitCollider : MonoBehaviour
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        HurtCollider hurt = collision.GetComponent<HurtCollider>(); //Esto es para detectar si el collider que ha entrado en contacto con el hurt collider tiene un componente HitCollider, lo cual indicaría que es un ataque o daño

        hurt?.NotifyHit(this); //Si el collider tiene un componente HitCollider, se llama a su método NotifyHit, pasando una referencia al HurtCollider que ha sido golpeado.
    }

}
