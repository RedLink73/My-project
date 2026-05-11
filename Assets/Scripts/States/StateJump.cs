using System;
using UnityEngine;

public class StateJump : State
{
    private Rigidbody2D _rb;
    private float _jumpPower;
    private readonly Func<Vector2> _getDir;
    private readonly float _moveSpeed;

    float baseGravity = 1.5f;
    float fallMultiplier = 1.5f;
    
    public StateJump(Rigidbody2D rb, float jumpPower, Func<Vector2> getDir, float moveSpeed)
    {
        _rb = rb;
        _jumpPower = jumpPower;
        _getDir = getDir;
        _moveSpeed = moveSpeed;
        canReEnter = true;
    }

    public override void Enter()
    {
        _rb.linearVelocity = new Vector2(_rb.linearVelocity.x, _jumpPower);
    }

    public override void Update()
    {
        Vector2 dir = _getDir();
        // Better jump feel

        if (_rb.linearVelocity.y < -0.1f)
        {
            _rb.gravityScale = baseGravity * fallMultiplier;
        }
        else
        {
            _rb.gravityScale = baseGravity;
        }
        // else
        // {
        //     _rb.gravityScale = 1.5f; // Floaty apex
        // }
        //
        
        if (_rb.linearVelocity.y < -0.1f)
        {
            _rb.gravityScale = 4.5f;
        }
        else
        {
            _rb.gravityScale = 3f;
        }
        
        // if (dir.y < 0)
        // {
        //     _rb.gravityScale = 5;
        // }
        // else
        // {
        //     _rb.gravityScale = 3;
        // }


        _rb.linearVelocity = new Vector2(dir.x * _moveSpeed, _rb.linearVelocity.y);
    }

    public override void Exit()
    {
        _rb.linearVelocity = new Vector2(0 * _moveSpeed, _rb.linearVelocity.y);
    }
}