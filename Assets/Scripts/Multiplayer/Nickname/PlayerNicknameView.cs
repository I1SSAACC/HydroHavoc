using UnityEngine;
using TMPro;
using System;

public class PlayerNicknameView : MonoBehaviour
{
    private const string Player = nameof(Player);

    [SerializeField] private TMP_Text _text;

    public string CreateName()
    {
        string name = $"{Player} {UnityEngine.Random.Range(100, 999)}";
        UpdateName(name);

        return name;
    }

    public void UpdateName(string text) =>
        _text.text = text;

    public void UpdateCanvasName(string name, bool isLocalPlayer)
    {
        if (isLocalPlayer == false)
            return;

        CanvasPlayerName canvasName = FindObjectOfType<CanvasPlayerName>();

        if (canvasName == null)
            throw new NullReferenceException("Не найден компонент имени игрока на канвасе");

        canvasName.UpdateName(name);
    }
}