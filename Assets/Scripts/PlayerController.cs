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
    [SerializeField] int coins = 0;
    [SerializeField] public int lifes = 3;
    [SerializeField] private Animator anim;


    private bool dead = false;


    [Header("Disable Scripts After Dead")]
    public MonoBehaviour[] scriptsToDisableOnDeath; //Aquí se pueden agregar los scripts que se quieran desactivar al morir, como el script de movimiento, ataque, etc, para evitar que el personaje siga moviéndose o atacando después de morir

    CharacterController characterController2D;

    [Header("Inputs")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference attack;

    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();
        anim = GetComponent<Animator>();

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
        rawMove = context.ReadValue<Vector2>();
        characterController2D.SetRawMove(rawMove);
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        characterController2D.Attack();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        characterController2D.Jump();
    }  

    public void AddScoreCoin()
    {
        //Reproducir sonido de recoger moneda

        coins++;
        textCoins.text = "X " + coins;
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

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        attack.action.Disable();
    }
}
