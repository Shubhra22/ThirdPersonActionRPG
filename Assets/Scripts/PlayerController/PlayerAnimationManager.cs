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
    [RequireComponent(typeof(Animator))]
    public class PlayerAnimationManager : MonoBehaviour
    {
        #region Variables
        
        #endregion
        
        #region MainMethods
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void Update()
        {
            
        }
        #endregion
        
        #region CustomMethods

        public void HandleFootEvent()
        {
            SoundManager.Instance.SetFootSound();
        }

        public void PauseIKAndRootMotion()
        {
            
        }
        #endregion
    }
}