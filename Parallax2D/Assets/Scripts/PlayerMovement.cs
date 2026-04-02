using UnityEngine;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class PlayerMovement : MonoBehaviour
{
    public float moveSpeed = 5f;
    public float jumpForce = 7f;
    public float wallJumpForce = 7f;
    public float wallPushForce = 5f;

    private bool isTouchingWall;
    private bool canWallJump;
    private bool touchingLeftWall;
    private bool touchingRightWall;

    private Rigidbody2D rb;
    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        float move = Input.GetAxisRaw("Horizontal") * moveSpeed;
        rb.linearVelocity = new Vector2(move, rb.linearVelocity.y);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // normal jump mech
            if (isGrounded)
            {
                rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);
                isGrounded = false;
                canWallJump = true; //allows player char to jump after leaving ground
            }
            // wall jump mech
            else if (canWallJump)
            {
                if (touchingLeftWall && Input.GetKey(KeyCode.D))
                {
                    rb.linearVelocity = new Vector2(wallPushForce, wallJumpForce);
                    canWallJump = false;
                }
                else if (touchingRightWall && Input.GetKey(KeyCode.A))
                {
                    rb.linearVelocity = new Vector2(-wallPushForce, wallJumpForce);
                    canWallJump = false;
                }
            }
        }
        // slide down wall slowly
        if ((touchingLeftWall || touchingRightWall) && !isGrounded && rb.linearVelocity.y < 0)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, -2f);
        }
    }

    // collision functions (e.g. if grounded -> can jump)
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            canWallJump = true;
        }

        if (collision.gameObject.CompareTag("Wall"))
        {
            foreach (ContactPoint2D contact in collision.contacts)
            {
                if (contact.normal.x > 0)
                {
                    touchingLeftWall = true; // facing wall to left of char
                }
                else if (contact.normal.x < 0)
                {
                    touchingRightWall = true; // facing wall on right of char
                }
            }
        }
    }
    void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Wall"))
        {
            touchingLeftWall = false;
            touchingRightWall = false;
        }
    }
}
