/*
Author: Shubhra Sarker
Company: JoystickLab
Email: ssuvro22@outlook.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;

namespace JoystickLab
{
    public class GeckoBehaviour : MonoBehaviour
    {
        #region Variables
        // Where to look 
        [SerializeField] private Transform target;
        // neck reference
        [SerializeField] private Transform headBone;
        // Slerp Speed. Smoothly moves head
        [SerializeField] private float headRotationSpeed = 5;
        //Max rotation angle in degree
        [SerializeField] private float maxRotation = 90;
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

        private void LateUpdate()
        {
            UpdateHeadPosition(headRotationSpeed);
        }

        #endregion
        
        #region CustomMethods

        private void UpdateHeadPosition(float speed)
        {
            
            // First We calculate the direction vector in world space.
            // We can not use localPosition directly as the local position is basically calculated
            // on corresponding to the immidiate parent of that transform
            Vector3 headToTargetWorld = target.position - headBone.position;
            
            // We then calculate the local direction by using the InverseTransform
            //Vector3 headToTargetLocal = headBone.InverseTransformDirection(headToTargetWorld)
            Debug.DrawRay(headBone.position,headToTargetWorld*10,Color.green);

            float angle = Vector3.SignedAngle(transform.forward, headToTargetWorld.normalized, headBone.up);
            float adjustedRotation = Mathf.Clamp(angle, -maxRotation, maxRotation);

            Vector3 clampedForward = Quaternion.AngleAxis(adjustedRotation, Vector3.up) * transform.forward;
            Quaternion targetRotation = Quaternion.LookRotation(clampedForward, headBone.up);
            
            
            headBone.rotation =
                Quaternion.Slerp(headBone.rotation, targetRotation, 1 - Mathf.Exp(-speed * Time.deltaTime));
        }
        #endregion
    }
}