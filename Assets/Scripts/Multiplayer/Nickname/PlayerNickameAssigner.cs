using Mirror;
using UnityEngine;

public class PlayerNickameAssigner : NetworkBehaviour
{
    [SerializeField] private PlayerNicknameView _viewName;
    [SerializeField] private PlayerHealthView _viewHP;
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
        string name = _viewName.CreateName();
        _viewName.UpdateName(name);
        _viewName.UpdateCanvasName(name, isLocalPlayer);
        _viewName.gameObject.SetActive(false);
        _viewHP.gameObject.SetActive(false);

        CmdSetupPlayer(name);
    }

    [Command]
    public void CmdSetupPlayer(string name) =>
        Name = name;

    private void OnNameChanged(string oldName, string newName)
    {
        _viewName.UpdateName(Name);
        _viewName.UpdateCanvasName(Name, isLocalPlayer);
    }
}