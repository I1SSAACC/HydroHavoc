using UnityEngine;

public static class Utils
{
    public static float RoundThreeDecimalPlaces(float value) =>
        Mathf.Round(value * 1000f) / 1000f;
}