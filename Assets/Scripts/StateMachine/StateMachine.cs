using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ProjectDynamax.GameLogic.FSM
{
    public class StateMachine
    {
        private IState _previousState;
        private IState _currentState;

        public void ChangeState(IState newState)
        {
            _currentState?.Exit();
            _previousState = _currentState;
            _currentState = newState;
            _currentState.Enter();
        }

        public void ExecuteStateUpdate()
        {
            _currentState?.Execute();
        }

        public void SwitchToPreviousState()
        {
            _currentState.Exit();
            var tempState = _previousState;
            _previousState = _currentState;
            _currentState = tempState;
            _currentState.Enter();
        }
    }
}