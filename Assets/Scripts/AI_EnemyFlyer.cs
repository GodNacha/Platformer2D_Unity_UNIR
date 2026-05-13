using UnityEngine;
using System.Collections;

public class AI_EnemyFlyer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController characterController2D;
    [SerializeField] SplashCoins splashCoins;

    GameManager gameManager;

    [Header("Target a Perseguir")]
    [SerializeField] Transform target;

    [Header("Enemy Stats")]
    public int lifes = 3;
    public Animator anim;
    public float attackDownSpeed = 2.8f;

    [Header("Detection Range")]
    [SerializeField] float detectionRange = 4f; //Rango de detección del enemigo para detectar al jugador
    [SerializeField] float chestRange = 8f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundCheckDistance = 0.2f;

    private bool chesing = false; //Variable para saber si el enemigo está persiguiendo al jugador

    private bool dead = false;
    bool impulseActivate = false;
    bool inmune = false;

    private float targetAttackY;
    public Transform jugadorReal;


    Collider2D enemyCollider;
    Collider2D playerCollider;

    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();
        splashCoins = GetComponentInChildren<SplashCoins>();
        gameManager = FindAnyObjectByType<GameManager>();
        enemyCollider = GetComponent<Collider2D>();
        playerCollider = FindAnyObjectByType<PlayerController>().GetComponent<Collider2D>();

        anim = GetComponent<Animator>();
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

        if (target && chesing && !dead && !gameManager.endGame && !impulseActivate)
        {
   
            Vector2 direction = (target.position - transform.position).normalized; //Calcula la dirección normalizada (X-Y)
            rawMove = direction;

            if (Vector2.Distance(transform.position, target.position) < 0.3f)
            {
                rawMove = Vector2.zero; //Se queda quieto
                StartCoroutine(AttackCoroutine()); //Llama a la Corrutina Attack para que el enemigo ataque al jugador cuando esté lo suficientemente cerca, esto se puede ajustar dependiendo de la distancia que se quiera para que el enemigo ataque al jugador
            }

        }

        if (characterController2D.attacking && impulseActivate)
        {
            characterController2D.rb.linearVelocity = new Vector2(0, -attackDownSpeed);
        }

        characterController2D.SetRawMove(rawMove);
    }

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayerMask); //Esto es para detectar si el enemigo tocó el suelo.        

        bool grounded = hit.collider != null;
      
        // Si toca el suelo mientras ataca
        if (grounded && characterController2D.attacking && impulseActivate && transform.position.y <= targetAttackY + 0.2f)
        {
            StopAttackImpulse();
        }

        return grounded;
    }
   
    public IEnumerator AttackCoroutine()
    {
        if (characterController2D.attacking == true) //Si el personaje ya esta atacando, no se puede iniciar otro ataque
        {
            yield break; //Salir de la corrutina si ya está atacando
        }      

        characterController2D.canMove = false;
        characterController2D.attacking = true;

        characterController2D.SetRawMove(Vector2.zero); //Detiene el movimiento horizontal del enemigo al iniciar el ataque, para que no siga moviéndose mientras ataca

        anim.SetTrigger("Pre-Attack"); //Animación de pre-Ataque
      
        yield return new WaitForSeconds(0.45f); //Esperar a que termine la animación.

        inmune = true;

        AttackImpulseDown();

    }

    public void AttackImpulseDown() //Llamar en un UnityEvent? O en una corrutina?
    {
        //El enemigo se impulsa hacia abajo de forma constante hasta tocar el suelo con la layer Ground.

        if (target && inmune)
        {
            targetAttackY = jugadorReal.position.y;

            impulseActivate = true;

            anim.SetTrigger("Attack"); //Animación de pre-Ataque

            characterController2D.OnAttackAnimation(); //Se activa el Collider Hit del enemigo.

        }

    }

    public void StopAttackImpulse() //Llamar en un UnityEvent? O en una corrutina?
    {
        impulseActivate = false; //Detiene el impulso hacia abajo del ataque, para que el enemigo se quede quieto después de tocar el suelo, y haga la animación de ataque final.

        anim.SetTrigger("Hit Floor"); //Animación de golpe al suelo.

        characterController2D.rb.linearVelocityY = 0; //Detiene el impulso hacia abajo
 
        StartCoroutine(AfterAttack());

        Debug.Log("Impulso de ataque detenido");
    }

    public IEnumerator AfterAttack()
    { 
        yield return new WaitForSeconds(0.2f);
        inmune = false;
        yield return new WaitForSeconds(1.3f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        anim.SetBool("IsRunning", false);
        characterController2D.attacking = false;
        characterController2D.canMove = true;
    }

    public void CancelAttack()
    {
        characterController2D.attacking = true;
        characterController2D.canMove = false;

        StopCoroutine(AttackCoroutine());
        StartCoroutine(AfterAttack());

        Debug.Log("Ataque cancelado");
    }


    public void Damage()
    {
        if (!dead && !inmune)
        {
            lifes--;

            if (characterController2D.attacking && !inmune)
            {
                CancelAttack();
            }

            inmune = true;

            if (lifes <= 0)
            {
                
                Dead();

                //Agregar efecto de sonido

            }
            else
            {

                anim.SetTrigger("Hit"); //Animación de recibir daño
                StartCoroutine(AfterAttack());
                //Agregar efecto de sonido
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
                                   //
        splashCoins.Splash(); //Llama a la función Splash de SplashCoins para que el enemigo suelte monedas al morir.
        

    }


    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(3.5f);
        Destroy(gameObject); //Destruye el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            StartCoroutine(AfterAttack());

        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = IsGrounded() ? Color.green : Color.red;

        // Gizmo de de linea para visualizar el raycast de detección de suelo
        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * groundCheckDistance));

        Gizmos.color = Color.red;

        // Gizmo de rango circular para detectar al jugador
        Gizmos.DrawWireSphere(transform.position, detectionRange);

        Gizmos.color = Color.blue;

        // Gizmo de rango circular para mostrar el rango de persecusión
        Gizmos.DrawWireSphere(transform.position, chestRange);

    }



}
