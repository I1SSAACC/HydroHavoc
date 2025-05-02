using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class StarterAssetsInputs : NetworkBehaviour
    {
        [Header("Character Input Values")]
        private Vector2 _move;
        private Vector2 _look;
        private bool _isJump;
        private bool _isWalking;
        private bool _isSneaking;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        [SerializeField] private bool _isCursorLocked = true;
        [SerializeField] private bool _isInputRotation = true;

        public bool IsWalking => _isWalking;

        public Vector2 Move => _move;

        public Vector2 Look => _look;

        public bool IsJump => _isJump;

        public bool IsSneaking => _isSneaking;

        public void SetJumpStatus(bool isJump) =>
            _isJump = isJump;

        public void OnMove(InputValue value) =>
            MoveInput(value.Get<Vector2>());

        public void OnLook(InputValue value)
        {
            if (_isInputRotation)
                LookInput(value.Get<Vector2>());
        }

        public void OnJump(InputValue value) =>
            JumpInput(value.isPressed);

        public void OnSprint(InputValue value) =>
            SprintInput(value.isPressed);

        private void OnApplicationFocus(bool hasFocus) =>
            SetCursorState(_isCursorLocked);

        public void MoveInput(Vector2 newMoveDirection) =>
            _move = newMoveDirection;

        public void LookInput(Vector2 newLookDirection) =>
            _look = newLookDirection;

        public void JumpInput(bool newJumpState) =>
            _isJump = newJumpState;

        public void SprintInput(bool newSprintState) =>
            _isWalking = newSprintState;

        private void SetCursorState(bool newState) =>
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}