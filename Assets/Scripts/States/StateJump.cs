using System;
using UnityEngine;

public class StateJump : State
{
    private Rigidbody2D _rb;
    private float _jumpPower;
    private readonly Func<Vector2> _getDir;
    private  readonly float _moveSpeed;
    
    public StateJump( Rigidbody2D rb, float jumpPower, Func<Vector2> getDir, float moveSpeed)
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
        _rb.linearVelocity = new Vector2(dir.x * _moveSpeed, _rb.linearVelocity.y);
    }

    public override void Exit()
    {
        _rb.linearVelocity = new Vector2(0 * _moveSpeed, _rb.linearVelocity.y);
    }
    
    
}