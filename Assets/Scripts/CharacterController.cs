using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))] //Agrega el componente Rigidbody al GameObject si no lo tiene, esto unicamente cuando se agregar el script al GameObject
public class CharacterController : MonoBehaviour
{
    public Rigidbody2D rb;
    Animator anim;
    [SerializeField] public SpriteRenderer spriteRenderer;


    [Header("Stats")]
    [SerializeField] public float movementSpeed = 5f;
    [SerializeField] public float jumpVelocity = 3f;
    [SerializeField] public int lifes = 3;
    [SerializeField] public bool isEnemy = false; //Esto es para diferenciar entre el jugador y los enemigos.
    [SerializeField] public bool isEnemyFryer = false;
    [SerializeField] bool delayHitActivate = true;
    [SerializeField] bool visualHitColliderActivate = false;


    [Header("Srite Settings")]
    [SerializeField] public bool facingRightByDefault = true;

    [Header("Ground Check")]
    [SerializeField] public float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayerMask;

    [Header("Combat")]
    [SerializeField] public Collider2D rightHitCollider;
    [SerializeField] public Collider2D leftHitCollider;
    [SerializeField] float desactivatehitDelay = 0.25f; //Duración de la animación de ataque, se puede ajustar dependiendo de la animación que se use

    public bool canMove = true;
    public bool attacking = false;
    bool canJump = true;
    private bool isMoving = false;

    private bool touchedFloor = true;

    private SpriteRenderer rightCollider;
    private SpriteRenderer leftCollider;

    [Header("References")]
    GameManager gameManager;

    [Header("Audio")]
    public AudioSource audioMovement;
    public AudioClip jumpClip;
    public AudioClip moveClip;
    public AudioClip touchFloorClip;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();     
        gameManager = FindAnyObjectByType<GameManager>();
        rightCollider = rightHitCollider.GetComponent<SpriteRenderer>();
        leftCollider = leftHitCollider.GetComponent<SpriteRenderer>();

