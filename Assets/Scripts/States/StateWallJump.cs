using System;
using UnityEngine;

public class StateWallJump : State
{
    private Rigidbody2D _rb;
    Func<float> _wallJumpDirection;
    Vector2 _wallJumpPower;
    private float transitionTimer;
    private float maxTransitionTimer;

    private Action _resolveLoc;

    public StateWallJump(Rigidbody2D rb, Func<float> getWallJumpDir, float wallJumpDirection, Vector2 wallJumpPower,
        Action resolveLoc)
    {
        _rb = rb;
        _wallJumpDirection = getWallJumpDir;
        _wallJumpPower = wallJumpPower;
        _resolveLoc = resolveLoc;
        maxTransitionTimer = wallJumpDirection;
    }


    public override void Enter()
    {
        canTransition = false;
        transitionTimer = maxTransitionTimer;

        _rb.linearVelocity = new Vector2(
            _wallJumpDirection() * _wallJumpPower.x,
            _wallJumpPower.y
        );
    }

    public override void Update()
    {
        if (transitionTimer > 0)
        {
            transitionTimer -= Time.deltaTime;
        }
        else
        {
            canTransition = true;
            transitionTimer = maxTransitionTimer;
            _resolveLoc();
        }
    }

    public override void Exit()
    {
    }
}