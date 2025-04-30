using StarterAssets;
using UnityEngine;

public class Mover
{
    private const float SpeedChangeRate = 10.0f;
    private const float MoveSpeed = 3f;
    private const float SprintSpeed = 6f;

    private readonly CharacterController _controller;
    private readonly StarterAssetsInputs _input;
    private Transform _transform;

    private float _targetSpeed;
    private float _inputMagnitude;

    private Vector2 _currentInputs;

    public Mover(CharacterController controller, StarterAssetsInputs input)
    {
        _controller = controller;
        _input = input;
        _transform = _controller.transform;
    }

    public float TargetSpeed => _targetSpeed;

    public float InputMagnitude => _inputMagnitude;

    public Vector2 CurrentInputs => _currentInputs;

    public void Move(float verticalVelocity)
    {
        _targetSpeed = _input.IsSprint ? MoveSpeed : SprintSpeed;

        if (_input.move == Vector2.zero)
            _targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        _inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        float speedMovement;

        if (currentHorizontalSpeed < _targetSpeed - speedOffset ||
            currentHorizontalSpeed > _targetSpeed + speedOffset)
        {
            speedMovement = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * _inputMagnitude, Time.deltaTime * SpeedChangeRate);
            speedMovement = Utils.RoundThreeDecimalPlaces(speedMovement);
        }
        else
        {
            speedMovement = _targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        if (_input.move != Vector2.zero)
            inputDirection = _transform.right * _input.move.x + _transform.forward * _input.move.y;

        Vector3 inputs = inputDirection.normalized * (speedMovement * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime;

        _controller.Move(inputs);
        _currentInputs = new(inputDirection.x, inputDirection.z);
        _currentInputs *= SprintSpeed;
    }
}