using UnityEngine;

public static class Utils
{
    public static Vector3 ResetHeight(Vector3 value)
    {
        value.y = 0;
        return value;
    }

    public static float RoundThreeDecimalPlaces(float value) =>
        Mathf.Round(value * 1000f) / 1000f;

    public static bool IsHeadless()
    {
        return Application.isBatchMode;
    }
}
