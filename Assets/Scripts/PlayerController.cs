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
    private bool dieByZone = false; //Variable para saber si el jugador murió por caer en la DeadZone, esto es para aplicar un efecto de salto al morir en la DeadZone
    bool inmune = false;
    private Coroutine attackCorutine;
    private bool canceledAttack = false;


    [Header("References")]
    [SerializeField] CharacterController characterController2D;
    [SerializeField] GameManager gameManager;

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

        dead = false;
        dieByZone = false;
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
        if (!dead && !characterController2D.attacking)
        {
            attackCorutine = StartCoroutine(Attack());
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

    public IEnumerator Attack()
    {
        canceledAttack = false;

        if (characterController2D.attacking == true || dead)
        {
            yield break;
        }//Si el personaje ya esta atacando, no se puede iniciar otro ataque       

        characterController2D.attacking = true;

        if (characterController2D.IsGrounded())
        {           
            anim.SetTrigger("Attack"); //Animación de ataque
            characterController2D.canMove = false;

        }
        else
        {
            anim.SetTrigger("Attack"); //Animación de ataque en el aire
            
        }
     
        yield return new WaitForSeconds(0.15f); 
        
        if (canceledAttack || dead)
        {
            yield break;
        }
        else
        {
            StartCoroutine(AfterAttack());
        }
       
    }

    public IEnumerator AfterAttack()
    {      
        yield return new WaitForSeconds(0.23f); //Tiempo de espera después de atacar, para que el enemigo no pueda atacar constantemente sin esperar un tiempo entre ataques, esto se puede ajustar dependiendo de la velocidad de ataque que se quiera para el enemigo
        characterController2D.attacking = false;
        characterController2D.canMove = true;
        inmune = false;
        canceledAttack = false;

        characterController2D.SetRawMove(rawMove);
    }

    public void CancelAttack() //Se supone que, como se sobreescribe la animación de attack por la hit, la aniamción no se alcanzará a ejecutar por completo, por ende, lo boz colliders no les pasará nada.
    {
        canceledAttack = true;
        StopCoroutine(attackCorutine);
        attackCorutine = null;
        StartCoroutine(CanceledAttack());
    }

    public IEnumerator CanceledAttack()
    {
        inmune = true;
        yield return new WaitForSeconds(0.2f); //Tiempo de espera después de atacar, para que el enemigo no pueda atacar constantemente sin esperar un tiempo entre ataques, esto se puede ajustar dependiendo de la velocidad de ataque que se quiera para el enemigo
        characterController2D.attacking = false;
        characterController2D.canMove = true;
        inmune = false;
        canceledAttack = false;
        characterController2D.SetRawMove(rawMove);
    }

    public void Damage()
    {
        if (!dead && !inmune)
        {
            lifes--;
            textLifes.text = "X " + lifes;

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
                                 //
        gameManager.GameOver(); //Llama a la función GameOver del GameManager para mostrar la pantalla de Game Over después de morir

        if (dieByZone)
        {
            characterController2D.rb.linearVelocityX = 0f; //Esto hace que el personaje salte un poco al caer en la DeadZone

            characterController2D.rb.linearVelocityY = 3.5f; //Esto hace que el personaje salte un poco al caer en la DeadZone
        }
        else
        {
            characterController2D.canMove = false; //Evita que el personaje se mueva después de morir, para que no siga moviéndose después de la animación de muerte

        }

    }  

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("DeadZone") && !dieByZone)
        {
            dieByZone = true;
            Dead();
            
        }
    }

    public void NoMovement()
    {
        characterController2D.canMove = false;
    }

    public void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        attack.action.Disable();
    }
}
