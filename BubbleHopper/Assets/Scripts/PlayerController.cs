using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float jumpForce = 10f;
    [SerializeField] private float gravity = -9.81f;

    private CharacterController characterController;
    private Vector3 moveDirection;
    private bool isJumping = false;
    private bool isGrounded;

    private PlayerInput playerInput;
    private InputAction moveAction;
    private InputAction jumpAction;

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();

        moveAction = playerInput.actions["Move"];
        jumpAction = playerInput.actions["Jump"];
    }

    private void Update()
    {
        isGrounded = characterController.isGrounded;

        // Get movement input
        Vector2 moveInput = moveAction.ReadValue<Vector2>();

        // Only allow movement on the X-axis (left and right)
        Vector3 move = transform.right * moveInput.x * moveSpeed;

        // Handle jumping (forward direction)
        if (isGrounded && jumpAction.triggered)
        {
            isJumping = true;
            moveDirection.y = jumpForce;
        }

        // Apply gravity
        if (!isGrounded)
        {
            moveDirection.y += gravity * Time.deltaTime;
        }
        else
        {
            moveDirection.y = 0f;
        }

        // Apply movement and jumping
        moveDirection.x = move.x;
        characterController.Move(moveDirection * Time.deltaTime);
    }
}