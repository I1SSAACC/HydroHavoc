using UnityEngine;
using Mirror;
using System;

public class NicknameStartAdder : NetworkBehaviour
{
    [SerializeField] private string _startName;
    public override void  OnStartClient()
    {
        _startName = NicknameRandomizer.GetCreatedName();
        UpdateCanvasName(_startName, isLocalPlayer);
    }

    public string GetName()
    {
        return _startName;
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
