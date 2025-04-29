using StarterAssets;
using System;
using UnityEngine;

public class CameraRotator
{
    private const float Threshold = 0.01f;

    private readonly Vector2 _verticalRotationLimits = new(-30f, 80f);

    private readonly Transform _cameraTarget;
    private readonly StarterAssetsInputs _input;

    private float _cinemachineTargetYaw;
    private float _cinemachineTargetPitch;

    public CameraRotator(StarterAssetsInputs input, Transform player)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input), "Аргументу не присвоено значение");

        PlayerCameraTarget cameraTarget = player.GetComponentInChildren<PlayerCameraTarget>(true);

        if (cameraTarget == null)
            throw new ArgumentNullException(nameof(cameraTarget), "Не удалось получить компонент в иерархии");

        _cameraTarget = cameraTarget.transform;
        _input = input;
        _cinemachineTargetYaw = _cameraTarget.rotation.eulerAngles.y;
    }

    public void RotateCamera()
    {
        if (_input.look.sqrMagnitude < Threshold)
            return;

        _cinemachineTargetYaw += _input.look.x;
        _cinemachineTargetPitch += _input.look.y;

        _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _verticalRotationLimits.x, _verticalRotationLimits.y);

        _cameraTarget.rotation = Quaternion.Euler(_cinemachineTargetPitch, _cinemachineTargetYaw, 0.0f);
    }

    private static float ClampAngle(float angle, float minValue, float maxValue)
    {
        angle = Mathf.Repeat(angle, 360f);

        return Mathf.Clamp(angle, minValue, maxValue);
    }
}