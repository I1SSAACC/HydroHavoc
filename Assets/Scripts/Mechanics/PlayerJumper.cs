using StarterAssets;
using System;
using UnityEngine;

public class PlayerJumper
{
    private const float _jumpHeight = 1.2f;
    private const float _gravity = -10.0f;
    private const float _jumpTimeout = 0.50f;
    private const float _fallTimeout = 0.15f;
    private const float _playerMass = 80f;

    private bool _isGrounded = true;
    private float _groundedOffset = -0.14f;
    private float _groundedRadius = 0.28f;

    private readonly StarterAssetsInputs _input;
    private readonly Transform _transform;
    private readonly LayerMask _groundLayers;

    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;
    private float _verticalVelocity;

    public event Action JumpAppeared;
    public event Action JumpCompleted;
    public event Action FallsFreely;

    public float VerticalVelocity => _verticalVelocity;

    public bool IsGrounded => _isGrounded;

    public PlayerJumper(StarterAssetsInputs input, Transform player, LayerMask groundLayers)
    {
        _input = input;
        _transform = player;
        _groundLayers = groundLayers;
        ResetTimeoutOnStart();
    }

    public void ProcessJump()
    {
        bool isJump = _input.IsJump;

        if (isJump)
            Debug.Log("jump");

        Debug.Log($"_isGrounded == {_isGrounded}");

        if (_isGrounded)
        {
            _fallTimeoutDelta = _fallTimeout;
            JumpCompleted?.Invoke();

            if (_verticalVelocity < 0.0f)
                _verticalVelocity = -2f;

            if (isJump && _jumpTimeoutDelta <= 0.0f)
            {
                _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);
                JumpAppeared?.Invoke();
            }

            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
                FallsFreely?.Invoke();
            }
        }
        else
        {
            _jumpTimeoutDelta = _jumpTimeout;

            if (_fallTimeoutDelta >= 0.0f)
                _fallTimeoutDelta -= Time.deltaTime;
            else
                _input.SetJumpStatus(false);
        }

        if (_verticalVelocity < _playerMass)
            _verticalVelocity += _gravity * Time.deltaTime;
    }

    public void CheckGrounded()
    {
        Vector3 spherePosition = new(
            _transform.position.x,
            _transform.position.y - _groundedOffset,
            _transform.position.z);

        _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);
    }

    private void ResetTimeoutOnStart()
    {
        _jumpTimeoutDelta = _jumpTimeout;
        _fallTimeoutDelta = _fallTimeout;
    }
}