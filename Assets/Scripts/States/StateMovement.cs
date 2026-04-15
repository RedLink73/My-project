using System;
using UnityEngine;

public class StateMovement : State
{
    private Rigidbody2D _rb;
    private readonly Func<Vector2> _getDir;
    private float _moveSpeed;
    

    public StateMovement(Rigidbody2D rb, Func<Vector2> getDir, float moveSpeed)
    {
        _rb = rb;
        _getDir = getDir;
        _moveSpeed = moveSpeed;
    }

    public override void Enter()
    {
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