using System;
using System.Collections;
using System.Collections.Generic;
using System.Timers;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Interactions;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace JoystickLab
{
    [RequireComponent(typeof(PlayerInputController))]
    [RequireComponent(typeof(Animator))]
    public class PlayerController : Manager<PlayerController>
    {
        private PlayerInputController _inputController;
        private Animator _animator;
        
        [Header("Movement Setting")]
        [SerializeField] private float walkSpeed = 1;
        [SerializeField] private float runSpeed = 2;
        [SerializeField] private float sprintSpeed = 4;
        [SerializeField] private float crouchSpeed = 1;
        [SerializeField] private float proneSpeed = 1;
        
        [Header("Jump Setting")]
        [SerializeField] private float jumpForceUp = 4;
        [SerializeField] private float jumpForceForward = 1;
        [SerializeField] private float vaultForceUp = 2;
        [SerializeField] private float vaultForceForward = 1;

        [Header("Roll Setting")]
        [SerializeField] private float rollDistance = 5;
        [Range(0.2f,1f)]
        [SerializeField] private float rollLerp = 0.2f;


        [Header("Ground Setting")]
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundCheckSphereRadius = 1;
        [SerializeField] private float maxDistanceToFall = 3;
        
        
        
        private float _movementSpeed = 1;
        private float _finalSpeedX;
        private float _finalSpeedY;
        
        private bool _grounded;
        private bool _canVault;
        private bool _canRoll;
        
        private Rigidbody _rbody;
        
        private float _playerHeight;
        private float _distanceToGround;
        private float _distanceFromWall;
        private bool _canMoveForward = true;
        
        
        public Transform matchTargetPos;
        
        private PlayerIKManager _playerIk;

        public Rigidbody Rbody => GetComponent<Rigidbody>();
        public Animator PlayerAnimator => GetComponent<Animator>();
        public PlayerIKManager PlayerIk => GetComponent<PlayerIKManager>();

        public float WalkSpeed => walkSpeed;
        public float RunSpeed => runSpeed;
        public float SprintSpeed => sprintSpeed;
        public float CrouchSpeed => crouchSpeed;
        
        // Start is called before the first frame update
        void Start()
        {
            _inputController = GetComponent<PlayerInputController>();
            _animator = GetComponent<Animator>();
            _playerIk = GetComponent<PlayerIKManager>();
            _playerHeight = GetComponent<CapsuleCollider>().height;

            StateMachine.ChangeState(States.IdleState);
        }
        

        // Update is called once per frame
        void FixedUpdate()
        {
            StateMachine.FixedUpdate();
        }
        
        #region CustomMethods

        public void MovementController()
        {
            var movementMag = Mathf.Clamp(_inputController.Movement.sqrMagnitude, 0, 1);
            if (movementMag < 0.1f)
            {
                StateMachine.ChangeState(States.IdleState);
            }
            _finalSpeedX = Mathf.Lerp(_finalSpeedX, _movementSpeed * movementMag, 4 * Time.deltaTime);
            _finalSpeedY = Mathf.Lerp(_finalSpeedY, _movementSpeed * movementMag * _inputController.Movement.y,
                4 * Time.deltaTime);

            _animator.SetFloat("InputX", _finalSpeedX * _inputController.Movement.x);
            _animator.SetFloat("InputY", _finalSpeedY);
            //_animator.SetFloat("VelocityY", Rbody.velocity.y);
        }

        public void SetMovementSpeed(float value)
        {
            _movementSpeed = value;
        }
        
        private void SetMovementType()
        {
            _canMoveForward = CanMoveForward(out _canVault, out _distanceFromWall, 2);
           // print(_canVault+" "+_distanceFromWall);
            //_inputController.MovementType = _canMoveForward? PlayerInputController.MovementTypeEnum.Run : PlayerInputController.MovementTypeEnum.Idle;
            
            switch (_inputController.MovementType)
            {
                case PlayerInputController.MovementTypeEnum.Idle: 
                    _movementSpeed = 0;
                    break;
                
                case PlayerInputController.MovementTypeEnum.Walk:
                    _movementSpeed = WalkSpeed;
                    break;
                case PlayerInputController.MovementTypeEnum.Run:
                    if(!_canMoveForward) return;
                    _movementSpeed = WalkSpeed + runSpeed * _inputController.DoRun;
                    break;
                case PlayerInputController.MovementTypeEnum.Sprint:
                    _movementSpeed = WalkSpeed + sprintSpeed * _inputController.DoRun;
                    break;
                case PlayerInputController.MovementTypeEnum.Crouching:
                    if(!_canMoveForward) return;
                    _animator.SetBool("Crouch", _inputController.DoCrouch);
                    _movementSpeed = crouchSpeed;// * _inputController.DoCrouch;
                    break;
                case PlayerInputController.MovementTypeEnum.Proning:
                    _movementSpeed = proneSpeed * _inputController.DoProne;
                    break;
                case PlayerInputController.MovementTypeEnum.Jump:
                    if(!_canMoveForward) return;
                    //if (_inputController.DoJump)
                    {
                        Jump();
                    }
                    break;
                case PlayerInputController.MovementTypeEnum.Roll:
                    Roll();
                    break;
                case PlayerInputController.MovementTypeEnum.Fall:
                    Fall();
                    break;
                case PlayerInputController.MovementTypeEnum.Action:
                    Vault();
                    break;
                case PlayerInputController.MovementTypeEnum.Default:
                    _movementSpeed = WalkSpeed;
                    ResetPlayerToDafault();
                    break;
            }
        }
        
        private void Vault()
        {
            if (_canVault && _grounded)
            {
                _animator.applyRootMotion = false;
                _playerIk.EnableFeetIk = false;
                _grounded = false;
                _canVault = false;
                _animator.SetTrigger("Vault");
                MatchTarget(matchTargetPos.position,matchTargetPos.rotation,AvatarTarget.LeftHand,new MatchTargetWeightMask(Vector3.one, 0),0.226f,0.356f);
                Vector3 jumpForce = Vector3.up * vaultForceUp + transform.forward * vaultForceForward;
                Rbody.velocity = jumpForce;
                //_rbody.DOMove(transform.position+ transform.forward * 3, 1);
                
            }
        }

        public void Jump()
        {
            _playerIk.EnableFeetIk = false;
            _animator.applyRootMotion = false;
            _animator.SetTrigger("Jump");
            Vector3 jumpForce = Vector3.up * jumpForceUp + transform.forward * jumpForceForward;
            Rbody.velocity = jumpForce;
        }
        
        private void Fall()
        {
            _playerIk.EnableFeetIk = false;
            _animator.applyRootMotion = false;
            _animator.SetFloat("Height",_distanceToGround);
        }

        private void Roll()
        {
            if (_canRoll)
            {
                _playerIk.EnableFeetIk = false;
                _animator.applyRootMotion = false;
               // transform.DOMove(transform.position+transform.forward*rollDistance, rollLerp);
                
                _animator.SetTrigger("Roll");
                _canRoll = false;
            }
        }

        private void CalculateRayCasts()
        {
            var position = transform.position;

            RaycastHit hitGround;
            Ray rayDownWard = new Ray(position,Vector3.down);
            
            if (Physics.Raycast(rayDownWard, out hitGround, groundMask))
            {
                _distanceToGround = hitGround.distance;
            }
        }


        private bool CanMoveForward(out bool canVault, out float distance, float maxDistance)
        {
            Vector3 origin = new Vector3(transform.position.x, _playerHeight / 2, transform.position.z);
            Ray rayForward = new Ray(origin,transform.forward);
            RaycastHit hitWall;

            float angleFromGround = -1;
            float heightFromGround = -1;
            distance = -1;
            Debug.DrawRay(origin,transform.forward*2,Color.green);
            if (Physics.Raycast(rayForward, out hitWall))
            {
                distance = Vector3.Distance(origin, hitWall.point);
                angleFromGround = Vector3.Angle(rayForward.direction, hitWall.normal) - 90;
                heightFromGround = hitWall.collider.bounds.extents.y;
                Debug.DrawRay(hitWall.point,hitWall.normal*2,Color.red);
            }
            canVault = distance < maxDistance && angleFromGround > 40 && angleFromGround <= 90 && Math.Round(heightFromGround,2) <= _playerHeight / 2;
            return (distance < 0 || distance > maxDistance) && !canVault;
        }
        
        private void OnCollisionEnter(Collision other)
        {
            if (other.transform.CompareTag("Ground"))
            {
                ResetPlayerToDafault();
            }
        }

        private void GroundCollisionCheck()
        {
            
            if (_distanceToGround > maxDistanceToFall)
            {
                _inputController.MovementType = PlayerInputController.MovementTypeEnum.Fall;
                
            }
        }

        public void ResetPlayerToDafault()
        {
            _playerIk.EnableFeetIk = true;
            _animator.applyRootMotion = true;
            _canRoll = true;
            _grounded = true;
            _inputController.MovementType = PlayerInputController.MovementTypeEnum.Run;
        }

        public bool IsOnGround()
        {
            return Physics.OverlapSphereNonAlloc(transform.position, groundCheckSphereRadius,null,groundMask) > 0;
        }
        public void MatchTarget(Vector3 matchPosition, Quaternion matchRotation, AvatarTarget target, MatchTargetWeightMask weightMask, float normalisedStartTime, float normalisedEndTime)
        {
            var animator = GetComponent<Animator>();

            if (animator.isMatchingTarget)
                return;

            float normalizeTime = Mathf.Repeat(animator.GetCurrentAnimatorStateInfo(0).normalizedTime, 1f);

            if (normalizeTime > normalisedEndTime)
                return;

            animator.MatchTarget(matchPosition, matchRotation, target, weightMask, normalisedStartTime, normalisedEndTime);
        }
        #endregion

       
    }
}