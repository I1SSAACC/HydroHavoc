using UnityEngine;

public class DeltaMovementCalculator
{
    private readonly Transform _transform;
    private Vector3 _previousPosition;

    public DeltaMovementCalculator(Transform transform)
    {
        _transform = transform;
        _previousPosition = _transform.position;
    }

    public Vector2 GetNormalizedDelta() =>
        GetDelta() / (PlayerParams.DefaultSpeed * Time.deltaTime);

    private Vector2 GetDelta()
    {
        Vector3 delta = Utils.ResetHeight(_transform.position) - Utils.ResetHeight(_previousPosition);
        delta = _transform.InverseTransformDirection(delta);
        _previousPosition = _transform.position;

        return new(delta.x, delta.z);
    }
}