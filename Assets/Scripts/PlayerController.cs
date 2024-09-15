using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    public static PlayerController instance;

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

    // Private variables.
    private PlayerInput playerInput;
    private Vector2 moveInput;

    private bool isGrounded = false;
    private bool wasGrounded = false;
    private bool canMove = true;
    private bool isMoving = false;
    private bool isFacingRight = true;

    // Public variables, but hidden.
    [HideInInspector]
    public bool isAttacking = false;
    [HideInInspector]
    public bool isHeliSlamAttacking = false;
    [HideInInspector]
    public bool isRollAttacking = false;

    private void Awake()
    {
        // Singleton pattern - Ensure only one instance of PlayerController exists.
        instance = this;

        // Initialize the input system and bind actions.
        playerInput = new PlayerInput();

        // Registering movement actions.
        playerInput.Player.Move.performed += OnMovePerformed;
        playerInput.Player.Move.canceled += OnMoveCanceled;

        // Registering jump action.
        playerInput.Player.Jump.performed += OnJumpPerformed;

        // Registering attack actions.
        playerInput.Player.Attacks.performed += OnAttackPerformed;
        playerInput.Player.HeliSlamAttack.performed += OnHeliSlamAttackPerformed;
        playerInput.Player.RollAttack.performed += OnRollAttackPerformed;
    }

    // FixedUpdate is called every fixed framerate frame.
    private void FixedUpdate()
    {
        // Ground check using Physics2D to determine if the player is on the ground.
        wasGrounded = isGrounded;
        isGrounded = Physics2D.OverlapCircle(groundCheck.position, groundCheckRadius, groundLayer);

        // Moving the player horizontally
        if (canMove)
        {
            rigidbody2d.velocity = new Vector2(moveInput.x * moveSpeed, rigidbody2d.velocity.y);
        }

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

            // Reset the roll attack trigger if the player lands
            if (isGrounded && !wasGrounded)
            {
                animator.ResetTrigger("isJumpingWithRollAttack");
                isRollAttacking = false;
            }
        }
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
        isMoving = true;
    }

    // Called when movement input is canceled.
    private void OnMoveCanceled(InputAction.CallbackContext context)
    {
        // Reset movement input when the key is released.
        moveInput = Vector2.zero;
        isMoving = false;
    }

    // Called when jump input is performed.
    private void OnJumpPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded)
        {
            rigidbody2d.velocity = new Vector2(rigidbody2d.velocity.x, jumpForce);
            isGrounded = false;
        }
    }

    // Called when attack input is performed.
    private void OnAttackPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded && !isAttacking)
        {
            isAttacking = true;
        }
    }

    // Called when heli slam attack input is performed.
    private void OnHeliSlamAttackPerformed(InputAction.CallbackContext contex)
    {
        if (isGrounded && !isMoving && !isHeliSlamAttacking)
        {
            isHeliSlamAttacking = true;
        }
    }

    // Called when roll attack input is performed.
    private void OnRollAttackPerformed(InputAction.CallbackContext context)
    {
        if (isGrounded && !isRollAttacking)
        {
            isRollAttacking = true;
        }

        if (!isGrounded && !isRollAttacking)
        {
            animator.SetTrigger("isJumpingWithRollAttack");
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
