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
    public class PlayerMoveState : IState
    {
        #region Variables
        protected float speed = 1;
        #endregion

        #region InterfaceMethods

        public virtual void Enter()
        {
            PlayerController.Instance.SetMovementSpeed(speed);
        }

        public virtual void Update()
        {
            throw new System.NotImplementedException();
        }

        public virtual void FixedUpdate()
        {
            PlayerController.Instance.MovementController();
        }

        public virtual void Exit()
        {
            speed = 1;
        }
        
        #endregion
    }

}
