/*
Author: Shubhra Sarker
Company: JoystickLab
Email: ssuvro22@outlook.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Interactions;

namespace JoystickLab
{
    public class StateMachine
    {
        #region Variables
        
        public static IState _currentState;
        public static IState _prevState;
    
        #endregion


        public static void ChangeState(IState state)
        {
            _prevState = _currentState;
             _currentState?.Exit();
            _currentState = state;
            _currentState.Enter();
        }

        public static void Update()
        {
            _currentState.Update();
        }

        public static void FixedUpdate()
        {
            //Debug.Log(_currentState);
            _currentState.FixedUpdate();
        }

        public static void Exit()
        {
            _currentState.Exit();
        }
    }

}
