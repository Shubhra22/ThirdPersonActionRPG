/*
Author: Shubhra Sarker
Company: JoystickLab
Email: ssuvro22@outlook.com
*/

using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using Debug = System.Diagnostics.Debug;

namespace JoystickLab
{
    public class PlayerAiming : MonoBehaviour
    {
        #region Variables

        [SerializeField] private float turnSpeed = 10;
        
        #endregion
        
        #region MainMethods
        // Start is called before the first frame update
        void Start()
        {
            
        }
    
        // Update is called once per frame
        void FixedUpdate()
        {
            float camRot = Camera.main.transform.rotation.eulerAngles.y;
            transform.rotation = Quaternion.Slerp(transform.rotation,Quaternion.Euler(0,camRot,0), turnSpeed*Time.deltaTime);
        }
        #endregion
        
        #region CustomMethods
                
        #endregion
    }
}