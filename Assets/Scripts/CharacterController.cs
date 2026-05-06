using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))] //Agrega el componente Rigidbody al GameObject si no lo tiene, esto unicamente cuando se agregar el script al GameObject
public class CharacterController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer spriteRenderer;


    [Header("Stats")]
    [SerializeField] public float movementSpeed = 5f;
    [SerializeField] public float jumpVelocity = 3f;

    [Header("Srite Settings")]
    [SerializeField] public bool facingRightByDefault = true;

    [Header("Ground Check")]
    [SerializeField] public float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayerMask;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

    }


    const float Threshold = 0.1f;
    void Update()
    {
        rb.linearVelocityX = rawMove.x * movementSpeed;
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

    Vector2 rawMove;
    public void SetRawMove (Vector2 rawMove)
    {
        this.rawMove = rawMove;
    }

    bool IsGrounded()
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
        if (IsGrounded())
        {
            anim.SetTrigger("Jump"); //Animación de salto
            rb.linearVelocityY = jumpVelocity;
            
        }
        
    }


}

