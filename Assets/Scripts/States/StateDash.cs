using UnityEngine;
using System;
using UnityEngine.UIElements;

public class StateDash : State
{
    private Rigidbody2D _rb;
    private readonly Func<Vector2> _getDir;
    private float _dashSpeed;
    private TrailRenderer _trailRenderer;

    private float _dashDuration;
    private float _currentDashDuration;
    public Action A_DashEnded;

    
    public StateDash(Rigidbody2D rb, Func<Vector2> getDir, float dashSpeed, TrailRenderer trailRenderer,
        float dashDuration)
    {
        _rb = rb;
        _getDir = getDir;
        _dashSpeed = dashSpeed;
        _trailRenderer = trailRenderer;
        _dashDuration = dashDuration;
    }

    public override void Enter()
    {
        _trailRenderer.emitting = true;
        canTransition = false;
    }

    public override void Update()
    {
        if (_currentDashDuration >= 0f)
        {
            _currentDashDuration -= Time.deltaTime;
            _rb.linearVelocity = new Vector2(_getDir().x * _dashSpeed, _rb.linearVelocity.y);
        }
        else
        {
            canTransition = true;
            A_DashEnded?.Invoke();
        }
    }

    public override void Exit()
    {
        _rb.linearVelocity = new Vector2(0 * _dashSpeed, _rb.linearVelocity.y);
        _trailRenderer.emitting = false;
        _currentDashDuration = _dashDuration;
    }
}