using Mirror;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

public class InputInformer : NetworkBehaviour
{
    [Header(" урсор и мышь")]
    [SerializeField] private bool _isCursorLocked = true;
    [SerializeField] private bool _isInputRotation = true;

    private Vector2 _move;
    private Vector2 _look;
    private bool _isWalking;

    public event Action JumpPressed;
    public event Action CrouchPressed;
    public event Action CrouchUnpressed;

    public bool IsWalking => _isWalking;

    public Vector2 Move => _move;

    public Vector2 Look => _look;

    public void OnMove(InputValue value) =>
        MoveInput(value.Get<Vector2>());

    public void OnLook(InputValue value)
    {
        if (_isInputRotation)
            LookInput(value.Get<Vector2>());
    }

    public void OnJump(InputValue _) =>
        JumpPressed?.Invoke();

    public void OnCrouching(InputValue _) =>
        CrouchPressed?.Invoke();

    public void OnCrouchingUp(InputValue _) =>
        CrouchUnpressed?.Invoke();

    public void OnSprint(InputValue value) =>
        SprintInput(value.isPressed);

    private void OnApplicationFocus(bool hasFocus) =>
        SetCursorState(_isCursorLocked);

    public void MoveInput(Vector2 newMoveDirection) =>
        _move = newMoveDirection;

    public void LookInput(Vector2 newLookDirection) =>
        _look = newLookDirection;

    public void SprintInput(bool newSprintState) =>
        _isWalking = newSprintState;

    private void SetCursorState(bool newState) =>
        Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
}