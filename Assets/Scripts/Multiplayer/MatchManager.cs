using Mirror;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct AddPlayerMessage : NetworkMessage { }

public struct RemovePlayerMessage : NetworkMessage { }

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private ButtonClickInformer _addToQueue;
    [SerializeField] private ButtonClickInformer _removeFromQueue;
    [SerializeField] private LoadSceneMode _sceneMode;
    [SerializeField] private int _requiredPlayers;

    private readonly List<NetworkConnectionToClient> _playersInQueue = new();
    private List<Scene> _loadedScenes = new();

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

        NetworkServer.RegisterHandler<AddPlayerMessage>(OnAddedPlayer);
        NetworkServer.RegisterHandler<RemovePlayerMessage>(OnRemovedPlayer);
    }

    public override void OnStopServer()
    {
        Debug.LogWarning("Сервер остановлен");

        NetworkServer.UnregisterHandler<AddPlayerMessage>();
        NetworkServer.UnregisterHandler<RemovePlayerMessage>();
    }

    public override void OnStartClient()
    {
        Debug.LogWarning("Клиент присоединился");
    }

    public override void OnStopClient()
    {
        Debug.LogWarning("Остановка клиента");
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
        NetworkClient.Send(new AddPlayerMessage());

    private void RemovePlayerFromQueue() =>
        NetworkClient.Send(new RemovePlayerMessage());

    private void OnAddedPlayer(NetworkConnectionToClient conn, AddPlayerMessage _)
    {
        _playersInQueue.Add(conn);

        if (_playersInQueue.Count >= _requiredPlayers)
            StartMatch();
    }

    private void OnRemovedPlayer(NetworkConnectionToClient connection, RemovePlayerMessage _)
    {
        if (connection != null)
            _playersInQueue.Remove(connection);
    }

    private void StartMatch()
    {
        Debug.Log($"_playersInQueue.Count = {_playersInQueue.Count}");
        StartCoroutine(CreateMatch(new(_playersInQueue)));
        _playersInQueue.Clear();
    }

    private IEnumerator CreateMatch(List<NetworkConnectionToClient> connections)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.Scenes.Playground, LoadSceneMode.Additive);

        yield return asyncLoad;

        Scene loadedScene = SceneManager.GetSceneByName(Constants.Scenes.Playground);
        if (loadedScene.isLoaded)
        {
            _loadedScenes.Add(loadedScene);
            SceneManager.SetActiveScene(loadedScene);
        }

        foreach (NetworkConnectionToClient connection in connections)
            MovePlayersInScene(connection);
    }

    private void MovePlayersInScene(NetworkConnectionToClient connection)
    {
        GameObject oldPlayer = connection.identity.gameObject;
        UIDisabler uiDisable = oldPlayer.GetComponent<UIDisabler>();

        if (uiDisable != null)
            uiDisable.UIDisable();

        GameObject newPlayer = Instantiate(_playerPrefab);

        NetworkServer.ReplacePlayerForConnection(connection, newPlayer, ReplacePlayerOptions.KeepAuthority);
        SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetActiveScene());

        //NicknameStartAdder oldName = oldPlayer.GetComponent<NicknameStartAdder>();
        //string nameToSet = oldName.GetName();
        //Nickname newName = newPlayer.GetComponent<Nickname>();
        //newName.SetName(nameToSet);
    }
}