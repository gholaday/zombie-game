using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CharacterController))]
public class PlayerLocomotion : MonoBehaviour
{
    public Transform[] groundedChecks = new Transform[0];

    public float moveSpeed = 3f;
    public float rotationSpeed = 10.0f;
    public float jumpHeight = 5f;
    public float sprintSpeed = 5f;
    public float backupSpeedModifier = .5f;
    public float rollSpeed = 5f;

    private Animator animator;
    private CharacterController controller;
    private Transform cameraTransform;

    private Vector3 velocity;
    private Vector3 dodgeDir;
    private float dodgeModifier;

    private bool isJumping = false;
    private bool isGrounded = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
    }

    // Update is called once per frame
    private void Update()
    {
        CheckForGrounded();
        HandleMovement(InputHandler.Instance.moveInput);

        animator.SetBool("IsSprinting", InputHandler.Instance.isSprinting);
        animator.SetFloat("Horizontal", InputHandler.Instance.moveInput.x, 1f, Time.deltaTime * 5f);
        animator.SetFloat("Vertical", InputHandler.Instance.moveInput.y, 1f, Time.deltaTime * 5f);
    }

    private void FixedUpdate()
    {
    }

    private void LateUpdate()
    {
    }

    private void OnEnable()
    {
        InputHandler.Instance.playerInput.actions["Jump"].performed += _ => OnJump();
        InputHandler.Instance.playerInput.actions["Dodge"].performed += _ => OnDodge();
        InputHandler.Instance.playerInput.actions["Aim"].started += _ => OnPlayerAim(true);
        InputHandler.Instance.playerInput.actions["Aim"].canceled += _ => OnPlayerAim(false);
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.playerInput.actions["Jump"].performed -= _ => OnJump();
            InputHandler.Instance.playerInput.actions["Dodge"].performed -= _ => OnDodge();
            InputHandler.Instance.playerInput.actions["Aim"].canceled -= _ => OnPlayerAim(false);
            InputHandler.Instance.playerInput.actions["Aim"].started -= _ => OnPlayerAim(false);
        }
    }

    private void HandleMovement(Vector2 input)
    {
        Vector3 move = new Vector3(input.x, 0, input.y);
        float adjustedPlayerSpeed = moveSpeed;

        if (InputHandler.Instance.isSprinting)
        {
            adjustedPlayerSpeed = sprintSpeed;
            move.x = 0;
        }
        //else if (input.y < 0)
        //{
        //    adjustedPlayerSpeed *= backupSpeedModifier;
        //}

        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;
        move.y = 0;

        // Changes the height position of the player..
        if (isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 9.81f);
            isJumping = false;
        }

        velocity.y -= 9.81f * 2 * Time.deltaTime;

        if (animator.GetBool("IsDodging"))
        {
            move = dodgeDir;
            move.y = 0;
            dodgeModifier = rollSpeed;
        }
        else
        {
            dodgeDir = Vector3.zero;
            dodgeModifier = 1;
        }

        controller.Move(adjustedPlayerSpeed * Time.deltaTime * move.normalized * dodgeModifier + velocity * Time.deltaTime);

        Quaternion targetRotation = transform.rotation;

        if (animator.GetBool("IsAiming"))
        {
            targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        }
        else if (input.magnitude > 0)
        {
            targetRotation = Quaternion.LookRotation(move);
        }

        transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
            animator.SetBool("IsGrounded", true);
        }
        else
        {
            animator.SetBool("IsGrounded", false);
        }
    }

    private void CheckForGrounded()
    {
        isGrounded = false;
        if (groundedChecks != null && groundedChecks.Length > 0)
        {
            foreach (Transform t in groundedChecks)
            {
                if (Physics.Raycast(t.position, Vector3.down, .5f))
                {
                    Debug.DrawLine(t.position, t.position + Vector3.down * .5f, Color.green);
                    isGrounded = true;
                }
                else
                {
                    Debug.DrawLine(t.position, t.position + Vector3.down * .5f, Color.red);
                }
            }
        }
    }

    private void OnJump()
    {
        if (!isGrounded || animator.GetBool("IsInteracting"))
        {
            return;
        }

        animator.SetTrigger("Jump");
        isJumping = true;
    }

    private void OnDodge()
    {
        if (animator.GetBool("IsInteracting") || animator.GetBool("IsDodging") || !isGrounded)
        {
            return;
        }

        animator.SetBool("IsDodging", true);

        // TODO: Create method to get current dir vector based on camera, used above as well
        dodgeDir = new Vector3(InputHandler.Instance.moveInput.x, 0, InputHandler.Instance.moveInput.y);
        dodgeDir = dodgeDir.x * cameraTransform.right.normalized + dodgeDir.z * cameraTransform.forward.normalized;
    }

    private void OnPlayerAim(bool test)
    {
    }

    public void ToggleIsInteracting(int toggle)
    {
        animator.SetBool("IsInteracting", toggle == 0 ? false : true);
    }
}