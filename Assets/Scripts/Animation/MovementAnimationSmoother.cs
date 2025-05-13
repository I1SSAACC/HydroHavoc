using UnityEngine;

public class MovementAnimationSmoother
{
    private Vector2 _currentValue;
    private Vector2 _currentVelocity;

    public Vector2 CalculateMovementSmoothedValue(Vector2 targetValue)
    {
        _currentValue = Vector2.SmoothDamp(
            current: _currentValue,
            target: targetValue,
            currentVelocity: ref _currentVelocity,
            smoothTime: PlayerParams.SmoothTimeAnimationMovement
        );

        return _currentValue;
    }
}