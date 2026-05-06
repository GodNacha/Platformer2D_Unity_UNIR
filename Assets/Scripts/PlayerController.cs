using System;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    CharacterController characterController2D;

    [Header("Inputs")]
    [SerializeField] InputActionReference move;
    [SerializeField] InputActionReference jump;
    [SerializeField] InputActionReference attack;

    private void Awake()
    {
        characterController2D = GetComponent<CharacterController>();

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

    Vector2 rawMove;
    private void OnMove(InputAction.CallbackContext context)
    {
        rawMove = context.ReadValue<Vector2>();
        characterController2D.SetRawMove(rawMove);
    }
    private void OnAttack(InputAction.CallbackContext context)
    {
        throw new NotImplementedException();
    }

    private void OnJump(InputAction.CallbackContext context)
    {
        characterController2D.Jump();
    }

    private void OnDisable()
    {
        move.action.Disable();
        jump.action.Disable();
        attack.action.Disable();
    }
}
