using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    CharacterController characterController2D;
    [SerializeField] Transform target;

    private bool dead = false;
    bool canAttack = true;

    private int attackDirection = 1;

    [Header("Enemy Stats")]
    public int lifes = 3;
    public Animator anim;
    public float impulseForce = 2.8f; //Fuerza del impulso al atacar

    [Header("Detection Range")]
    [SerializeField] float detectionRange = 4f; //Rango de detección del enemigo para detectar al jugador
    [SerializeField] float chestRange = 8f; 

    private bool chesing = false; //Variable para saber si el enemigo está persiguiendo al jugador

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
          float distanceToTarget = Vector2.Distance(transform.position, target.position);
    
            if (distanceToTarget <= detectionRange) //El enemigo empieza a perseguir al jugador si está dentro del rango de detección
            {
                 chesing = true; 
            }
            else
            {
                if (distanceToTarget >= chestRange) //El enemigo deja de perseguir al jugador si está fuera del rango de persecusión
                {
                    chesing = false;
                }
                     
            }
        }

        if (target && chesing)
        {
           if (transform.position.x > target.position.x)
           {
                rawMove = Vector2.left;
           }
           else
           {
                rawMove = Vector2.right;
           }

           if (Mathf.Abs(transform.position.x - target.position.x) < 0.5f) //Para que se quede quieto al llegar al target, y no se quede vibrando por estar cambiando constantemente de dirección
           {
                rawMove = Vector2.zero; //Se queda quieto
                Attack(); //Llama a la función Attack para que el enemigo ataque al jugador cuando esté lo suficientemente cerca, esto se puede ajustar dependiendo de la distancia que se quiera para que el enemigo ataque al jugador
           }

           
        }

        characterController2D.SetRawMove(rawMove);
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        // Gizmo de rango circular para detectar al jugador
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;

        // Gizmo de rango circular para mostrar el rango de persecusión
        Gizmos.DrawWireSphere(transform.position, chestRange);
    }

    public void AttackImpulse()
    {
        //Se impulsa hacia la derecha o izquierda dependiendo de la posición del target, para que el enemigo se impulse hacia el jugador al atacar, y no se quede quieto en su posición.
        if (target)
        {         
            characterController2D.rb.linearVelocityX = impulseForce * attackDirection; // Impulso hacia la derecha o izquierda dependiendo de la posición del target.

            Debug.Log("Impulso de ataque aplicado"); //Aquí es donde se aplicaría el impulso al enemigo al atacar, para que se impulse hacia el jugador, esto se puede ajustar dependiendo de la fuerza del impulso que se quiera para el enemigo al atacar

        }

    }

    public void Attack()
    {
        if (characterController2D.attacking == true) return; //Si el personaje ya esta atacando, no se puede iniciar otro ataque

        characterController2D.canMove = false;
        characterController2D.attacking = true;

        if (characterController2D.IsGrounded() && canAttack)
        {
            // Guardar la dirección del ataque dependiendo de la posición del target/jugador
            if (transform.position.x > target.position.x)
            {
                attackDirection = -1;
            }
            else
            {
                attackDirection = 1;
            }

            anim.SetTrigger("Attack"); //Animación de ataque

            StartCoroutine(AfterAttack());
        }

    }

    public IEnumerator AfterAttack()
    {
        anim.SetBool("IsRunning", false);
        yield return new WaitForSeconds(1.5f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        characterController2D.attacking = false;
        characterController2D.canMove = true;
    }

    public void CancelAttack()
    {
       // if (!characterController2D.attacking) return;

        //Completar

        
    }


    public void Damage()
    {
        if (!dead)
        {
            lifes--;

           // CancelAttack();

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
