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

        // _rb.linearVelocity = new Vector2(
        //     _wallJumpDirection() * _wallJumpPower.x,
        //     _wallJumpPower.y
        // );
        //
        Execute();
    }

    public void Execute()
    {
        Vector2 velocity = _rb.linearVelocity;
        // Optional: kill upward momentum if you want consistency
        velocity.y = Mathf.Min(velocity.y, 0f);

        _rb.linearVelocity = velocity;

        if (Mathf.Sign(_rb.linearVelocity.x) != Mathf.Sign(_wallJumpPower.x))
            _wallJumpPower.x -= _rb.linearVelocity.x;

        if (_rb.linearVelocity.y <
            0) //checks whether player is falling, if so we subtract the velocity.y (counteracting force of gravity). This ensures the player always reaches our desired jump force or greater
            _wallJumpPower.y -= _rb.linearVelocity.y;

        //Unlike in the run we want to use the Impulse mode.
        //The default mode will apply are force instantly ignoring mass
        Vector2 JumpPower = new Vector2(_wallJumpDirection() * _wallJumpPower.x, _wallJumpPower.y);
        _rb.AddForce(JumpPower, ForceMode2D.Impulse);
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