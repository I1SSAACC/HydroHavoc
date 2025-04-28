using StarterAssets;
using UnityEngine;

public class Mover
{
    private const float SpeedChangeRate = 10.0f;

    private readonly CharacterController _controller;
    private readonly StarterAssetsInputs _input;
    private readonly float _moveSpeed;
    private readonly float _sprintSpeed;
    private readonly Transform _camera;

    private float _targetSpeed;
    private float _inputMagnitude;

    public Mover(StarterAssetsInputs input, CharacterController controller, float moveSpeed, float sprintSpeed)
    {
        _input = input;
        _controller = controller;
        _moveSpeed = moveSpeed;
        _sprintSpeed = sprintSpeed;
        _camera = Camera.main.transform;
    }

    public float TargetSpeed => _targetSpeed;

    public float InputMagnitude => _inputMagnitude;

    public void Move(float verticalVelocity)
    {
        _targetSpeed = _input.IsSprint ? _moveSpeed : _sprintSpeed;

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

        float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        //float rotation = Mathf.SmoothDampAngle(_controller.transform.eulerAngles.y, targetRotation, ref _rotationVelocity, 0);
        _controller.transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (speedMovement * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }
}