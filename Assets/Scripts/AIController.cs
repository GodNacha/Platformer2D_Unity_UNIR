using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    CharacterController characterController2D;
    [SerializeField] Transform target;

    private bool dead = false;

    [Header("Enemy Stats")]
    public int lifes = 3;
    public Animator anim;

    [Header("Disable Scripts After Dead")]
    public MonoBehaviour[] scriptsToDisableOnDeath; //Aquí se pueden agregar los scripts que se quieran desactivar al morir, como el script de movimiento, ataque, etc, para evitar que el personaje siga moviéndose o atacando después de morir

    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
    }

    private void Update()
    {
        Vector2 rawMove = Vector2.zero;

        if (target)
        {
           if (transform.position.x > target.position.x)
           {
                rawMove = Vector2.left;
           }
           else
           {
                rawMove = Vector2.right;
           }

           if (Mathf.Abs(transform.position.x - target.position.x) < 0.1f) //Para que se quede quieto al llegar al target, y no se quede vibrando por estar cambiando constantemente de dirección
            {
                rawMove = Vector2.zero;
           }

        }
       
            characterController2D.SetRawMove(rawMove);
    }

    public void Damage()
    {
        if (!dead)
        {
            lifes--;
            

            if (lifes <= 0)
            {
                Dead();

                //Agregar efecto de sonido

            }
            else
            {
                anim.SetTrigger("Hit"); //Animación de recibir daño


                //Agregar efecto de sonido
            }
        }
    }

    public void Dead()
    {
        dead = true;

        foreach (var script in scriptsToDisableOnDeath) //Desactiva los scripts que se hayan agregado al array scriptsToDisableOnDeath, para evitar que el personaje siga moviéndose o atacando después de morir
        {
            script.enabled = false;
        }

        anim.SetTrigger("Dead"); //Animación de muerte      

        characterController2D.SetRawMove(Vector2.zero); //Detiene el movimiento horizontal del personaje al morir, para que no siga moviéndose después de la animación de muerte

        StartCoroutine(Destroy()); //Inicia la corrutina para destruir el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena

    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(4f);
        Destroy(gameObject); //Destruye el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            Dead();
        }
    }


}
