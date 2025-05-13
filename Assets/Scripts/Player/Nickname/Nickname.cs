using Mirror;
using UnityEngine;

public class Nickname : NetworkBehaviour
{
    [SerializeField] private NicknameView _nameView;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string Name;

    [Command]
    public void SetName(string name)
    {
        if (isLocalPlayer)
            Name = name;
    }

    private void OnNameChanged(string _, string newName) =>
        _nameView.UpdateName(newName, isLocalPlayer);
}