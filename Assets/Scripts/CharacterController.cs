using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody))] //Agrega el componente Rigidbody al GameObject si no lo tiene, esto unicamente cuando se agregar el script al GameObject
public class CharacterController : MonoBehaviour
{
    [Header("Inputs")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference attack;

    Rigidbody2D rb;
    Animator anim;
    SpriteRenderer spriteRenderer;

    [Header("Stats")]
    [SerializeField] public float movementSpeed = 5f;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();

        move.action.performed += OnMove;
        move.action.canceled += OnMove;
        move.action.started += OnMove;

        jump.action.performed += OnJump;
        attack.action.performed += OnAttack;
    }

    private void OnEnable()
    {
        move.action.Enable();
        jump.action.Enable();
        attack.action.Enable();
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }


    const float Threshold = 0.1f;
    void Update()
    {
        rb.linearVelocityX = rawMove.x * movementSpeed;
        bool isMoving = Mathf.Abs(rawMove.x) > Threshold;

        anim.SetBool("IsRunning", isMoving);

        if (isMoving)
        {
            spriteRenderer.flipX = rawMove.x < 0;
        }

    }

    Vector2 rawMove;
    private void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.ReadValue<Vector2>();
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        attack.action.Disable();
    }
}

