using UnityEngine;
using TMPro;
using System;

public class NicknameView : MonoBehaviour
{
    [SerializeField] private TMP_Text _text;

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