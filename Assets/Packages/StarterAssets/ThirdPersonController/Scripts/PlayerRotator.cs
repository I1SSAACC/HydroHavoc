using StarterAssets;
using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerRotator
{
    private const float Threshold = 0.01f;

    private readonly Vector2 _rotationVerticalLimits = new(-30f, 70f);

    private readonly PlayerCameraTarget _cameraTarget;
    private readonly StarterAssetsInputs _input;
    private readonly PlayerInput _playerInput;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    public PlayerRotator(StarterAssetsInputs input, PlayerInput playerInput, PlayerCameraTarget cameraTarget)
    {
        if(input == null)
            throw new ArgumentNullException(nameof(input), "Аргументу не присвоено значение");

        if (playerInput == null)
            throw new ArgumentNullException(nameof(playerInput), "Аргументу не присвоено значение");

        _cameraTarget = cameraTarget;
        _input = input;
        _playerInput = playerInput;
        _cinemachineTargetYaw = _cameraTarget.transform.rotation.eulerAngles.y;
    }

    private bool IsCurrentDeviceMouse
    {
        get
        {
#if ENABLE_INPUT_SYSTEM
            return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
        }
    }

    public void RotateCamera()
    {
        if (_input.look.sqrMagnitude >= Threshold)
        {
            float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

            _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
            _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
        }

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _rotationVerticalLimits.x, _rotationVerticalLimits.y);

        _cameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f)
            lfAngle += 360f;

        if (lfAngle > 360f)
            lfAngle -= 360f;

        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
}