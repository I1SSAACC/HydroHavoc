using System;
using UnityEngine;

public class PlayerRotator
{
    private const float Threshold = 0.01f;

    private readonly Transform _cameraTarget;
    private readonly Transform _transform;
    private readonly InputInformer _input;

    private float _cinemachineTargetPitch;
    private float _rotationVelocity;

    public PlayerRotator(InputInformer input, Transform player)
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
        Vector2 lookInput = _input.Look;

        if (lookInput.sqrMagnitude < Threshold)
            return;

        _cinemachineTargetPitch += lookInput.y * PlayerParams.RotationSensitivity;
        _rotationVelocity = lookInput.x * PlayerParams.RotationSensitivity;

        _cinemachineTargetPitch = Utils.ClampAngle(_cinemachineTargetPitch, PlayerParams.VerticalRotationLimits.x, PlayerParams.VerticalRotationLimits.y);
        _cameraTarget.transform.localRotation = Quaternion.Euler(_cinemachineTargetPitch, 0.0f, 0.0f);
        _transform.Rotate(Vector3.up * _rotationVelocity);
    }
}