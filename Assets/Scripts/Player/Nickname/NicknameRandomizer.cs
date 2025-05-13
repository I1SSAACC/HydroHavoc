using UnityEngine;

public static class NicknameRandomizer
{
    private const string Player = nameof(Player);

    public static string GetCreatedName() =>
        $"{Player}{Random.Range(100, 999)}";
}