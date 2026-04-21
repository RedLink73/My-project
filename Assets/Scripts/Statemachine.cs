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
    }

    public bool ChangeState(State state)
    {
        if (_playerMovement.restristedStates.Contains(state.ToString()) || !currentState.canTransition)
        {
            return false;
        }

        if (state == currentState && !currentState.canReEnter)
        {
            return false;
        }

        _previousState = currentState;
        currentState.Exit();
        currentState = state;
        currentState.Enter();
        return true;
    }
}