using System;
using System.Collections;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI textLifes;
    public TextMeshProUGUI textCoins;

    [Header("Player Stats")]
    [SerializeField] public int coins = 0;
    [SerializeField] public int lifes = 3;
    [SerializeField] private Animator anim;


    private bool dead = false;
    bool canAttack = true;


    [Header("Disable Scripts After Dead")]
    public MonoBehaviour[] scriptsToDisableOnDeath; //Aquí se pueden agregar los scripts que se quieran desactivar al morir, como el script de movimiento, ataque, etc, para evitar que el personaje siga moviéndose o atacando después de morir

    CharacterController characterController2D;
    GameManager gameManager;

    [Header("Inputs")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference attack;

    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();
        gameManager = FindAnyObjectByType<GameManager>();

        move.action.performed += OnMove;
        move.action.canceled += OnMove;
        move.action.started += OnMove;

        jump.action.performed += OnJump;
        attack.action.performed += OnAttack;

        textCoins.text = "X " + coins;
        textLifes.text = "X " + lifes;
    }
    


    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        attack.action.Enable();
    }

    Vector2 rawMove;
    private void OnMove(InputAction.CallbackContext context)
    {
        if (!dead)
        {
            rawMove = context.ReadValue<Vector2>();
            characterController2D.SetRawMove(rawMove);
        }
        
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        if (!dead)
        {
            Attack();
        }
       
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        if (!dead)
        {
            characterController2D.Jump();
        }
           
    }  

    public void AddScoreCoin()
    {
        //Agregar reproducción de sonido al recoger moneda

        coins++;
        textCoins.text = "X " + coins;
    }

    public void Attack()
    {
        if (characterController2D.attacking == true) return; //Si el personaje ya esta atacando, no se puede iniciar otro ataque       

        if (characterController2D.IsGrounded() && canAttack)
        {
            characterController2D.canMove = false;

            anim.SetTrigger("Attack"); //Animación de ataque
            characterController2D.attacking = true;
            StartCoroutine(AfterAttack());

        }
        else
        {
            anim.SetTrigger("Attack"); //Animación de ataque en el aire
            characterController2D.attacking = true;
            StartCoroutine(AfterAttack());

        }

    }

    public IEnumerator AfterAttack()
    {
        yield return new WaitForSeconds(0.5f); //Tiempo de espera después de atacar, para que el enemigo no pueda atacar constantemente sin esperar un tiempo entre ataques, esto se puede ajustar dependiendo de la velocidad de ataque que se quiera para el enemigo
        characterController2D.attacking = false;
        characterController2D.canMove = true;

        characterController2D.SetRawMove(rawMove);
    }

    public void Damage()
    {
        if (!dead)
        {
            lifes--;
            textLifes.text = "X " + lifes;

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

        anim.SetTrigger("Dead"); //Animación de muerte
        
        characterController2D.canMove = false; //Evita que el personaje se mueva después de morir, para que no siga moviéndose después de la animación de muerte     

        StartCoroutine(Destroy()); //Inicia la corrutina para destruir el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena

    }

    IEnumerator Destroy()
    {
        yield return new WaitForSeconds(1.5f);

        gameManager.GameOver(); //Llama a la función GameOver del GameManager para mostrar la pantalla de Game Over después de morir

        yield return new WaitForSeconds(2f);

        Destroy(gameObject); //Destruye el GameObject después de un tiempo, para que la animación de muerte se reproduzca completamente antes de eliminar el personaje de la escena
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone"))
        {
            Dead();
        }
    }

    public void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        attack.action.Disable();
    }
}
