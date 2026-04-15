using System.Runtime.CompilerServices;
using States;
using UnityEngine;

public class Statemachine
{
    private State _currentState;
    private PlayerMovement _playerMovement;
    
    public Statemachine(PlayerMovement playerMovement)
    {
        _currentState = new StateIdle();
        _playerMovement = playerMovement;
    }

    public void Update()
    {
        _currentState.Update();
        Debug.Log(_currentState.ToString());
    }

    public void ChangeState(State state)
    {
        if (_playerMovement.restristedStates.Contains(state.ToString()) || !state.canTransition && (state == _currentState && !state.canReEnter))
        {
            return;
        }

        if (state == _currentState && !state.canReEnter)
        {
            
        }
        
        _currentState.Exit();
        _currentState = state;
        _currentState.Enter();
    }

}