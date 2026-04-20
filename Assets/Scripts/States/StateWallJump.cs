using System;
using UnityEngine;

public class StateWallJump : State
{
    private Rigidbody2D _rb;
    float _wallJumpDirection;
    Vector2 _wallJumpPower;

    public StateWallJump(Rigidbody2D rb, Func<float> getWallJumpDir, Vector2 wallJumpPower)
    {
        _rb = rb;
        _wallJumpDirection = getWallJumpDir();
        _wallJumpPower = wallJumpPower;
    }


    public override void Enter()
    {
        // _wallJumpDir = _getDir();
        // _rb.linearVelocity = new Vector2(_wallJumpDir.x * _wallJumpPower.x, _wallJumpPower.y);
        // Debug.unityLogger.Log("Entered StateWallJump");

        // if (transform.localScale.x != wallJumpDirection)
        // {
        //     isFacingRight = !isFacingRight;
        //     Vector3 ls = transform.localScale;
        //     ls.x *= -1f;
        //     transform.localScale = ls;
        // }

        Debug.Log("Entering State");

        _rb.linearVelocity = new Vector2(
            _wallJumpDirection * _wallJumpPower.x,
            _wallJumpPower.y
        );
    }

    public override void Update()
    {
    }

    public override void Exit()
    {
    }

    // isWallJumping = true;
    // rb.linearVelocity = new Vector2(wallJumpDirection * wallJumpPower.x, wallJumpPower.y);
    // wallJumpTimer = 0;
    //
    // if (transform.localScale.x != wallJumpDirection)
    // {
    //     isFacingRight = !isFacingRight;
    //     Vector3 ls = transform.localScale;
    //     ls.x *= -1f;
    //     transform.localScale = ls;
    // }
    //
    // Invoke(nameof(CancelWallJump), wallJumpTimer + 0.1f);
}