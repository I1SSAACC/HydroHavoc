using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public struct AddPlayerMessage : NetworkMessage { }
public struct RemovePlayerMessage : NetworkMessage { }

public struct SceneMessage : NetworkMessage
{
    public string sceneName;
    public LoadSceneMode loadMode;
}

public class MatchManager : NetworkBehaviour
{
    [SerializeField] private GameObject _playerPrefab;
    [SerializeField] private ButtonClickInformer _addToQueue;
    [SerializeField] private ButtonClickInformer _removeFromQueue;
    [SerializeField] private LoadSceneMode _sceneMode;  // Обычно Additive
    [SerializeField] private int _requiredPlayers;

    private readonly List<NetworkConnectionToClient> _playersInQueue = new();
    private readonly List<Scene> _loadedScenes = new();

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
        // Регистрируем обработчик нового сообщения для загрузки сцены
        NetworkClient.RegisterHandler<SceneMessage>(OnSceneMessageReceived);
        // Регистрируем спавнимый префаб игрока
        NetworkClient.RegisterPrefab(_playerPrefab);
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
        // Создаём копию подключений
        List<NetworkConnectionToClient> connections = new List<NetworkConnectionToClient>(_playersInQueue);
        _playersInQueue.Clear();
        StartCoroutine(CreateMatch(connections));
    }

    private IEnumerator CreateMatch(List<NetworkConnectionToClient> connections)
    {
        // Загружаем новую сцену (например, Playground) асинхронно на сервере
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Constants.Scenes.Playground, LoadSceneMode.Additive);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }

        Scene loadedScene = SceneManager.GetSceneByName(Constants.Scenes.Playground);
        if (loadedScene.isLoaded)
        {
            _loadedScenes.Add(loadedScene);
            // Устанавливаем загруженную сцену как активную на сервере
            SceneManager.SetActiveScene(loadedScene);
        }
        else
        {
            Debug.LogError("Сцена не была загружена!");
            yield break;
        }

        // Отправляем сообщение клиентам, чтобы они также загрузили новую сцену
        SceneMessage sceneMsg = new SceneMessage
        {
            sceneName = Constants.Scenes.Playground,
            loadMode = _sceneMode
        };
        foreach (NetworkConnectionToClient conn in connections)
        {
            conn.Send(sceneMsg);
        }

        // Даем клиентам время на загрузку сцены (при необходимости увеличьте задержку)
        yield return new WaitForSeconds(2f);

        // Перемещаем игроков в загруженную и активную сцену на сервере
        foreach (NetworkConnectionToClient connection in connections)
        {
            MovePlayersInScene(connection);
            yield return new WaitForEndOfFrame();
        }
    }

    // Обработчик сообщения для загрузки сцены на клиенте
    private void OnSceneMessageReceived(SceneMessage msg)
    {
        StartCoroutine(LoadAndActivateScene(msg));
    }

    private IEnumerator LoadAndActivateScene(SceneMessage msg)
    {
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(msg.sceneName, msg.loadMode);
        while (!asyncLoad.isDone)
        {
            yield return null;
        }
        Scene loadedScene = SceneManager.GetSceneByName(msg.sceneName);
        if (loadedScene.IsValid() && loadedScene.isLoaded)
        {
            SceneManager.SetActiveScene(loadedScene);
            Debug.Log("Клиент: загружена и активирована сцена " + msg.sceneName);
        }
        else
        {
            Debug.LogError("Клиент: не удалось загрузить сцену " + msg.sceneName);
        }
    }

    // Метод, который на сервере заменяет игрока новым объектом в активной (новой) сцене
    private void MovePlayersInScene(NetworkConnectionToClient connection)
    {
        // Получаем старый объект игрока
        GameObject oldPlayer = connection.identity.gameObject;
        UIDisabler uiDisable = oldPlayer.GetComponent<UIDisabler>();
        if (uiDisable != null)
            uiDisable.UIDisable();

        // Создаем новый объект игрока
        GameObject newPlayer = Instantiate(_playerPrefab);
        // Перемещаем новый объект в активную (новую) сцену
        Scene newScene = SceneManager.GetActiveScene();
        if (newPlayer.scene != newScene)
            SceneManager.MoveGameObjectToScene(newPlayer, newScene);

        // Заменяем игрока для подключения, сохраняя авторитет
        NetworkServer.ReplacePlayerForConnection(connection, newPlayer, ReplacePlayerOptions.KeepAuthority);
    }
}