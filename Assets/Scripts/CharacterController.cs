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
    SpriteRenderer spriteRenderer;


    [Header("Stats")]
    [SerializeField] public float movementSpeed = 5f;
    [SerializeField] public float jumpVelocity = 3f;
    [SerializeField] public int lifes = 3;
    [SerializeField] public bool isEnemy = false; //Esto es para diferenciar entre el jugador y los enemigos.

    [Header("Srite Settings")]
    [SerializeField] public bool facingRightByDefault = true;

    [Header("Ground Check")]
    [SerializeField] public float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayerMask;

    [Header("Combat")]
    [SerializeField] public Collider2D rightHitCollider;
    [SerializeField] public Collider2D leftHitCollider;

    [Header("Script Disables")]
    [SerializeField] MonoBehaviour[] scriptsToDisableOnDeath; //Aquí se pueden agregar los scripts que se quieran desactivar al morir, como el script de movimiento, ataque, etc, para evitar que el personaje siga moviéndose o atacando después de morir

    public bool canMove = true;
    public bool attacking = false;
    bool canJump = true;

    [Header("References")]
    GameManager gameManager;



    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();     
        gameManager = FindAnyObjectByType<GameManager>();


    }

    private void Start()
    {
        rightHitCollider.enabled = false;
        leftHitCollider.enabled = false;

        
    }


    const float Threshold = 0.1f;
    void Update()
    {
        if (canMove && !gameManager.endGame)
        {
            rb.linearVelocityX = rawMove.x * movementSpeed;
        }
        else
        {       
            if (!isEnemy || gameManager.endGame)
            {
                rb.linearVelocityX = 0; //El jugador se queda quieto al atacar.
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

        }

        anim.SetBool("IsGrounded", IsGrounded()); //Animación volando en el aire

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

        }

    }

    #endregion

    #region Ataque Evento

    const float desactivatehitDelay = 0.25f; //Duración de la animación de ataque, se puede ajustar dependiendo de la animación que se use
    public void OnAttackAnimation()
    {
        if (spriteRenderer.flipX) //Si el sprite esta volteado, se activa el hit collider de la izquierda, de lo contrario se activa el hit collider de la derecha
        {
            leftHitCollider.enabled = true;
            Invoke(nameof(DesactivateHits), desactivatehitDelay); //Invoca la función DesactivateHits después de un tiempo, para desactivar los hit colliders después de que se haya completado la animación de ataque
        }
        else
        {           
            rightHitCollider.enabled = true;
            Invoke(nameof(DesactivateHits), desactivatehitDelay);
        }

        Debug.Log("Golpe dado"); //Aquí es donde se haría el daño al enemigo, o la detección de colisiones con el enemigo para aplicar el daño, dependiendo de la implementación del sistema de combate que se quiera hacer
    }

    void DesactivateHits()
    {
        leftHitCollider.enabled = false;
        rightHitCollider.enabled = false;
        
    }

   


    #endregion




}

