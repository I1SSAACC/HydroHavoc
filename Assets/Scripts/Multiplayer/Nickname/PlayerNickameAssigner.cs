using Mirror;
using UnityEngine;

public class PlayerNickameAssigner : NetworkBehaviour
{
    [SerializeField] private Nameplate nameplate;
    [SerializeField] private PlayerNicknameView _viewName;
    [SerializeField] private PlayerHealthView _viewHP;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string Name;

    public override void OnStartLocalPlayer()
    {
        base.OnStartLocalPlayer();

        string name = _viewName.CreateName(isLocalPlayer);
        CmdSetupPlayer(name);

        nameplate.gameObject.SetActive(false);
    }

    [Command]
    public void CmdSetupPlayer(string name) =>
        Name = name;

    private void OnNameChanged(string _, string newName)
    {
        Name = newName;
        _viewName.UpdateName(newName, isLocalPlayer);
    }
}