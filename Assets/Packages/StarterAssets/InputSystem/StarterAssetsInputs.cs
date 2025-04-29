using Mirror;
using UnityEngine;
using UnityEngine.InputSystem;

namespace StarterAssets
{
    public class StarterAssetsInputs : NetworkBehaviour
    {
        [Header("Character Input Values")]
        public Vector2 move;
        public Vector2 look;
        public bool jump;
        private bool _isSprint;

        [Header("Movement Settings")]
        public bool analogMovement;

        [Header("Mouse Cursor Settings")]
        [SerializeField] private bool _isCursorLocked = true;
        [SerializeField] private bool _isInputRotation = true;

        public bool IsSprint => _isSprint;

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
            move = newMoveDirection;

        public void LookInput(Vector2 newLookDirection) =>
            look = newLookDirection;

        public void JumpInput(bool newJumpState) =>
            jump = newJumpState;

        public void SprintInput(bool newSprintState) =>
            _isSprint = newSprintState;

        private void SetCursorState(bool newState) =>
            Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
    }
}