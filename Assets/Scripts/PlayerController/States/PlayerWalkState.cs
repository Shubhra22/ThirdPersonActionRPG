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
    public class PlayerWalkState : PlayerMoveState
    {
        #region Variables

        #endregion

        #region Override Methods

        public override void Enter()
        {
            speed = PlayerController.Instance.WalkSpeed;
            PlayerController.Instance.SetMovementSpeed(speed);
        }
        
        #endregion
    }

}
