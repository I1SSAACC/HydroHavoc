using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct MatchmakingMessage : NetworkMessage { }

public struct LeaveMatchmakingMessage : NetworkMessage { }

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private int _requiredPlayers;
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private GameObject _uiCanvas;
    [SerializeField] private ButtonClickInformer _addToQueue;
    [SerializeField] private ButtonClickInformer _removeFromQueue;

    private readonly List<NetworkConnectionToClient> _playersInQueue = new();

    private void OnEnable()
    {
        _addToQueue.Clicked += AddPlayerToQueue;
        _removeFromQueue.Clicked += RemovePlayerFromQueue;
    }

    private void OnDisable()
    {
        _addToQueue.Clicked -= AddPlayerToQueue;
        _removeFromQueue.Clicked -= RemovePlayerFromQueue;
    }

    public override void OnStartServer()
    {
        Debug.LogWarning("Сервер запущен");

        NetworkServer.RegisterHandler<MatchmakingMessage>(OnMatchmakingMessageReceived);
        NetworkServer.RegisterHandler<LeaveMatchmakingMessage>(OnLeaveMatchmakingMessageReceived);
    }

    public override void OnStopServer()
    {
        Debug.LogWarning("Сервер остановлен");

        NetworkServer.UnregisterHandler<MatchmakingMessage>();
        NetworkServer.UnregisterHandler<LeaveMatchmakingMessage>();
    }

    public override void OnStartClient()
    {
        Debug.LogWarning("Клиент присоединился");
    }

    public override void OnStopClient()
    {
        Debug.LogWarning("Клиент откючился");
    }

    public override void OnStartLocalPlayer()
    {
        Debug.LogWarning("OnStartLocalPlayer()");
    }

    public override void OnStartAuthority()
    {
        Debug.LogWarning("OnStartAuthority()");
    }

    public override void OnStopAuthority()
    {
        Debug.LogWarning("OnStopAuthority()");
    }

    private void AddPlayerToQueue() =>
        NetworkClient.Send(new MatchmakingMessage());

    private void RemovePlayerFromQueue() =>
        NetworkClient.Send(new LeaveMatchmakingMessage());

    private void OnMatchmakingMessageReceived(NetworkConnectionToClient conn, MatchmakingMessage _)
    {
        _playersInQueue.Add(conn);

        if (_playersInQueue.Count >= _requiredPlayers)
            StartMatch();
    }

    private void OnLeaveMatchmakingMessageReceived(NetworkConnectionToClient conn, LeaveMatchmakingMessage _)
    {
        if (conn != null)
            _playersInQueue.Remove(conn);
    }

    private void StartMatch()
    {
        NetworkConnectionToClient[] connections = new NetworkConnectionToClient[_playersInQueue.Count];

        for (int i = 0; i < _playersInQueue.Count; i++)
            connections[i] = _playersInQueue[i];

        _playersInQueue.Clear();

        StartCoroutine(CreateMatch(connections));
    }

    private IEnumerator CreateMatch(NetworkConnectionToClient[] conns)
    {
        yield return SceneManager.LoadSceneAsync(Constants.Scenes.Playground, new LoadSceneParameters(LoadSceneMode.Additive));

        foreach (NetworkConnectionToClient conn in conns)
        {
            GameObject oldPlayer = conn.identity.gameObject;

            GameObject newPlayer = Instantiate(_playerPrefab);
            SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));

            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, ReplacePlayerOptions.KeepActive);
            //NetworkServer.Spawn(newPlayer, conn);
            //NetworkIdentity netId = newPlayer.GetComponent<NetworkIdentity>();            
            //netId.AssignClientAuthority(conn);

            if (oldPlayer == null)
                continue;

            NicknameStartAdder oldName = oldPlayer.GetComponent<NicknameStartAdder>();

            if (oldName)
            {
                string nameToSet = oldName.GetName();
                Nickname newName = newPlayer.GetComponent<Nickname>();
                newName.SetName(nameToSet);
            }

            UIDisabler uiDisable = oldPlayer.GetComponent<UIDisabler>();

            if (uiDisable)
                uiDisable.UIDisable();
        }
    }


}