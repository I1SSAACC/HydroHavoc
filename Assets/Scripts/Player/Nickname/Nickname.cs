using Mirror;
using UnityEngine;

public class Nickname : NetworkBehaviour
{
    private const string Player = nameof(Player);

    [SerializeField] private NicknameView _nameView;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string Name;

    public override void OnStartLocalPlayer() =>
        Name = CreateName();

    private void OnNameChanged(string _, string newName) =>
        _nameView.UpdateName(newName, isLocalPlayer);

    private string CreateName() =>
        $"{Player}{Random.Range(100, 999)}";
}