using UnityEngine;

public class Gravity
{
    private readonly CharacterController _controller;

    private float _maximumVerticalVelocity;
    private float _currentVerticalVelocity;

    public Gravity(CharacterController characterController)
    {
        _controller = characterController;
    }

    public float GetUpdateVelocity()
    {
        UpdateVelosity();

        return _currentVerticalVelocity;
    }

    private void UpdateVelosity()
    {
        bool isGrounded = _controller.isGrounded && _currentVerticalVelocity != _maximumVerticalVelocity;
        _currentVerticalVelocity -= isGrounded ? Mathf.Epsilon : PlayerParams.Gravity * Time.deltaTime;
    }

    public void SetVerticalVelocity(float value)
    {
        _currentVerticalVelocity = value;
        _maximumVerticalVelocity = value;
    }
}