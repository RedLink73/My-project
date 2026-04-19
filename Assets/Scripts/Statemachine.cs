using System.Runtime.CompilerServices;
using States;
using UnityEngine;

public class Statemachine
{
    public State _currentState;
    public State _previousState;
    private PlayerMovement _playerMovement;
    

    public Statemachine(PlayerMovement playerMovement)
    {
        _currentState = new StateIdle();
        _playerMovement = playerMovement;
    }

    public void Update()
    {
        _currentState.Update();
        Debug.Log(_currentState.canTransition.ToString());
    }

    public void ChangeState(State state)
    {
        if (_playerMovement.restristedStates.Contains(state.ToString()) || !_currentState.canTransition)
        {
            return;
        }

        if (state == _currentState && !_currentState.canReEnter)
        {
            return;
        }

        _previousState = _currentState;
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }
}