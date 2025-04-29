using StarterAssets;
using System;
using UnityEngine;

public class CameraRotator
{
    private const float Threshold = 0.01f;
    private const float RotationSpeed = 2f;

    private readonly Vector2 _verticalRotationLimits = new(-90f, 90f);

    private readonly Transform _cameraTarget;
    private readonly Transform _transform;
    private readonly StarterAssetsInputs _input;

    private float _cinemachineTargetPitch;
    private float _rotationVelocity;

    public CameraRotator(StarterAssetsInputs input, Transform player)
    {
        if (input == null)
            throw new ArgumentNullException(nameof(input), "Аргументу не присвоено значение");

        PlayerCameraTarget cameraTarget = player.GetComponentInChildren<PlayerCameraTarget>(true);

        if (cameraTarget == null)
            throw new ArgumentNullException(nameof(cameraTarget), "Не удалось получить компонент в иерархии");

        _transform = player;
        _cameraTarget = cameraTarget.transform;
        _input = input;
        _cinemachineTargetPitch = _cameraTarget.rotation.eulerAngles.y;
    }

    public void RotateCamera()
    {
        if (_input.look.sqrMagnitude < Threshold)
            return;

        _cinemachineTargetPitch += _input.look.y * RotationSpeed;
        _rotationVelocity = _input.look.x * RotationSpeed;

        _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _verticalRotationLimits.x, _verticalRotationLimits.y);
        _cameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        _transform.Rotate(Vector3.up * _rotationVelocity);
    }

    private static float ClampAngle(float angle, float minValue, float maxValue)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;

        return Mathf.Clamp(angle, minValue, maxValue);
    }
}