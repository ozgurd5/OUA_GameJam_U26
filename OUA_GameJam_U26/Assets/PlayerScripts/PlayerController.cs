using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Assign")]
    [SerializeField] private float speedDefault;
    [SerializeField] private float jumpSpeedDefault;
    [SerializeField] private float speedRunning;
    [SerializeField] private float jumpSpeedRunning;
    [SerializeField] private int extraJumpCountLimit;
    [SerializeField] private Transform feet;
    [SerializeField] private PlayerData playerData;

    private float moveInput;
    private float speed;
    private float jumpSpeed;
    private float jumpBufferLimit = 0.1f;
    private float jumpBufferTimer;
    private float coyoteTimeLimit = 0.1f;
    private float coyoteTimer;
    private int extraJumpCounter;
    private RaycastHit2D groundCheck;
    private Rigidbody2D rb;
    
    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update()
    {
        //horizontal movement
        moveInput = Input.GetAxisRaw("Horizontal");
        rb.velocity = new Vector2(moveInput * speed, rb.velocity.y);
        //horizontal movement

        //state calculation
        if (moveInput != 0)
        {
            playerData.isIdle = false;
            
            if (moveInput == 1f)
            {
                playerData.isFacedRight = true; 
            }

            else
            {
                playerData.isFacedRight = false;
            }
            
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerData.isRunning = true;
            }

            else
            {
                playerData.isRunning = false;
                playerData.isWalking = true;
            }
        }

        else
        {
            playerData.isIdle = true;
            playerData.isWalking = false;
            playerData.isRunning = false;
        }
        //state calculation

        //jump buffer
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jumpBufferTimer = jumpBufferLimit;
        }

        else
        {
            jumpBufferTimer -= Time.deltaTime;
        }
        //jump buffer
        
        //coyote time
        if (isGrounded())
        {
            coyoteTimer = coyoteTimeLimit;
            extraJumpCounter = extraJumpCountLimit; //extra jump
            playerData.isJumping = false; //jump state
        }
        
        else
        {
            coyoteTimer -= Time.deltaTime;
        }
        //coyote time

        //jump
        if (coyoteTimer > 0f && jumpBufferTimer > 0f && playerData.canJump)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed);
            playerData.isJumping = true; //jump state
        }
        //jump
        
        //extra jump
        if (Input.GetKeyDown(KeyCode.Space) && playerData.canExtraJump && !isGrounded() && extraJumpCounter > 0)
        {
            rb.velocity = new Vector2(rb.velocity.x, jumpSpeed * 1.25f);
            extraJumpCounter--;
        }
        //extra jump

        //running
        if (Input.GetKey(KeyCode.LeftShift) && playerData.canRun)
        {
            speed = speedRunning;
            jumpSpeed = jumpSpeedRunning;
        }
        
        else
        {
            speed = speedDefault;
            jumpSpeed = jumpSpeedDefault;
        }
        //running
    }
    
    private bool isGrounded()
    {
        groundCheck = Physics2D.Raycast(feet.position, Vector2.down, 0.1f);
        
        if (groundCheck.collider != null)
        {
            playerData.isGrounded = groundCheck.collider.CompareTag("Ground");
        }
        
        else
        {
            playerData.isGrounded = false;
        }

        return playerData.isGrounded;
    }
}