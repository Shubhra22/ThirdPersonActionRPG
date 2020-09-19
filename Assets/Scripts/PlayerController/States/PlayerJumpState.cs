/*
Author: Shubhra Sarker
Company: JoystickLab
Email: ssuvro22@outlook.com
*/
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JoystickLab
{
    public class PlayerJumpState : IState
    {
        #region Variables

        private bool _grounded;
        
        #endregion
        
        #region Interfaces
        
        public void Enter()
        {
            _grounded = false;
            PlayerController.Instance.Jump();
        }

        public void Update()
        {
            // if (_grounded)
            // {
            //     StateMachine.ChangeState(States.IdleState);
            // }
        }

        public void FixedUpdate()
        {
            float velY = PlayerController.Instance.Rbody.velocity.y;
            Debug.Log(velY + " " +StateMachine._currentState);
            PlayerController.Instance.PlayerAnimator.SetFloat("VelocityY", velY );
            _grounded = PlayerController.Instance.IsOnGround();
        }

        public void Exit()
        {
            //StateMachine.ChangeState(StateMachine._prevState);
            
        }
        
        #endregion
    }

}
