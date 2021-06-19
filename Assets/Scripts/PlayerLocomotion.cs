using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Animations.Rigging;
using Cinemachine;

[RequireComponent(typeof(CharacterController))]
public class PlayerLocomotion : MonoBehaviour
{
    public Transform[] groundedChecks = new Transform[0];

    public List<Rig> aimingRigs = new List<Rig>(); //TODO: Maybe move this and other aiming logic to seperate aiming component?

    public CinemachineVirtualCamera aimCam;

    public float moveSpeed = 3f;
    public float strafeSpeed = 2f;
    public float rotationSpeed = 10.0f;
    public float jumpHeight = 5f;
    public float sprintSpeed = 5f;
    public float backupSpeedModifier = .5f;
    public float rollSpeed = 5f;
    public float aimDuration = .3f;

    private Animator animator;
    private CharacterController controller;
    private Transform cameraTransform;

    private Vector3 velocity;
    private Vector3 dodgeDir;
    private float dodgeModifier;

    private bool isJumping = false;
    private bool isGrounded = false;
    private bool isAiming = false;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        cameraTransform = Camera.main.transform;
    }

    private void Start()
    {
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    private void Update()
    {
        CheckForGrounded();
        HandleAiming();
        HandleMovement(InputHandler.Instance.moveInput);
        HandleBufferedInputs();

        isAiming = InputHandler.Instance.aimHeld && !animator.GetBool("IsDodging");
        animator.SetBool("IsSprinting", InputHandler.Instance.isSprinting);
        animator.SetBool("IsAiming", isAiming);
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
        InputHandler.Instance.playerInput.actions["Aim"].started += _ => OnPlayerAim(true);
        InputHandler.Instance.playerInput.actions["Aim"].canceled += _ => OnPlayerAim(false);
    }

    private void OnDisable()
    {
        if (InputHandler.Instance != null)
        {
            InputHandler.Instance.playerInput.actions["Jump"].performed -= _ => OnJump();
            InputHandler.Instance.playerInput.actions["Aim"].canceled -= _ => OnPlayerAim(false);
            InputHandler.Instance.playerInput.actions["Aim"].started -= _ => OnPlayerAim(false);
        }
    }

    private void HandleAiming()
    {
        if (isAiming)
        {
            aimCam.Priority = 12;
            foreach (Rig rig in aimingRigs)
            {
                rig.weight += Time.deltaTime / aimDuration;
            }
        }
        else
        {
            aimCam.Priority = 8;
            foreach (Rig rig in aimingRigs)
            {
                rig.weight -= Time.deltaTime / aimDuration;
            }
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

        if (isAiming && !animator.GetBool("IsDodging"))
        {
            adjustedPlayerSpeed = strafeSpeed;
        }

        // Changes the height position of the player..
        if (isJumping)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * 9.81f);
            isJumping = false;
        }

        move = move.x * cameraTransform.right.normalized + move.z * cameraTransform.forward.normalized;

        velocity.y -= 9.81f * 2 * Time.deltaTime;

        if (animator.GetBool("IsDodging"))
        {
            move = dodgeDir;
            dodgeModifier = rollSpeed;
        }
        else
        {
            dodgeDir = Vector3.zero;
            dodgeModifier = 1;
        }

        move.y = 0;
        controller.Move(adjustedPlayerSpeed * Time.deltaTime * move.normalized * dodgeModifier + velocity * Time.deltaTime);

        Quaternion targetRotation = transform.rotation;

        if (isAiming || InputHandler.Instance.isSprinting)
        {
            targetRotation = Quaternion.Euler(0, cameraTransform.eulerAngles.y, 0);
        }
        else if (move.magnitude > 0)
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

    private void HandleBufferedInputs()
    {
        InputBufferAction.ActionName action = InputHandler.Instance.TryBufferedAction();

        if (action == InputBufferAction.ActionName.None)
        {
            return;
        }

        if (action == InputBufferAction.ActionName.Dodge)
        {
            HandleDodge();
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
        if (!isGrounded || animator.GetBool("IsInteracting") || isJumping)
        {
            return;
        }

        animator.SetTrigger("Jump");
        isJumping = true;
    }

    private void HandleDodge()
    {
        if (animator.GetBool("IsInteracting") || animator.GetBool("IsDodging"))
        {
            return;
        }

        InputHandler.Instance.UseBufferedAction(InputBufferAction.ActionName.Dodge);
        animator.SetBool("IsDodging", true);

        foreach (Rig rig in aimingRigs)
        {
            rig.weight = 0;
        }

        //Invoke("SlowTime", .3f);

        // TODO: Create method to get current dir vector based on camera, used above as well
        dodgeDir = new Vector3(InputHandler.Instance.moveInput.x, 0, InputHandler.Instance.moveInput.y);
        dodgeDir = dodgeDir.x * cameraTransform.right.normalized + dodgeDir.z * cameraTransform.forward.normalized;

        // If we are standing still, roll in place where we are facing
        if (dodgeDir == Vector3.zero)
        {
            dodgeDir = cameraTransform.forward;
        }
    }

    private void SlowTime()
    {
        Time.timeScale = Mathf.Epsilon;
    }

    private void OnPlayerAim(bool test)
    {
    }

    public void ToggleIsInteracting(int toggle)
    {
        animator.SetBool("IsInteracting", toggle == 0 ? false : true);
    }
}