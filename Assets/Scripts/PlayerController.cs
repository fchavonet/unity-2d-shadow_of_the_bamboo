using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [Header("Components")]
    public Animator animator;
    public Rigidbody2D rigidbody2d;

    [Header("Movement Settings")]
    public float moveSpeed = 5f;
    public float jumpForce = 5f;
    public float flipOffsetX = -1f;

    [Header("Ground Detection")]
    public Transform groundCheck;
    public LayerMask groundLayer;
    public float groundCheckRadius = 0.2f;

    // Private variables
    private PlayerInput playerInput;
    private Vector2 moveInput;

    private bool isFacingRight = true;
    private bool isGrounded = false;

    private void Awake()
    {
        // Initialize the input system and bind actions.
        playerInput = new PlayerInput();

        // Registering movement actions.
        playerInput.Player.Move.performed += OnMovePerformed;
        playerInput.Player.Move.canceled += OnMoveCanceled;

        // Registering jump action.
        playerInput.Player.Jump.performed += OnJumpPerformed;
    }

    // FixedUpdate is called every fixed framerate frame.
    private void FixedUpdate()
    {
        // Ground check using Physics2D to determine if the player is on the ground.
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Moving the player horizontally
        rigidbody2d.velocity = new Vector2(moveInput.x * moveSpeed, rigidbody2d.velocity.y);

        // Flipping the player when changing direction.
        if (moveInput.x > 0 && !isFacingRight)
        {
            Flip();
        }
        else if (moveInput.x < 0 && isFacingRight)
        {
            Flip();
        }

        // Update player's animations.
        UpdateAnimation();
    }

    // Called when the object becomes enabled and active.
    private void OnEnable()
    {
        playerInput.Enable();
    }

    // Called when the object becomes disabled or inactive.
    private void OnDisable()
    {
        playerInput.Disable();
    }

    // Called when movement input is performed.
    private void OnMovePerformed(InputAction.CallbackContext context)
    {
        // Read movement input as Vector2.
        moveInput = context.ReadValue<Vector2>();
    }

    // Called when movement input is canceled.
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Reset movement input when the key is released.
        moveInput = Vector2.zero;
    }

    // Called when jump input is performed.
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        // Only jump if the player is grounded.
        if (isGrounded)
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // Flip the player's sprite and adjust position.
    private void Flip()
    {
        isFacingRight = !isFacingRight;

        // Flip the character's local scale.
        Vector3 scaler = transform.localScale;
        scaler.x *= -1;
        transform.localScale = scaler;

        // Adjust position based on facing direction.
        Vector3 newPosition = transform.position;

        if (isFacingRight)
        {
            newPosition.x -= flipOffsetX;
        }
        else
        {
            newPosition.x += flipOffsetX;
        }

        transform.position = newPosition;
    }

    // Update animator's speed parameter based on movement.
    private void UpdateAnimation()
    {
        if (animator != null)
        {
            // Update the "Speed" parameter in the animator based on the absolute value of the horizontal velocity.
            animator.SetFloat("Speed", Mathf.Abs(rigidbody2d.velocity.x));

            // If the player is in the air, handle jumping and falling animations.
            if (!isGrounded)
            {
                // If the player is moving upwards, play the jumping animation.
                if (rigidbody2d.velocity.y > 0)
                {
                    animator.SetBool("isJumping", true);
                    animator.SetBool("isFalling", false);
                }
                // If the player is moving upwards, play the jumping animation.
                else if (rigidbody2d.velocity.y < 0)
                {
                    animator.SetBool("isJumping", false);
                    animator.SetBool("isFalling", true);
                }
            }
            else
            {
                // If the player is grounded, reset both jumping and falling animations.
                animator.SetBool("isJumping", false);
                animator.SetBool("isFalling", false);
            }
        }
    }

    // Visualize the ground check area in the editor.
    private void OnDrawGizmos()
    {
        if (groundCheck != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(groundCheck.position, groundCheckRadius);
        }
    }
}
