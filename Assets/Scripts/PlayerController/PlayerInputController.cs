using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace JoystickLab
{
    [RequireComponent(typeof(PlayerInput))]
    public class PlayerInputController : Manager<PlayerInputController>
    {
        #region Variables

        private Vector2 _movement;
        private float _doRun;
        private bool _doCrouch;
        private float _doProne;
        private bool _doRoll;
        private bool _doJump;
        
        public Vector2 Movement => _movement;

        public float DoRun => _doRun;
        public bool DoCrouch => _doCrouch;
        public float DoProne => _doProne;
        public bool DoJump => _doJump;
        public bool DoRoll => _doRoll;
        
        public MovementTypeEnum MovementType
        {
            get => _movementType;
            set => _movementType = value;
        }


        
        public enum MovementTypeEnum
        {
            Idle,
            Walk,
            Run,
            Crouching,
            Proning,
            Sprint,
            Jump,
            Vault,
            Fall,
            Roll,
            Default,
            Action,
        }

        private MovementTypeEnum _movementType;
        #endregion


        #region MainMethods

        // Start is called before the first frame update
        void Start()
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            _movementType = MovementTypeEnum.Idle;
            
        }
        
        #endregion

        #region CallBack Methods

        private void OnMove(InputValue value)
        {
            _movement = value.Get<Vector2>();
            StateMachine.ChangeState(States.WalkState);
        }

        private void OnRun(InputValue value)
        {
            _doRun = value.Get<float>();
            StateMachine.ChangeState(States.RunState);
        }

        private void OnCrouch(InputValue value)
        {
            _doCrouch = value.Get<float>()>0.1f;
            _movementType = MovementTypeEnum.Crouching ;//: MovementTypeEnum.Walk;
        }

        private void OnProne(InputValue value)
        {
            _doProne = value.Get<float>();
            _movementType = MovementTypeEnum.Proning;
        }

        private void OnJump(InputValue value)
        {
            //_doJump = value.Get<float>() >=0.1f;
            print("jump");
            StateMachine.ChangeState(States.JumpState);
        }
        
        private void OnRoll(InputValue value)
        {
            _doRoll = value.Get<float>()>0.1f;
            _movementType = _doRoll ? MovementTypeEnum.Roll : MovementTypeEnum.Default;
        }

        void OnAction(InputValue value)
        {
            _movementType = MovementTypeEnum.Action;
        }

        #endregion

        
    }
}

