using UnityEngine;

public class PlayerStateManager
{
   public PlayerBaseState currentState;

   public PlayerDashState dashState;
   public PlayerDubbeljumpState dubbeljumpState;
   public PlayerJumpState jumpState;
   public PlayerWalljumpState walljumpState;
   public PlayerWallslideStates wallslideStates;
   public PlayerWalkState walkState = new PlayerWalkState();
   public PlayerIdleState idleState = new PlayerIdleState();

   public Vector2 player = new Vector2(0, 1);
   public float health;

    public PlayerStateManager()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
