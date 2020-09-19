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
    public class PlayerIdleState : PlayerMoveState
    {
        #region Variables

        #endregion

        public override void Enter()
        {
            speed = 0;
            PlayerController.Instance.SetMovementSpeed(speed);
        }
    }

}
