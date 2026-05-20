using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class AI_EnemyFlyer : MonoBehaviour
{
    [Header("References")]
    [SerializeField] CharacterController characterController2D;
    [SerializeField] SplashCoins splashCoins;
    [SerializeField] WaypointPatrol waypointPatrol;

    GameManager gameManager;

    [Header("Target a Perseguir")]
    [SerializeField] Transform target;

    [Header("Enemy Stats")]
    public int startLifes = 1;
    public Animator anim;
    public float attackDownSpeed = 2.8f;

    [Header("Detection Range")]
    [SerializeField] float detectionRange = 4f; //Rango de detección del enemigo para detectar al jugador
    [SerializeField] float chestRange = 8f;
    [SerializeField] LayerMask groundLayerMask;
    [SerializeField] float groundCheckDistance = 0.2f;

    [Header("UI References")]
    [SerializeField] Image healthBarFill;

    [Header("Audio")]
    public AudioSource audioFlyer;
    public AudioClip hitClip;
    public AudioClip deadClip;
    public AudioClip preAttackClip;
    public AudioClip attackingClip;
    public AudioClip finalAttackClip;

    private bool chesing = false; //Variable para saber si el enemigo está persiguiendo al jugador

    private bool dead = false;
    bool impulseActivate = false;
    bool impulsing = false;
    bool inmune = false;
    bool canceledAttack = false;
    private Coroutine attackCoroutine;
    bool attackStarted = false;
    float currentLifes;

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

        waypointPatrol = GetComponent<WaypointPatrol>();
        anim = GetComponent<Animator>();
        audioFlyer = GetComponent<AudioSource>();

        currentLifes = startLifes;


    }

    private void Update()
    {
        Vector2 rawMove = Vector2.zero;

        IsGrounded(); //Llamar a la función IsGrounded para verificar si el enemigo está tocando el suelo

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


        //Patrullaje
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
                
                Transform patrolTarget = waypointPatrol.CurrentTarget;

                if (patrolTarget != null)
                {
                    Vector2 direction = (patrolTarget.position - transform.position).normalized;

                    rawMove = direction;

                     // Al llegar se detiene
                    if (Vector2.Distance(transform.position, patrolTarget.position) < 0.2f)
                    {
                        rawMove = Vector2.zero;
                    }
                }
                
            }
        }


        //Persecusión
        if (target && chesing && !dead && !gameManager.endGame && !impulseActivate && !attackStarted)
        {
            waypointPatrol.StopWaiting();
   
            Vector2 direction = (target.position - transform.position).normalized; //Calcula la dirección normalizada (X-Y)
            rawMove = direction;

            if (Vector2.Distance(transform.position, target.position) < 0.15f)
            {
                rawMove = Vector2.zero; //Se queda quieto

                if (!characterController2D.attacking && !dead && !attackStarted)
                {
                    attackStarted = true;   
                    characterController2D.rb.linearVelocity = Vector2.zero; //Detiene cualquier movimiento residual antes de atacar
                    characterController2D.SetRawMove(Vector2.zero);
                    characterController2D.canMove = false;

                    attackCoroutine = StartCoroutine(AttackCoroutine()); //Llama a la Corrutina Attack para que el enemigo ataque al jugador cuando esté lo suficientemente cerca
                    

                }
            }

        }

        if (characterController2D.attacking && impulseActivate && !impulsing)
        {
            impulsing = true;
            characterController2D.rb.linearVelocity = new Vector2(0, -attackDownSpeed);

        }

        characterController2D.SetRawMove(rawMove);
    }

    public bool IsGrounded()
    {
      //  RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayerMask); //Esto es para detectar si el enemigo tocó el suelo.
                                                                                                                      
        Collider2D hit = Physics2D.OverlapCircle(transform.position, groundCheckDistance, groundLayerMask); //Se utiliza un OverlapCircle en lugar de un Raycast para detectar el suelo, ya que el otro generaba problemas de detección.

        bool grounded = hit != null;
      
        // Si toca el suelo mientras ataca
        if (grounded && characterController2D.attacking && impulseActivate)
        {
            StopAttackImpulse();
        }

        return grounded;

        
    }
   
    public IEnumerator AttackCoroutine()
    {
        canceledAttack = false;     

       
        characterController2D.attacking = true;

        anim.SetTrigger("Pre-Attack"); //Animación de pre-Ataque

        audioFlyer.clip = preAttackClip;
        audioFlyer.Play();

        yield return new WaitForSeconds(0.45f); //Esperar a que termine la animación.

        if (canceledAttack || dead)
        {
            yield break;
        }
        else
        {
            inmune = true;

            AttackImpulseDown();
        }          

    }

    public void AttackImpulseDown() //Llamar en un UnityEvent? O en una corrutina?
    {
        //El enemigo se impulsa hacia abajo de forma constante hasta tocar el suelo con la layer Ground.

        if (target && inmune)
        {

            impulseActivate = true;

            anim.SetTrigger("Attack"); //Animación de pre-Ataque

            characterController2D.OnAttackAnimation(); //Se activa el Collider Hit del enemigo.

            audioFlyer.clip = attackingClip;
            audioFlyer.loop = true;
            audioFlyer.Play();

        }

    }

    public void StopAttackImpulse() //Llamar en un UnityEvent? O en una corrutina?
    {
        if (!impulseActivate)
            return;

        impulseActivate = false; //Detiene el impulso hacia abajo del ataque, para que el enemigo se quede quieto después de tocar el suelo, y haga la animación de ataque final.

        anim.SetTrigger("Hit Floor"); //Animación de golpe al suelo.

        characterController2D.rb.linearVelocityY = 0; //Detiene el impulso hacia abajo

        audioFlyer.Stop();
        audioFlyer.loop = false;
        audioFlyer.clip = finalAttackClip;
        audioFlyer.Play();

        StartCoroutine(AfterAttack());

        Debug.Log("Impulso de ataque detenido");
    }

    public IEnumerator AfterAttack()
    { 
        yield return new WaitForSeconds(0.3f);
        inmune = false;
        canceledAttack = false;
        yield return new WaitForSeconds(1.3f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        anim.SetBool("IsRunning", false);
        characterController2D.attacking = false;
        characterController2D.canMove = true;
        attackStarted = false;
        impulsing = false;
    }

    public void CancelAttack()
    {       
        StopCoroutine(attackCoroutine);
        attackCoroutine = null;

        StartCoroutine(AfterCanceled());

        Debug.Log("Ataque cancelado");
    }

    public IEnumerator AfterCanceled()
    {
        characterController2D.canMove = false;
        anim.SetBool("IsRunning", false);
        yield return new WaitForSeconds(0.3f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        canceledAttack = false;
        inmune = false;
        yield return new WaitForSeconds(0.7f); //Tiempo de espera después de atacar para recuperar movimiento y ataque.
        characterController2D.attacking = false;
        characterController2D.canMove = true;
        attackStarted = false;
        impulsing = false;
    }


    public void Damage()
    {
        if (!dead && !inmune)
        {
            currentLifes -= 0.35f; //Resta 0.35 vidas por cada ataque

            healthBarFill.fillAmount = currentLifes / startLifes; //Actualiza la barra de vida

            if (characterController2D.attacking && !inmune)
            {
                audioFlyer.Stop();
                CancelAttack();
            }

            inmune = true;

            if (currentLifes <= 0)
            {
                
                Dead();

                audioFlyer.PlayOneShot(deadClip);

            }
            else
            {

                anim.SetTrigger("Hit"); //Animación de recibir daño
                StartCoroutine(AfterAttack());
                audioFlyer.PlayOneShot(hitClip);
                //Agregar efecto de sonido
            }
        }
    }

    public void Dead()
    {
        dead = true;

        StopAllCoroutines(); //Detiene todas las corrutinas para evitar que el enemigo siga atacando o moviéndose después de morir.

        anim.SetTrigger("Dead"); //Animación de muerte      
        Physics2D.IgnoreCollision(enemyCollider, playerCollider, true); //Esto hace que el se ignoren las colisiones entre el enemigo y el jugador tras morir.

        characterController2D.SetRawMove(Vector2.zero); //Detiene el movimiento horizontal del personaje al morir, para que no siga moviéndose después de la animación de muerte

        StartCoroutine(Destroy()); //Inicia la corrutina para destruir el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena
                                   //
        splashCoins.Splash(); //Llama a la función Splash de SplashCoins para que el enemigo suelte monedas al morir.
        

    }


    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(2f);
        Destroy(gameObject); //Destruye el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            StopAttackImpulse();
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

        Gizmos.color = Color.pink;

        Gizmos.DrawWireSphere(transform.position, groundCheckDistance);

    }



}
