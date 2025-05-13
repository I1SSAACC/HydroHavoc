using UnityEngine;

public class PlayerMover
{
    private readonly CharacterController _characterController;
    private readonly Gravity _gravity;
    private readonly float _speed;

    public PlayerMover(Transform transform)
    {
        _characterController = transform.GetComponent<CharacterController>();
        _gravity = new(_characterController);
        _speed = PlayerParams.DefaultSpeed;
    }

    public void Move(Vector2 direction)
    {
        direction *= _speed;
        Vector3 localDirection = new(direction.x, _gravity.GetUpdateVelocity(), direction.y);
        localDirection = _characterController.transform.TransformDirection(localDirection);
        _characterController.Move(localDirection * Time.deltaTime);
    }

    public void SetVerticalVelocity(float velocity) =>
        _gravity.SetVerticalVelocity(velocity);
}