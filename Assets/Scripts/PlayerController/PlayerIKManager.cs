/*
Author: Shubhra Sarker
Company: JoystickLab
Email: ssuvro22@outlook.com
*/

using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace JoystickLab
{
    [RequireComponent(typeof(Animator))]
    public class PlayerIKManager : MonoBehaviour
    {
        #region Variables
        private Animator _animator;
        private Vector3 _leftFootPos, _rightFootPos, _leftFootIkPos, _rightFootIkPos;
        private Quaternion _leftFootIkRotation, _rightFootIkRotation;
        private float _lastPelisPosY, _lastLeftFootPosY, _lastRightFootPosY;

        [SerializeField]private float heightFromGroundRaycast = 1.14f;
        [SerializeField]private float rayCastDownDistance = 1.5f;
        [SerializeField]private LayerMask environmentLayer;
        [SerializeField]private float pelvisOffset = 0;
        [SerializeField]private float pelvisUpAndDownSpeed = 0.28f;
        [SerializeField]private float feelToIkPositionSpeed = 0.5f;

        private string _rightFootAnimVariableName = "RightFootCurve";
        private string _leftFootAnimVariableName = "LeftFootCurve";


        [SerializeField] private bool enableFeetIk;
        [SerializeField] private bool useProIkFeature;
        [SerializeField] private bool showDebug;

        public bool EnableFeetIk
        {
            get => enableFeetIk;
            set => enableFeetIk = value;
        }

        #endregion
        
        #region MainMethods
        // Start is called before the first frame update
        void Start()
        {
            _animator = GetComponent<Animator>();
        }
        
        void FixedUpdate()
        {
            if (!enableFeetIk || !_animator) return;
            
            AdjustFeet(ref _leftFootPos,HumanBodyBones.LeftFoot);
            AdjustFeet(ref _rightFootPos,HumanBodyBones.RightFoot);
            
            FeetPositionSolver(_leftFootPos,ref _leftFootIkPos,ref _leftFootIkRotation);
            FeetPositionSolver(_rightFootPos,ref _rightFootIkPos,ref _rightFootIkRotation);
            
        }
        #endregion
        
        #region CustomMethods

        private void OnAnimatorIK(int layerIndex)
        {
            // Vector3 leftFootPosition = _animator.GetBoneTransform(HumanBodyBones.LeftFoot).position;
            // Vector3 rightFootPosition = _animator.GetBoneTransform(HumanBodyBones.RightFoot).position;
            //
            // leftFootPosition = CalculateHit(leftFootPosition + Vector3.up, leftFootPosition - Vector3.up * 5);
            // rightFootPosition = CalculateHit(rightFootPosition + Vector3.up, rightFootPosition - Vector3.up * 5);
            //
            //
            // var localPosition = transform.localPosition;
            // localPosition.y = -Mathf.Abs((leftFootPosition.y - rightFootPosition.y) / 2);
            // transform.localPosition = localPosition;
            //
            // _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1f);
            // _animator.SetIKPosition(AvatarIKGoal.LeftFoot,leftFootPosition);
            //
            // _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1f);
            // _animator.SetIKPosition(AvatarIKGoal.RightFoot,rightFootPosition);
            if (!EnableFeetIk || !_animator) return;
            
            /* Three Steps
             1. Move the pelvis
             2. Set the IKpositions and rotation
             3. Move the feet to the IK
             */
            
            MovePelvisHeight();
            _animator.SetIKPositionWeight(AvatarIKGoal.LeftFoot,1);
            _animator.SetIKPositionWeight(AvatarIKGoal.RightFoot,1);
            if (useProIkFeature)
            {
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftFoot, _animator.GetFloat(_leftFootAnimVariableName));
                _animator.SetIKRotationWeight(AvatarIKGoal.RightFoot, _animator.GetFloat(_rightFootAnimVariableName));
            }
            MoveFeetToIkPoint(AvatarIKGoal.LeftFoot,_leftFootIkPos,_leftFootIkRotation,ref _lastLeftFootPosY);
            MoveFeetToIkPoint(AvatarIKGoal.RightFoot,_rightFootIkPos,_rightFootIkRotation,ref _lastRightFootPosY);
        }

        private void MoveFeetToIkPoint(AvatarIKGoal foot, Vector3 positionIkHolder,Quaternion rotationHolder, ref float lastFootPositionY)
        {
            Vector3 targetIkPosition = _animator.GetIKPosition(foot); // get the ik position from the foot
            if (positionIkHolder != Vector3.zero)
            {
                targetIkPosition = transform.InverseTransformPoint(targetIkPosition);
                positionIkHolder = transform.InverseTransformPoint(positionIkHolder);
                float upDown = Mathf.Lerp(lastFootPositionY, positionIkHolder.y, feelToIkPositionSpeed);
                targetIkPosition.y += upDown;
                lastFootPositionY = upDown;
                targetIkPosition = transform.TransformPoint(targetIkPosition);
                _animator.SetIKRotation(foot,rotationHolder);
            }
            _animator.SetIKPosition(foot,targetIkPosition);
            
        }

        
        void MovePelvisHeight()
        {
            if (_leftFootIkPos == Vector3.zero || _rightFootIkPos == Vector3.zero || _lastPelisPosY == 0)
            {
                _lastPelisPosY = _animator.bodyPosition.y;
                return;
            }

            float lOffsetPosition = _leftFootIkPos.y - transform.position.y;
            float rOffsetPosition = _rightFootIkPos.y - transform.position.y;

            float totalOffset = (lOffsetPosition < rOffsetPosition) ? lOffsetPosition : rOffsetPosition;

            Vector3 newPelvisPos = _animator.bodyPosition + Vector3.up * totalOffset;
            newPelvisPos.y = Mathf.Lerp(_lastPelisPosY, newPelvisPos.y, pelvisUpAndDownSpeed);

            _animator.bodyPosition = newPelvisPos;
            _lastPelisPosY = _animator.bodyPosition.y;

        }
        
        // Use Raycast to find the position of the feet
        void FeetPositionSolver(Vector3 fromPosition, ref Vector3 feetIkPositions, ref Quaternion feetIkRotation)
        {
            RaycastHit hit;
            if (showDebug)
            {
                Debug.DrawLine(fromPosition,fromPosition+Vector3.down*(rayCastDownDistance+heightFromGroundRaycast), Color.red);
            }

            if (Physics.Raycast(fromPosition, Vector3.down, out hit, rayCastDownDistance + heightFromGroundRaycast,
                environmentLayer))
            {
                feetIkPositions = fromPosition;
                feetIkPositions.y = hit.point.y + pelvisOffset;
                feetIkRotation = Quaternion.FromToRotation(Vector3.up, hit.normal) * transform.rotation;
                return;
            }
            feetIkPositions = Vector3.zero;
        }

        void AdjustFeet(ref Vector3 feetPosition, HumanBodyBones foot)
        {
            feetPosition = _animator.GetBoneTransform(foot).position;
            feetPosition.y = transform.position.y + heightFromGroundRaycast;
            
        }
        
        #endregion
    }
}