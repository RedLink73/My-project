using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using States;
using UnityEngine.Rendering;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class PlayerMovement : MonoBehaviour
{
    public List<string> restristedStates = new List<string>();

    public Rigidbody2D rb;
    public float moveSpeed = 5f;
    bool isFacingRight = true;

    public float jumpPower = 15f;
    public int maxJumps = 2;
    public int jumpsRemaining;
    public float jumpTimeThreashold = 0.5f;
    public float jumpHangGravityMulti = 0.2f;


    private Vector2 _dir;

    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    public bool IsGrounded;
    public bool wasGrounded;

    public float baseGravity = 3f;
    public float currentGravity = 0;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 1.8f;
    public float fallGravityAcceleration = 6.0f;
    public float gravityResetSpeed = 12.0f;

    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;
    public float wallSlideSpeed = 0.2f;
    public bool IsWallSliding;
    bool isWallJumping;
    public float wallJumpDirection;
    public float wallJumpTime = 0.2f;
    public float wallJumpTimer = 0f;
    public Vector2 wallJumpPower = new Vector2(200f, 200f);
    public float wallJumpDuration = 0.15f;

    public float dashSpeed = 4f;
    public float dashDuration = 0.5f;
    public float dashCooldown = 5f;
    public float currentDashCooldown = 0f;

    public bool isDashing = false;
    public bool canDash = true;

    TrailRenderer trailRenderer;
    //Ichanged something oooohhh
    // yayayayyayayayaya

    #region StateMachine

    public Statemachine statemachine;
    public State stateMovement;
    public State stateIdle;
    public State stateJump;
    public State stateWallJump;
    public State stateDash;
    public State stateWallSlide;

    #endregion

    #region Abilitis

    public List<AbilitieEnums> abilities = new();

    #endregion

    #region DEBUG

    [TextArea] public string DEBUG_STRING;

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        statemachine = new Statemachine(this);
        stateIdle = new StateIdle();
        stateMovement = new StateMovement(rb, GetDir, moveSpeed);
        stateJump = new StateJump(rb, jumpPower, GetDir, moveSpeed);
        stateWallJump = new StateWallJump(rb, GetWallJumpDir, wallJumpDuration, wallJumpPower, ResolveLocomotionState);
        stateDash = new StateDash(rb, GetDir, dashSpeed, trailRenderer, dashDuration, ExitIsDashing);
        stateWallSlide = new StateWallSlide(this);
        ((StateDash)stateDash).A_DashEnded += HandleDuration;
    }


    // Update is called once per frame
    void Update()
    {
        //SET ANIMATIONS ABOVE IF STATEMENT

        GroundCheck();
        ProcessGravity();
        //ProcessWallSlide();
        HandleStateTransitionForConditionBasedStates();
        //ProcessWallJump();
        UpdateDash();

        statemachine.Update();
        DEBUG_STRING = "Current State: " + statemachine.currentState.ToString() + "\n" +
                       "Can S: " + statemachine.currentState.canTransition + "\n";
        //DEBUG_STRING = "Y VEl: " + rb.linearVelocityY + "\n";
    }

    public void ExitIsDashing()
    {
        isDashing = false;
    }

    Vector2 GetDir()
    {
        return _dir;
    }

    float GetWallJumpDir()
    {
        return wallJumpDirection;
    }


    public void Move(InputAction.CallbackContext context)
    {
        _dir = context.ReadValue<Vector2>();

        if (_dir == Vector2.zero)
        {
            statemachine.ChangeState(stateIdle);
            return;
        }

        if (isWallJumping)
        {
            return;
        }

        statemachine.ChangeState(stateMovement);
        Flip();
    }


    public void Jump(InputAction.CallbackContext context)
    {
        //WallJump
        if (WallCheck() && context.performed && wallJumpTimer <= 0f && !IsGrounded)
        {
            wallJumpTimer = wallJumpTime;
            statemachine.ChangeState(stateWallJump);
            return;
        }

        //Normal Jump
        if (jumpsRemaining > 0 && context.performed)
        {
            if (statemachine.ChangeState(stateJump))
            {
                jumpsRemaining--;
                IsGrounded = false; // we just jumped
                wasGrounded = false; // prevent fake landing-edge 
            }
        }

        if (context.performed && rb.linearVelocity.y > 0)
        {
            rb.linearVelocity = new Vector2(
                rb.linearVelocity.x,
                rb.linearVelocity.y * 0.5f
            );
        }
    }


    public void UpdateDash()
    {
        // Always decrement cooldown if dash is not available
        if (!canDash)
        {
            if (currentDashCooldown > 0f)
            {
                currentDashCooldown -= Time.deltaTime;
            }
            else
            {
                HandleCooldown();
            }
        }
    }

    public void HandleCooldown()
    {
        // Only reset dash when cooldown is over and dash ability is still present
        if (currentDashCooldown <= 0f && abilities.Contains(AbilitieEnums.Dash))
        {
            canDash = true;
            abilities.Remove(AbilitieEnums.Dash);
        }
    }

    public void HandleDuration()
    {
        Debug.Log("Duration Ended");
        isDashing = false;
        ResolveLocomotionState();
    }

    public void Dash(InputAction.CallbackContext context)
    {
        if (context.performed && canDash && !isDashing)
        {
            canDash = false;
            isDashing = true;
            currentDashCooldown = dashCooldown;
            statemachine.ChangeState(stateDash);
            if (!abilities.Contains(AbilitieEnums.Dash))
            {
                abilities.Add(AbilitieEnums.Dash);
            }
        }
    }

    private void ResolveLocomotionState()
    {
        Vector2 moveInput = _dir;

        if (moveInput.sqrMagnitude > 0.01f)
        {
            statemachine.ChangeState(stateMovement);
        }
        else
        {
            statemachine.ChangeState(stateIdle);
        }
    }

    private void GroundCheck()
    {
        bool groundedNow = Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer);
        IsGrounded = groundedNow;

        // Reset only on landing edge
        if (!wasGrounded && groundedNow && rb.linearVelocity.y <= 0.1f)
        {
            jumpsRemaining = maxJumps;
            rb.linearVelocity = Vector2.zero;
        }

        wasGrounded = groundedNow;
    }

    public bool WallCheck()
    {
        return Physics2D.OverlapBox(wallCheckPos.position, wallCheckSize, 0, wallLayer);
    }

    private void Flip()
    {
        if (isFacingRight && _dir.x < 0 || !isFacingRight && _dir.x > 0)
        {
            isFacingRight = !isFacingRight;
            Vector3 ls = transform.localScale;
            ls.x *= -1f;
            transform.localScale = ls;
        }
    }

    public void ProcessGravity()
    {
        // if (statemachine.currentState == stateJump && Mathf.Abs(rb.linearVelocity.y) < jumpTimeThreashold)
        // {
        //     SetGravityScale(baseGravity * jumpHangGravityMulti);
        // }
        // currentGravity = baseGravity;
        //
        // if (rb.linearVelocity.y < -0.1f)
        // {
        //     float targetGravity = baseGravity * fallSpeedMultiplier;
        //
        //     currentGravity = Mathf.Lerp(
        //         currentGravity,
        //         targetGravity,
        //         4f * Time.deltaTime
        //     );
        //
        //     rb.gravityScale = currentGravity;
        //
        //     rb.linearVelocity = new Vector2(
        //         rb.linearVelocity.x,
        //         Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)
        //     );
        // }
        // else
        // {
        //     currentGravity = Mathf.Lerp(
        //         currentGravity,
        //         baseGravity,
        //         10f * Time.deltaTime
        //     );
        //
        //     rb.gravityScale = currentGravity;
        // }

        if (rb.linearVelocity.y < -0.1f)
        {
            // Increase gravity quickly once descending
            float targetGravity = baseGravity * fallSpeedMultiplier;

            currentGravity = Mathf.Lerp(
                currentGravity,
                targetGravity,
                fallGravityAcceleration * Time.deltaTime
            );
        }
        else
        {
            // Keep jump apex light and floaty
            currentGravity = Mathf.Lerp(
                currentGravity,
                baseGravity,
                gravityResetSpeed * Time.deltaTime
            );
        }

        rb.gravityScale = currentGravity;

        // Clamp terminal velocity
        rb.linearVelocity = new Vector2(
            rb.linearVelocity.x,
            Mathf.Max(rb.linearVelocity.y, -maxFallSpeed)
        );
    }

    public void HandleStateTransitionForConditionBasedStates()
    {
        ProcessWallSlide();
        ProcessWallJump();
    }

    private void ProcessWallSlide()
    {
        if (!IsGrounded && WallCheck() && rb.linearVelocityY != 0)
        {
            statemachine.ChangeState(stateWallSlide);
            return;
        }

        // Exit wall slide
        if (statemachine.currentState == stateWallSlide)
        {
            ResolveLocomotionState();
        }
    }

    private void ProcessWallJump()
    {
        if (IsWallSliding)
        {
            wallJumpDirection = -transform.localScale.x;
        }

        if (WallCheck())
        {
            if (wallJumpTimer >= 0f)
            {
                wallJumpTimer -= Time.deltaTime;
            }
        }
    }

    private void CancelWallJump()
    {
        isWallJumping = false;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(groundCheckPos.position, groundCheckSize);

        Gizmos.color = Color.white;
        Gizmos.DrawWireCube(wallCheckPos.position, wallCheckSize);
    }
}

public enum AbilitieEnums
{
    Dash
}