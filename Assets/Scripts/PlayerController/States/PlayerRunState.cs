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
    public class PlayerRunState : PlayerMoveState
    {
        #region Variables

        #endregion

        #region OverRidden Methods
        
        public override void Enter()
        {
            speed = speed+PlayerController.Instance.RunSpeed * PlayerInputController.Instance.DoRun;
            PlayerController.Instance.SetMovementSpeed(speed);
        }
        
        #endregion
    }

}
