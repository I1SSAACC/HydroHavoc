using UnityEngine;
using UnityEngine.UI;
using Mirror;

public class PlayerTeamManager : NetworkBehaviour
{
    [SerializeField] private Button _addButton;
    [SerializeField] private Button _removeButton;
    [SerializeField] private GameObject _player;
    [SerializeField] private CNetworkManager _networkManager;

    private void Start()
    {
        if (_addButton != null)
            _addButton.onClick.AddListener(AddPlayerAccount);
    }

    public void AddPlayerAccount()
    {
        _player = NetworkClient.localPlayer.gameObject;
        CmdStartGame();
    }


    [Command(requiresAuthority = false)]
    public void CmdStartGame(NetworkConnectionToClient sender = null)
    {
        _player = sender.identity.gameObject;
        _networkManager.AddToTeam(_player);
    }
}