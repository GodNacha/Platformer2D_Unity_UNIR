using System;
using System.Collections;
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
    [SerializeField] public int lifes = 3;

    [Header("Srite Settings")]
    [SerializeField] public bool facingRightByDefault = true;

    [Header("Ground Check")]
    [SerializeField] public float groundCheckDistance = 0.2f;
    [SerializeField] LayerMask groundLayerMask;

    [Header("Combat")]
    [SerializeField] GameObject rightHitCollider;
    [SerializeField] GameObject leftHitCollider;

    private bool canMove = true;
    public bool dead = false;
    private bool attacking = false;
    bool canJump = true;
    bool canAttack = true;


    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        rightHitCollider.SetActive(false); 
        leftHitCollider.SetActive(false);

        dead = false;
    }


    const float Threshold = 0.1f;
    void Update()
    {
        if (!canMove) return; //Si el personaje no puede moverse, no se ejecuta el código de movimiento

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

    #region Movimiento

    Vector2 rawMove;
    public void SetRawMove(Vector2 rawMove)
    {
        this.rawMove = rawMove;
    }
    #endregion

    #region Salto 

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
        if (IsGrounded() && canJump)
        {
            anim.SetTrigger("Jump"); //Animación de salto
            rb.linearVelocityY = jumpVelocity;

        }

    }

    #endregion

    #region Ataque
    public void Attack()
    {
        if (attacking) return; //Si el personaje ya esta atacando, no se puede iniciar otro ataque

        if (IsGrounded() && canAttack)
        {
            anim.SetTrigger("Attack"); //Animación de ataque
            attacking = true;
        }
        else
        {
            anim.SetTrigger("Attack"); //Animación de ataque en el aire
            attacking = true;
        }

    }

    public void OnAttackAnimation()
    {
        if (spriteRenderer.flipX) //Si el sprite esta volteado, se activa el hit collider de la izquierda, de lo contrario se activa el hit collider de la derecha
        {
            leftHitCollider.SetActive(true);        
        }
        else
        {           
            rightHitCollider.SetActive(true);
        }

        Debug.Log("Golpe dado"); //Aquí es donde se haría el daño al enemigo, o la detección de colisiones con el enemigo para aplicar el daño, dependiendo de la implementación del sistema de combate que se quiera hacer
    }

    public void OnAttackAnimationEnd()
    {
        leftHitCollider.SetActive(false);
        rightHitCollider.SetActive(false);

        attacking = false;
    }

    #endregion  

    public void Damage()
    {
        if (!dead)
        {
            lifes--;
            if (lifes <= 0)
            {
                Dead();
                BlockActions(); //Bloquea las acciones del personaje al morir, para que no pueda moverse, saltar o atacar después de morir
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

        anim.SetBool("Dead", true); //Animación de muerte      

        SetRawMove(Vector2.zero); //Detiene el movimiento horizontal del personaje al morir, para que no siga moviéndose después de la animación de muerte

        StartCoroutine(Destroy()); //Inicia la corrutina para destruir el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena

    }

    void BlockActions()
    {
        canMove = false;
        canJump = false;
        canAttack = false;
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

