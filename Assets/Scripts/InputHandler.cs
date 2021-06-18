using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputHandler : Singleton<InputHandler>
{
    public PlayerInput playerInput;
    public Vector2 moveInput;
    public bool isSprinting = false;
    public bool isAiming = false;

    private void Awake()
    {
        playerInput = FindObjectOfType<PlayerInput>();
    }

    private void Update()
    {
        moveInput = playerInput.actions["Move"].ReadValue<Vector2>();

        if (moveInput.y <= 0)
        {
            isSprinting = false;
        }
    }

    private void OnSprint(InputValue value)
    {
        isSprinting = value.isPressed && moveInput.y > 0;
    }

    private void OnAim(InputValue value)
    {
        isAiming = value.isPressed;
    }
}