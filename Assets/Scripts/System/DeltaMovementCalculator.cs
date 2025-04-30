using UnityEngine;

public class DeltaMovementCalculator
{
    private const float Threshold = 0.05f;

    private readonly Transform _transform;
    private Vector3 _previousPosition;

    public DeltaMovementCalculator(Transform transform)
    {
        _transform = transform;
        _previousPosition = _transform.position;
    }

    public Vector2 GetNormalizedDelta() =>
        GetDelta() / (6 * Time.deltaTime);

    private Vector2 GetDelta()
    {
        Vector3 delta = Utils.ResetHeight(_transform.position) - Utils.ResetHeight(_previousPosition);
        delta = _transform.InverseTransformDirection(delta);
        _previousPosition = _transform.position;

        if (Mathf.Abs(delta.x) < Threshold)
            delta.x = 0;

        if (Mathf.Abs(delta.z) < Threshold)
            delta.z = 0;

        return new(delta.x, delta.z);
    }
}