        if (audioMovement == null)
        {
            Debug.LogWarning("AudioSource for movement is not assigned. Attempting to find one on the GameObject.");
        }


    }

    private void Start()
    {
        rightHitCollider.enabled = false;
        leftHitCollider.enabled = false;

        if (visualHitColliderActivate)
        {
            rightCollider.color = new Color(1f, 1f, 0f, 0.3f);
            leftCollider.color = new Color(1f, 1f, 0f, 0.3f);

        }
    }

    const float Threshold = 0.1f;
    void Update()
    {
        if (canMove && !gameManager.endGame)
        {
            if (!isEnemyFryer)
            {
                rb.linearVelocityX = rawMove.x * movementSpeed;

            }
            else
            {
                rb.linearVelocity = rawMove * movementSpeed; //Ver si funciona
            }
                
        }
        else
        {       
            if (!isEnemy && !canMove)
            {
                rb.linearVelocityX = 0; //El jugador se queda quieto al atacar.

                audioMovement.Stop();

                audioMovement.loop = false;
            }
            
        }
  

        bool isMoving = Mathf.Abs(rawMove.x) > Threshold;

        anim.SetBool("IsRunning", isMoving);

        if (isMoving)
        {

            bool movingLeft = rawMove.x < 0; // Esto para que los sprite que tiuenen una posición default/original mirando a la derecha, se volteen al moverse a la izquierda, y viceversa

            spriteRenderer.flipX = facingRightByDefault
                ? movingLeft
                : !movingLeft;

            if (IsGrounded() && !isEnemyFryer && !attacking && !audioMovement.isPlaying)
            {
                audioMovement.clip = moveClip;
                audioMovement.loop = true;
                audioMovement.Play();
            }
            else
            {
                audioMovement.loop = false;
            }

            if (isEnemyFryer && !attacking && !audioMovement.isPlaying)
            {
                audioMovement.clip = moveClip;
                audioMovement.loop = true;
                audioMovement.Play();
            }
            else
            {
                if (isEnemyFryer && attacking || rawMove == Vector2.zero)
                {
                    audioMovement.loop = false;
                }
         
            }

        }
        else
        {
            audioMovement.loop = false;
        }

        if (!isEnemyFryer)
        {
            anim.SetBool("IsGrounded", IsGrounded()); //Animación volando en el aire

            if (IsGrounded() && rb.linearVelocityY < 0) //Esto es para reproducir el sonido de tocar el suelo, solo se reproduce si el personaje esta cayendo, para evitar que se reproduzca el sonido al saltar
            {
                if (touchFloorClip != null && !touchedFloor && !isEnemyFryer)
                {
                    audioMovement.loop = false;
                    audioMovement.PlayOneShot(touchFloorClip);
                    touchedFloor = true;
                }


            }
        }

        

    }

    #region Movimiento

    Vector2 rawMove;
    public void SetRawMove(Vector2 rawMove)
    {
        if (canMove)
        {
            this.rawMove = rawMove;
                 
        }
        else       
        {
            this.rawMove = Vector2.zero; //Si no se puede mover, el rawMove se establece en cero para que el personaje no se mueva.

            audioMovement.loop = false;
        }

    }

    #endregion

    #region Salto 

    public bool IsGrounded()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, groundCheckDistance, groundLayerMask); //Esto es para detectar si el personaje esta tocando el suelo, se puede ajustar la distancia del raycast dependiendo de la altura del personaje     

        return hit & hit.collider != null;
    }

    void OnDrawGizmos()
    {

        Gizmos.color = IsGrounded() ? Color.green : Color.red; // Usamos la función lógica para decidir el color

        Gizmos.DrawLine(transform.position, transform.position + (Vector3.down * groundCheckDistance)); // Dibuja una línea desde la posición del personaje hacia abajo, con la longitud de groundCheckDistance, para visualizar el raycast en la escena y hacer los ajustes pertinentes
    }

    public void Jump()
    {
        if (IsGrounded() && canJump)
        {
            anim.SetTrigger("Jump"); //Animación de salto
            rb.linearVelocityY = jumpVelocity;

            audioMovement.loop = false;

            audioMovement.PlayOneShot(jumpClip); //Reproduce el sonido de salto

            touchedFloor = false;

        }

    }

    #endregion

    #region Ataque Evento
    public void OnAttackAnimation()
    {
        if (spriteRenderer.flipX) //Si el sprite esta volteado, se activa el hit collider de la izquierda, de lo contrario se activa el hit collider de la derecha
        {
            leftHitCollider.enabled = true;

            if (visualHitColliderActivate)
            {
                leftCollider.color = new Color(1f, 0f, 0f, 0.5f);

            }


            if (delayHitActivate)
            {
                Invoke(nameof(DesactivateHits), desactivatehitDelay); //Invoca la función DesactivateHits después de un tiempo, para desactivar los hit colliders después de que se haya completado la animación de ataque
            }
            
        }
        else
        {           
            rightHitCollider.enabled = true;

            if (visualHitColliderActivate)
            {
                rightCollider.color = new Color(1f, 0f, 0f, 0.5f);

            }

            if (delayHitActivate)
            {
                Invoke(nameof(DesactivateHits), desactivatehitDelay); //Invoca la función DesactivateHits después de un tiempo, para desactivar los hit colliders después de que se haya completado la animación de ataque
            }
        }

        Debug.Log("Golpe dado"); //Aquí es donde se haría el daño al enemigo, o la detección de colisiones con el enemigo para aplicar el daño, dependiendo de la implementación del sistema de combate que se quiera hacer
    }

    void DesactivateHits()
    {
        leftHitCollider.enabled = false;
        rightHitCollider.enabled = false;

        if (visualHitColliderActivate)
        {
            rightCollider.color = new Color(1f, 1f, 0f, 0.3f);
            leftCollider.color = new Color(1f, 1f, 0f, 0.3f);
        }
            
    }

   


    #endregion




}

