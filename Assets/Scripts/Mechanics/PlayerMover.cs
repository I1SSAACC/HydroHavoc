using StarterAssets;
using UnityEngine;

public class PlayerMover
{
    private const float Acceleration = 10.0f;

    private readonly CharacterController _controller;
    private readonly StarterAssetsInputs _inputs;
    private readonly Transform _transform;

    private float _currentSpeed;
    private float _targetSpeed;
    private float _speedSmoothVelocity;

    public PlayerMover(CharacterController controller, StarterAssetsInputs inputs)
    {
        _controller = controller != null ? controller : throw new System.ArgumentNullException(nameof(controller));
        _inputs = inputs != null ? inputs : throw new System.ArgumentNullException(nameof(inputs));
        _transform = _controller.transform;
    }

    public void Move(float verticalVelocity)
    {
        _targetSpeed = CalculateTargetSpeed();
        Vector2 moveInput = _inputs.Move;

        _currentSpeed = Mathf.SmoothDamp(
            current: _currentSpeed,
            target: _targetSpeed * GetInputMagnitude(moveInput),
            currentVelocity: ref _speedSmoothVelocity,
            smoothTime: 1f / Acceleration
        );

        Vector3 moveDirection = Vector3.zero;
        if (moveInput != Vector2.zero)
        {
            moveDirection = _transform.right * moveInput.x + _transform.forward * moveInput.y;
            moveDirection.Normalize();
        }

        Vector3 motion = moveDirection * (_currentSpeed * Time.deltaTime) + Vector3.up * (verticalVelocity * Time.deltaTime);
        _controller.Move(motion);
    }

    private float CalculateTargetSpeed()
    {
        if (_inputs.Move == Vector2.zero)
            return 0f;
        if (_inputs.IsCrouching)
            return PlayerParams.SneakingSpeed;
        if (_inputs.IsWalking)
            return PlayerParams.WalkingSpeed;

        return PlayerParams.SprintSpeed;
    }

    private float GetInputMagnitude(Vector2 moveInput) =>
        _inputs.analogMovement ? moveInput.magnitude : 1f;
}