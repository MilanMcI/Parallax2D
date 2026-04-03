using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    //basic left-right movement
    public float moveSpeed = 9f;

    //jump + hold jump with spacebar
    public float jumpForce = 13f;
    public float jumpHoldForce = 8f;
    public float jumpHoldTime = 0.03f;

    //jump allowances
    public float coyoteTime = 0.12f; //allows jump slightly after leaving surface
    public float jumpBufferTime = 0.12f;
    public float jumpCutMultiplier = 0.5f; //cuts off jump faster - for snappier short hops

    //walljump
    public float wallJumpForce = 10f;
    public float wallPushForce = 10f; //pushes char away from wall
    public float wallJumpInputBuffer = 0.12f;
    public float wallGraceTime = 0.12f;
    public float wallJumpLockTime = 0.12f;

    //slide down wall speed
    public float wallSlideSpeed = -2f;

    //gravity/fall speed
    public float extraFallGravity = 45f;
    public float maxFallSpeed = -22f;

    //peak of jump gravity changes(helps make jumps less floaty, particularly at peak of jump)
    public float peakGravityMultiplier = 2.2f;
    public float peakVelocityThreshold = 1.5f;

    private Rigidbody2D rb;

    private bool isGrounded;
    private bool touchingLeftWall;
    private bool touchingRightWall;

    private bool isJumping;
    private float jumpTimeCounter;

    private float coyoteTimer;
    private float jumpBufferTimer;
    private float leftInputTimer;
    private float rightInputTimer;
    private float leftWallGraceTimer;
    private float rightWallGraceTimer;
    private float wallJumpLockTimer;

    // 0 = none, -1 = last wall jump was from left wall, 1 = from right wall
    private int lastWallJumpedFrom = 0;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        HandleTimers();
        HandleMovement();
        HandleJump();
        HandleWallSlide();
        HandleBetterFall();
    }

    void HandleTimers()
    {
        if (Input.GetKeyDown(KeyCode.A))
            leftInputTimer = wallJumpInputBuffer;
        else
            leftInputTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.D))
            rightInputTimer = wallJumpInputBuffer;
        else
            rightInputTimer -= Time.deltaTime;

        if (Input.GetKeyDown(KeyCode.Space))
            jumpBufferTimer = jumpBufferTime;
        else
            jumpBufferTimer -= Time.deltaTime;

        coyoteTimer -= Time.deltaTime;
        leftWallGraceTimer -= Time.deltaTime; //leave from wall on left
        rightWallGraceTimer -= Time.deltaTime; //right
        wallJumpLockTimer -= Time.deltaTime;
    }

    void HandleMovement()
    {
        float move = Input.GetAxisRaw("Horizontal") * moveSpeed;

        // briefly lock horizontal movement after wall jump
        if (wallJumpLockTimer <= 0f)
        {
            rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);
        }
    }

    void HandleJump()
    {
        bool canJumpFromGround = coyoteTimer > 0f;
        bool canJumpFromLeftWall = (touchingLeftWall || leftWallGraceTimer > 0f) && lastWallJumpedFrom != -1;
        bool canJumpFromRightWall = (touchingRightWall || rightWallGraceTimer > 0f) && lastWallJumpedFrom != 1;

        // Ground jump with coyote time + jump buffer
        if (jumpBufferTimer > 0f && canJumpFromGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
            isGrounded = false;
            isJumping = true;
            jumpTimeCounter = jumpHoldTime;
            coyoteTimer = 0f;
            jumpBufferTimer = 0f;

            // Landing / normal jump resets wall usage
            lastWallJumpedFrom = 0;
            return;
        }

        // Wall jump from left wall -> must press D
        if (jumpBufferTimer > 0f && !isGrounded && canJumpFromLeftWall &&
            (Input.GetKey(KeyCode.D) || rightInputTimer > 0f))
        {
            rb.linearVelocity = new Vector2(wallPushForce, wallJumpForce);
            isJumping = false;
            jumpBufferTimer = 0f;
            wallJumpLockTimer = wallJumpLockTime;
            lastWallJumpedFrom = -1;
            return;
        }

        // Wall jump from right wall -> must press A
        if (jumpBufferTimer > 0f && !isGrounded && canJumpFromRightWall &&
            (Input.GetKey(KeyCode.A) || leftInputTimer > 0f))
        {
            rb.linearVelocity = new Vector2(-wallPushForce, wallJumpForce);
            isJumping = false;
            jumpBufferTimer = 0f;
            wallJumpLockTimer = wallJumpLockTime;
            lastWallJumpedFrom = 1;
            return;
        }

        // variable jump height(depends on time spent pressing spacebar)
        if (Input.GetKey(KeyCode.Space) && isJumping && jumpTimeCounter > 0f)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y + jumpHoldForce * Time.deltaTime);
            jumpTimeCounter -= Time.deltaTime;
        }

        // Jump cut for snappier short hops
        if (Input.GetKeyUp(KeyCode.Space))
        {
            isJumping = false;

            if (rb.linearVelocity.y > 0f)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * jumpCutMultiplier);
            }
        }
    }

    void HandleWallSlide()
    {
        bool onWall = touchingLeftWall || touchingRightWall;

        if (!isGrounded && onWall && rb.linearVelocity.y < wallSlideSpeed)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, wallSlideSpeed);
        }
    }

    void HandleBetterFall()
    {
        float yVelocity = rb.linearVelocity.y;

        //falling
        if(yVelocity < 0f)
        {
            float newY = yVelocity - extraFallGravity * Time.deltaTime;

            if (newY < maxFallSpeed)
                newY = maxFallSpeed;

            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);
        }
        else if (Mathf.Abs(yVelocity) < peakVelocityThreshold)
        {
            float newY = yVelocity - (extraFallGravity * peakGravityMultiplier) * Time.deltaTime;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, newY);
        }
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            isJumping = false;
            coyoteTimer = coyoteTime;

            // Reset wall-jump tracking when grounded
            lastWallJumpedFrom = 0;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            UpdateWallContacts(collision);
        }
    }

    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            coyoteTimer = coyoteTime;

            // Reset wall-jump tracking while grounded
            lastWallJumpedFrom = 0;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            UpdateWallContacts(collision);
        }
    }

    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = false;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            touchingLeftWall = false;
            touchingRightWall = false;
        }
    }

    void UpdateWallContacts(Collision2D collision)
    {
        touchingLeftWall = false;
        touchingRightWall = false;

        foreach (ContactPoint2D contact in collision.contacts)
        {
            if (contact.normal.x > 0)
            {
                touchingLeftWall = true;
                leftWallGraceTimer = wallGraceTime;
            }
            else if (contact.normal.x < 0)
            {
                touchingRightWall = true;
                rightWallGraceTimer = wallGraceTime;
            }
        }
    }
}