using Mirror;
using UnityEngine;

public class PlayerNickameAssigner : NetworkBehaviour
{
    [SerializeField] private PlayerNicknameView _view;
    [SerializeField] private NicknameMarkerForRotation _rotationName;

    [SyncVar(hook = nameof(OnNameChanged))]
    public string Name;

    private void Update()
    {
        if (isLocalPlayer == false)
            _rotationName.transform.LookAt(Camera.main.transform);
    }

    public override void OnStartLocalPlayer()
    {
        string name = _view.CreateName();
        _view.UpdateName(name);
        _view.UpdateCanvasName(name, isLocalPlayer);
        _view.gameObject.SetActive(false);

        CmdSetupPlayer(name);
    }

    [Command]
    public void CmdSetupPlayer(string name) =>
        Name = name;

    private void OnNameChanged(string oldName, string newName)
    {
        _view.UpdateName(Name);
        _view.UpdateCanvasName(Name, isLocalPlayer);
    }
}