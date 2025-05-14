using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public struct MatchmakingRequestMessage : NetworkMessage
{
    public bool IsJoining;
}

public class MatchmakingManager : NetworkBehaviour
{
    // Public constants
    public const string DEFAULT_PLAYGROUND_SCENE = "Playground";

    // Private constants
    private const float LOADING_CHECK_INTERVAL = 0.1f;

    // Serialized fields
    [SerializeField] private int _requiredPlayersCount = 4;
    [SerializeField] private GameObject _newPlayerPrefab;
    [SerializeField] private string _playgroundSceneName = DEFAULT_PLAYGROUND_SCENE;

    // Private fields
    private readonly List<NetworkConnectionToClient> _playerQueue = new List<NetworkConnectionToClient>();
    private bool _isMatchmakingInProgress = false;

    // Public properties
    public int QueueCount => _playerQueue.Count;

    // Unity methods
    public override void OnStartServer()
    {
        DontDestroyOnLoad(gameObject);

        NetworkServer.RegisterHandler<MatchmakingRequestMessage>(OnMatchmakingRequestMessage);
    }

    [ServerCallback]
    private void OnDestroy()
    {
        if (NetworkServer.active == true)
            NetworkServer.UnregisterHandler<MatchmakingRequestMessage>();
    }

    // Public custom methods (для вызова из UI)
    [Client]
    public void JoinQueue()
    {
        if (NetworkClient.active == false)
            return;

        MatchmakingRequestMessage message = new MatchmakingRequestMessage { IsJoining = true };
        NetworkClient.Send(message);
    }

    [Client]
    public void LeaveQueue()
    {
        if (NetworkClient.active == false)
            return;

        MatchmakingRequestMessage message = new MatchmakingRequestMessage { IsJoining = false };
        NetworkClient.Send(message);
    }

    // Private custom methods
    [Server]
    private void OnMatchmakingRequestMessage(NetworkConnectionToClient conn, MatchmakingRequestMessage message)
    {
        if (message.IsJoining == true)
            AddPlayerToQueue(conn);
        else
            RemovePlayerFromQueue(conn);
    }

    [Server]
    private void AddPlayerToQueue(NetworkConnectionToClient conn)
    {
        if (_playerQueue.Contains(conn) == false)
            _playerQueue.Add(conn);

        if (_playerQueue.Count >= _requiredPlayersCount && _isMatchmakingInProgress == false)
        {
            _isMatchmakingInProgress = true;
            StartCoroutine(LoadPlaygroundAndSpawnPlayers());
        }
    }

    [Server]
    private void RemovePlayerFromQueue(NetworkConnectionToClient conn)
    {
        if (_playerQueue.Contains(conn) == true)
            _playerQueue.Remove(conn);
    }

    [Server]
    private IEnumerator LoadPlaygroundAndSpawnPlayers()
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_playgroundSceneName, LoadSceneMode.Additive);
        yield return asyncLoad;

        // Устанавливаем загруженную сцену как активную
        Scene playgroundScene = SceneManager.GetSceneByName(_playgroundSceneName);
        SceneManager.SetActiveScene(playgroundScene);

        foreach (NetworkConnectionToClient conn in _playerQueue)
        {
            // Создание экземпляра нового префаба игрока
            GameObject newPlayerInstance = Instantiate(_newPlayerPrefab);

            // Перемещение нового объекта в активную сцену
            SceneManager.MoveGameObjectToScene(newPlayerInstance, playgroundScene);

            // Добавляем небольшую задержку для инициализации новой NetworkIdentity
            yield return new WaitForSeconds(0.1f);

            // Передаём авторитет новому объекту, заменяя существующего игрока
            NetworkServer.ReplacePlayerForConnection(conn, newPlayerInstance, 0u);

            yield return new WaitForEndOfFrame();
        }

    _playerQueue.Clear();
        _isMatchmakingInProgress = false;
    }
}
