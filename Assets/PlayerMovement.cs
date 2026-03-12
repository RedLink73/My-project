using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;

public class PlayerMovement : MonoBehaviour
{
    public Rigidbody2D rb; 
    public float moveSpeed = 5f;
    bool isFacingRight =true; 
    

    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining; 

    float horizontalMovement;

    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer; 
    bool isGrounded;

    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer; 

    public float baseGravity = 2f;
    public float maxFallSpeed =18f;
    public float fallSpeedMultiplier =2f;

    public float wallSlideSpeed = 2; 
    bool isWallSliding; 

    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer;
    public Vector2 wallJumpPower = new Vector2 (5f, 10f);


    public float dashSpeed = 20f;    
    public float dashDuration = 0.01f;
    public float dashCooldown =0.1f;
    bool isDashing;
    bool canDash = true; 
    TrailRenderer trailRenderer;    

   // yayayayyayayayaya

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();
    }

    // Update is called once per frame
    void Update()
    {


        //SET ANIMATIONS ABOVE IF STATEMENT
        if (isDashing)
        {
            return;
        }

        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();


        if (!isWallJumping)
        {
            Vector2 newVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);
            rb.linearVelocity = newVelocity;

            Flip();
        }
      

    }

    public void ProcessGravity()
    {
        if(rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //Fall increasingly faster
            rb.linearVelocity = new Vector2( rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
        }
        else
        {
            rb.gravityScale = baseGravity;
        }
    }

    private void ProcessWallSlide()
    {
        if (!isGrounded & WallCheck() & horizontalMovement != 0) 
        {
            isWallSliding = true;
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -wallSlideSpeed));
        }
        else
        {
            isWallSliding = false; 
        }
    }

    private void ProcessWallJump()
    {
        if (isWallSliding)
        {
            isWallJumping = false; 
            wallJumpDirection = - transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if(wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;        
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false; 
    }

    public void Move(InputAction.CallbackContext context)
    {
        horizontalMovement = context.ReadValue<Vector2>().x;
    }

     public void Dash(InputAction.CallbackContext context)
    {
        if(context.performed && canDash)
        {
            StartCoroutine(DashCoroutine());
        }
    }

    private IEnumerator DashCoroutine()
    {
        canDash = false;
        isDashing = true; 

        trailRenderer.emitting = true; 

        float dashDirection = isFacingRight ? 1f : -1f;
        rb.linearVelocity = new Vector2 (dashDirection * dashSpeed, rb.linearVelocity.y);

        yield return new WaitForSeconds(dashDuration);
        rb.linearVelocity = new Vector2(0f, rb.linearVelocity.y);

        isDashing = false;
        trailRenderer.emitting = false;

        yield return new WaitForSeconds(dashCooldown);
        canDash = true;




    }

     public void Jump(InputAction.CallbackContext context)
    {   
        if(jumpsRemaining > 0 )
        if (context.performed)
        {
             rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpPower);
             jumpsRemaining--;
        } else if (context.canceled)
        {
             rb.linearVelocity = new Vector2(rb.linearVelocity.x, rb.linearVelocity.y * 0.5f); 
             jumpsRemaining--;
        }

        //WallJump
        if(context.performed && wallJumpTimer > 0f)
        {
            isWallJumping = true;
            rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
            wallJumpTimer = 0;

            if(transform.localScale.x != wallJumpDirection)
            {
                  isFacingRight = !isFacingRight;
                 Vector3 ls = transform.localScale;
                 ls.x *= -1f; 
                 transform.localScale = ls; 
            }
            Invoke(nameof(CancelWallJump), wallJumpTimer + 0.1f);

        }
    }
    
    private void GroundCheck()
    {
        if(Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
        {
            jumpsRemaining = maxJumps;
            isGrounded = true;
        }
        else
        {
            isGrounded = false; 
        }
    }
    private bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }
    private void Flip()
    {
        if(isFacingRight && horizontalMovement < 0 || !isFacingRight && horizontalMovement > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f; 
            transform.localScale = ls; 
        }
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}
