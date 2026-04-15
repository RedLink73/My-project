using System;
using UnityEngine;

public class StateWallJump : State
{
    private Rigidbody2D _rb;
    float _wallJumpDirection;
    Vector2 _wallJumpPower;
    private readonly Func<Vector2> _getDir;
    Vector2 _wallJumpDir;

    public StateWallJump(Rigidbody2D rb, Func<Vector2> getDir, Vector2 wallJumpPower)
    {
        _rb = rb;
        _getDir = getDir;
        _wallJumpPower = wallJumpPower;
    }


    public override void Enter()
    {
        _wallJumpDir = _getDir();
        _rb.linearVelocity = new Vector2(_wallJumpDir.x * _wallJumpPower.x, _wallJumpPower.y);
        
        // if (transform.localScale.x != wallJumpDirection)
        // {
        //     isFacingRight = !isFacingRight;
        //     Vector3 ls = transform.localScale;
        //     ls.x *= -1f;
        //     transform.localScale = ls;
        // }
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