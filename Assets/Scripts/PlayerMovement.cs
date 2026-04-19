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

    public float jumpPower = 10f;
    public int maxJumps = 2;
    int jumpsRemaining;

    float horizontalMovement;
    private Vector2 _dir;

    public Transform groundCheckPos;
    public Vector2 groundCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask groundLayer;
    bool isGrounded;

    public Transform wallCheckPos;
    public Vector2 wallCheckSize = new Vector2(0.5f, 0.05f);
    public LayerMask wallLayer;

    public float baseGravity = 2f;
    public float maxFallSpeed = 18f;
    public float fallSpeedMultiplier = 2f;

    public float wallSlideSpeed = 2;
    bool isWallSliding;

    bool isWallJumping;
    float wallJumpDirection;
    float wallJumpTime = 0.5f;
    float wallJumpTimer = 0f;
    public Vector2 wallJumpPower = new Vector2(5f, 5f);

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

    #endregion

    #region Abilitis

    public List<AbilitieEnums> abilities = new();

    #endregion

    #region DEBUG
    [TextArea]
    public string DEBUG_STRING;
    
    

    #endregion

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        trailRenderer = GetComponent<TrailRenderer>();

        statemachine = new Statemachine(this);
        stateIdle = new StateIdle();
        stateMovement = new StateMovement(rb, GetDir, moveSpeed);
        stateJump = new StateJump(rb, jumpPower, GetDir, moveSpeed);
        stateWallJump = new StateWallJump(rb, GetDir, wallJumpPower);
        stateDash = new StateDash(rb, GetDir, dashSpeed, trailRenderer, dashDuration, ExitIsDashing);
        ((StateDash)stateDash).A_DashEnded += HandleDuration;
    }

   
    // Update is called once per frame
    void Update()
    {
        //SET ANIMATIONS ABOVE IF STATEMENT

        GroundCheck();
        ProcessGravity();
        ProcessWallSlide();
        ProcessWallJump();
        UpdateDash();

        statemachine.Update();
        DEBUG_STRING = "Current State: " + statemachine._currentState + "\n" + this;
        DEBUG_STRING = "Current State: " + statemachine._currentState.canTransition + "\n" + this;
    }

    
    public void ExitIsDashing()
    {
        isDashing = false;
    }
    
    Vector2 GetDir()
    {
        return _dir;
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
        if (WallCheck() && context.performed && wallJumpTimer <= 0f && !isGrounded)
        {
            Debug.Log("We are On the wall");
            statemachine.ChangeState(stateWallJump);
            return;
        }

        //Normal Jump
        if (jumpsRemaining > 0 && context.performed)
        {
            statemachine.ChangeState(stateJump);
            jumpsRemaining--;
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
        if (Physics2D.OverlapBox(groundCheckPos.position, groundCheckSize, 0, groundLayer))
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
        if (rb.linearVelocity.y < 0)
        {
            rb.gravityScale = baseGravity * fallSpeedMultiplier; //Fall increasingly faster
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Mathf.Max(rb.linearVelocity.y, -maxFallSpeed));
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
            wallJumpDirection = -transform.localScale.x;
            wallJumpTimer = wallJumpTime;

            CancelInvoke(nameof(CancelWallJump));
        }
        else if (wallJumpTimer > 0f)
        {
            wallJumpTimer -= Time.deltaTime;
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