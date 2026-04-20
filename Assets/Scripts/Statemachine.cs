using System.Runtime.CompilerServices;
using States;
using UnityEngine;

public class Statemachine
{
    public State currentState;
    public State _previousState;
    private PlayerMovement _playerMovement;


    public Statemachine(PlayerMovement playerMovement)
    {
        currentState = new StateIdle();
        _playerMovement = playerMovement;
    }

    public void Update()
    {
        currentState.Update();
        Debug.Log(currentState.ToString());
    }

    public void ChangeState(State state)
    {
        if (_playerMovement.restristedStates.Contains(state.ToString()) || !currentState.canTransition)
        {
            return;
        }

        if (state == currentState && !currentState.canReEnter)
        {
            return;
        }

        _previousState = currentState;
        currentState.Exit();
        currentState = state;
        currentState.Enter();
    }
}