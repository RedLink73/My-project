using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerBaseState
{
    public Rigidbody2D rb;
    public float moveSpeed = 5f;

    float horizontalMovement;



    public override void Enterstate(Rigidbody2D ridgidbody)
    {
        if (rb == null)
            rb = ridgidbody;
        Debug.Log("Welcome from the Walk state!");
    }
    public override void UpdateState()
    {

        rb.linearVelocity = new Vector2(horizontalMovement * moveSpeed, rb.linearVelocity.y);

        Debug.Log("Hello from the Walk state!");
    }
    public override void OnExit()
    {

    }
}
