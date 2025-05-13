using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public struct MatchmakingMessage : NetworkMessage { }

public struct LeaveMatchmakingMessage : NetworkMessage { }

public struct PlayerInfo
{
    public NetworkConnectionToClient connection;
}

public class MatchManager : MonoBehaviour
{
    [SerializeField] private int _requiredPlayers;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _uiCanvas;

    private readonly List<PlayerInfo> _playersInQueue = new();

    private void Start()
    {
        CustomNetworkManager netMan = NetworkManager.singleton as CustomNetworkManager;
        netMan.ServerStarted += OnStartServer;
        netMan.ServerStopped += OnStopServer;
    }

    public void JoinMatchmaking() =>
        NetworkClient.Send(new MatchmakingMessage());

    public void LeaveMatchmaking() =>
        NetworkClient.Send(new LeaveMatchmakingMessage());

    private void OnStartServer()
    {
        NetworkServer.RegisterHandler<MatchmakingMessage>(OnMatchmakingMessageReceived);
        NetworkServer.RegisterHandler<LeaveMatchmakingMessage>(OnLeaveMatchmakingMessageReceived);
    }

    private void OnStopServer()
    {
        NetworkServer.UnregisterHandler<MatchmakingMessage>();
        NetworkServer.UnregisterHandler<LeaveMatchmakingMessage>();
    }

    private void OnMatchmakingMessageReceived(NetworkConnectionToClient conn, MatchmakingMessage _)
    {
        PlayerInfo playerInfo = new() { connection = conn };
        _playersInQueue.Add(playerInfo);

        if (_playersInQueue.Count >= _requiredPlayers)
            StartMatch();
    }

    private void OnLeaveMatchmakingMessageReceived(NetworkConnectionToClient conn, LeaveMatchmakingMessage _)
    {
        PlayerInfo playerToRemove = _playersInQueue.Find(p => p.connection == conn);
        if (playerToRemove.connection != null)
        {
            _playersInQueue.Remove(playerToRemove);
        }
    }

    private void StartMatch()
    {
        NetworkConnectionToClient[] connections = new NetworkConnectionToClient[_playersInQueue.Count];
        for (int i = 0; i < _playersInQueue.Count; i++)
        {
            connections[i] = _playersInQueue[i].connection;
        }

        _playersInQueue.Clear();
        StartCoroutine(CreateMatch(connections));
    }

    private IEnumerator CreateMatch(NetworkConnectionToClient[] conns)
    {
        yield return SceneManager.LoadSceneAsync("Playground", new LoadSceneParameters(LoadSceneMode.Additive));

        foreach (var conn in conns)
        {
            GameObject oldPlayer = conn.identity != null ? conn.identity.gameObject : null;

            GameObject newPlayer = Instantiate(_playerPrefab);
            SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));

            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, ReplacePlayerOptions.KeepActive);

            if (oldPlayer != null)
            {
                NicknameStartAdder oldName = oldPlayer.GetComponent<NicknameStartAdder>();
                if (oldName != null)
                {
                    string nameToSet = oldName.GetName();
                    Nickname newName = newPlayer.GetComponent<Nickname>();
                    newName.SetName(nameToSet);
                }

                UIDisabler uiDisable = oldPlayer.GetComponent<UIDisabler>();
                if (uiDisable != null)
                {
                    uiDisable.UIDisable();
                }
            }
        }
    }

    private void OnDestroy()
    {
        CustomNetworkManager netMan = NetworkManager.singleton as CustomNetworkManager;
        if (netMan == null) return;

        netMan.ServerStarted -= OnStartServer;
        netMan.ServerStopped -= OnStopServer;
    }
}
