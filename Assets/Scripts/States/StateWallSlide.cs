using UnityEngine;

public class StateWallSlide : State
{
    private PlayerMovement player;

    public StateWallSlide(PlayerMovement player)
    {
        this.player = player;
        canReEnter = false;
    }

    public override void Enter()
    {
        player.IsWallSliding = true;
        Debug.Log("Entered Wall Slide State");
    }

    public override void Update()
    {
        player.IsWallSliding = true;

        var vel = player.rb.linearVelocity;

        if (vel.y < 0)
        {
            vel.y = Mathf.Max(vel.y, -player.wallSlideSpeed);
        }

        player.rb.linearVelocity = vel;

        // else
        // {
        //     player.IsWallSliding = false;
        //     //player.StateMachine.ChangeState(player.MoveState); // or whatever
        // }
    }

    public override void Exit()
    {
        player.IsWallSliding = false;
    }
}