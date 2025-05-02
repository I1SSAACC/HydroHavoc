using UnityEngine;
using TMPro;
using System;

public class PlayerNicknameView : MonoBehaviour
{
    private const string Player = nameof(Player);

    [SerializeField] private TMP_Text _text;

    public string CreateName(bool isLocalPlayer)
    {
        string name = $"{Player} {UnityEngine.Random.Range(100, 999)}";
        UpdateName(name, isLocalPlayer);

        return name;
    }

    public void UpdateName(string newName, bool isLocalPlayer)
    {
        _text.text = newName;
        UpdateCanvasName(newName, isLocalPlayer);
    }

    private void UpdateCanvasName(string name, bool isLocalPlayer)
    {
        if (isLocalPlayer == false)
            return;

        CanvasPlayerName canvasName = FindAnyObjectByType<CanvasPlayerName>();

        if (canvasName == null)
            throw new NullReferenceException("Не найден компонент имени игрока на канвасе");

        canvasName.UpdateName(name);
    }
}