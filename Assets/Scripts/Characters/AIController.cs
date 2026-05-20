using System.Collections;
using UnityEngine;

public class AIController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController characterController2D;
    [SerializeField] SplashCoins splashCoins;
    [SerializeField] WaypointPatrol waypointPatrol;

    GameManager gameManager;


    [Header("Target a Perseguir")]
    [SerializeField] Transform target;

    [Header("Enemy Stats")]
    public int lifes = 3;
    public Animator anim;
    public float impulseForce = 2.8f; //Fuerza del impulso al atacar
    public float delayAttack = 0.3f;

    [Header("Detection Range")]
    [SerializeField] float detectionRange = 4f; //Rango de detección del enemigo para detectar al jugador
    [SerializeField] float chestRange = 8f;

    private bool chesing = false; //Variable para saber si el enemigo está persiguiendo al jugador

    private bool dead = false;
    bool dieByZone = false; //Variable para saber si el enemigo murió por caer en la DeadZone, esto es para aplicar un efecto de salto al morir en la DeadZone
    bool inmune = false;
    private bool  canceledAttack = false;

    private int attackDirection = 1;

    private Coroutine attackCorutine;

    private bool attackStarted = false;


    Collider2D enemyCollider;
    Collider2D playerCollider;

    [Header("Audio")]
    public AudioSource audioMushroom;
    public AudioClip hitClip;
    public AudioClip deadClip;
    public AudioClip preAttackClip;
    public AudioClip impulseClip;


    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();
        splashCoins = GetComponentInChildren<SplashCoins>();
        gameManager = FindAnyObjectByType<GameManager>();
        enemyCollider = GetComponent<Collider2D>();
        playerCollider = FindAnyObjectByType<PlayerController>().GetComponent<Collider2D>();

        anim = GetComponent<Animator>();
        waypointPatrol = GetComponent<WaypointPatrol>();

        audioMushroom = GetComponent<AudioSource>();
    }

    private void Update()
    {
        Vector2 rawMove = Vector2.zero;

        if (target && !gameManager.endGame)
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

        //Patrullando los waypoints

        if (!chesing && !dead && !gameManager.endGame)
        {
            waypointPatrol.UpdatePatrol();

            // Si está esperando, no se mueve
            if (waypointPatrol.IsWaiting)
            {
                rawMove = Vector2.zero;
            }
            else
            {
                Transform patrolTarget = waypointPatrol.CurrentTarget; //Nuevo target para patrullar

                if (patrolTarget != null)
                {
                    if (transform.position.x > patrolTarget.position.x)
                    {
                        rawMove = Vector2.left;
                    }
                    else
                    {
                        rawMove = Vector2.right;
                    }
                }
            }
        }


        //Persiguiendo al jugador

        if (target && chesing && !dead && !gameManager.endGame && !attackStarted)
        {
            waypointPatrol.StopWaiting(); //Se corta la espera del waypoint para empezar a seguir al jugador.

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

                if (!characterController2D.attacking && !dead && !attackStarted)
                {
                    attackStarted = true;
                    attackCorutine = StartCoroutine(Attack()); //Llama a la función Attack para que el enemigo ataque al jugador cuando esté lo suficientemente cerca.

                }
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
        if (target && inmune)
        {         
            characterController2D.rb.linearVelocityX = impulseForce * attackDirection; // Impulso hacia la derecha o izquierda dependiendo de la posición del target.

            characterController2D.OnAttackAnimation();

            audioMushroom.PlayOneShot(impulseClip); //Reproduce el sonido de impulso al atacar

            Debug.Log("Impulso de ataque aplicado"); //Aquí es donde se aplicaría el impulso al enemigo al atacar, para que se impulse hacia el jugador, esto se puede ajustar dependiendo de la fuerza del impulso que se quiera para el enemigo al atacar

        }

    }

    public IEnumerator Attack()
    {
        canceledAttack = false;

        characterController2D.canMove = false;
        characterController2D.attacking = true;

        audioMushroom.Stop();
        audioMushroom.clip = preAttackClip;
        audioMushroom.Play();

        if (characterController2D.IsGrounded())
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

            yield return new WaitForSeconds(delayAttack); //Tiempo de espera para aplicar el impulso.

            if (canceledAttack || dead) //Se rompe la corrutina si el ataque se cancela
            {
                yield break;
            }

            inmune = true;

            AttackImpulse(); //Aplica el impulso al atacar        

        }
    }

    public IEnumerator AfterAttack()
    {
        anim.SetBool("IsRunning", false);
        yield return new WaitForSeconds(0.3f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.       
        inmune = false;

        yield return new WaitForSeconds(1.3f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        characterController2D.attacking = false;
        characterController2D.canMove = true;
        attackStarted = false;
    }

    public void CancelAttack()
    {            
        if (attackCorutine != null)
        {
            canceledAttack = true;
            StopCoroutine(attackCorutine);

            attackCorutine = null;

            StartCoroutine(AfterCanceled());
            Debug.Log("Ataque cancelado");
        }
     
    }

    public IEnumerator AfterCanceled()
    {
        characterController2D.canMove = false;
        anim.SetBool("IsRunning", false);
        yield return new WaitForSeconds(0.3f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        canceledAttack = false;
        inmune = false;
        yield return new WaitForSeconds(1f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        characterController2D.attacking = false;
        characterController2D.canMove = true;
        attackStarted = false;
    }


    public void Damage()
    {
        if (!dead && !inmune)
        {
            lifes--;

            if (characterController2D.attacking && !inmune)
            {
                audioMushroom.Stop();
                CancelAttack();
            }

            inmune = true;

            if (lifes <= 0)
            {
                Dead();

                audioMushroom.PlayOneShot(deadClip); //Reproduce el sonido de muerte

            }
            else
            {
                anim.SetTrigger("Hit"); //Animación de recibir daño
                StartCoroutine(AfterAttack());
                audioMushroom.PlayOneShot(hitClip);
            }
        }
    }

    public void Dead()
    {
        dead = true;      

        anim.SetTrigger("Dead"); //Animación de muerte      
        Physics2D.IgnoreCollision(enemyCollider, playerCollider, true); //Esto hace que el se ignoren las colisiones entre el enemigo y el jugador tras morir.

        characterController2D.SetRawMove(Vector2.zero); //Detiene el movimiento horizontal del personaje al morir, para que no siga moviéndose después de la animación de muerte

        StartCoroutine(Destroy()); //Inicia la corrutina para destruir el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena


        if (dieByZone)
        {
            characterController2D.rb.linearVelocityY = 3.5f; //Esto hace que el personaje salte un poco al caer en la DeadZone
        }
        else
        {
            splashCoins.Splash(); //Llama a la función Splash de SplashCoins para que el enemigo suelte monedas al morir.
        }

    }   

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject); //Destruye el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone") && !dieByZone)
        {
            dieByZone = true;
            Dead();
            
        }
    }


}
