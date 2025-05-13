using UnityEngine;

public static class Utils
{
    public static Vector3 ResetHeight(Vector3 value) =>
        new(value.x, 0, value.z);

    public static float ClampAngle(float angle, float minValue, float maxValue)
    {
        if (angle < -360f) angle += 360f;
        if (angle > 360f) angle -= 360f;

        return Mathf.Clamp(angle, minValue, maxValue);
    }
}