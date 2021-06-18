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

    public List<InputBufferAction> inputBuffer = new List<InputBufferAction>();

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

    public InputBufferAction.ActionName TryBufferedAction()
    {
        if (inputBuffer.Count > 0)
        {
            foreach (InputBufferAction iba in inputBuffer.ToArray())
            {
                if (iba.IsExpired())
                {
                    inputBuffer.Remove(iba);
                }
                else
                {
                    return iba.Action;
                }
            }
        }

        return InputBufferAction.ActionName.None;
    }

    public void UseBufferedAction(InputBufferAction.ActionName actionName)
    {
        InputBufferAction iba = inputBuffer.Find(x => x.Action == actionName);

        if (iba != null)
        {
            inputBuffer.Remove(iba);
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

    private void OnDodge(InputValue value)
    {
        if (inputBuffer.Exists(x => x.Action == InputBufferAction.ActionName.Dodge))
        {
            return;
        }

        inputBuffer.Add(InputBufferAction.Create(InputBufferAction.ActionName.Dodge, .3f));
    }
}

public class InputBufferAction
{
    public enum ActionName { None, Dodge }

    public ActionName Action;

    public float expirationTime;
    public float createdTime;

    public static InputBufferAction Create(ActionName action, float expiration)
    {
        InputBufferAction newAction = new InputBufferAction();
        newAction.Action = action;
        newAction.expirationTime = expiration;
        newAction.createdTime = Time.time;

        return newAction;
    }

    public bool IsExpired()
    {
        if (createdTime + expirationTime >= Time.time)
        {
            return false;
        }

        return true;
    }
}