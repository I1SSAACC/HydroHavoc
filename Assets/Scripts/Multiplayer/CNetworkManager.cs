using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class CNetworkManager : NetworkManager
{
    public static CNetworkManager Match;

    [Scene]
    [SerializeField] private string _game;

    [SerializeField] private GameObject _player;

    [SerializeField] private int _maxPlayerTeams;

    private Scene _startGameScene;

    [SerializeField] private List<GameObject> _listTeamRed = new List<GameObject>();
    [SerializeField] private List<GameObject> _listTeamBlue = new List<GameObject>();

    public override void Awake()
    {
        base.Awake();
        Match = this;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<SearchMessage>(SpawnAccount);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.Send(default(SearchMessage));
    }

    public void SpawnAccount(NetworkConnectionToClient conn, SearchMessage send)
    {
        GameObject gameObject = Object.Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, gameObject);
    }

    public void AddToTeam(GameObject player)
    {
        if (_listTeamRed.Count < _maxPlayerTeams)
            _listTeamRed.Add(player);
        else if (_listTeamBlue.Count < _maxPlayerTeams)
            _listTeamBlue.Add(player);

        if (_listTeamBlue.Count == _maxPlayerTeams)
            OnClientStartSearching();

        CleanUpNullPlayers();
    }

    public void RemoveFromTeam(GameObject player)
    {
        _listTeamRed.Remove(player);
        _listTeamBlue.Remove(player);
    }

    private void CleanUpNullPlayers()
    {
        _listTeamRed.RemoveAll(player => player == null);
        _listTeamBlue.RemoveAll(player => player == null);
    }

    public void OnClientStartSearching()
    {
        SceneManager.LoadSceneAsync(_game, new LoadSceneParameters { loadSceneMode = LoadSceneMode.Additive, localPhysicsMode = LocalPhysicsMode.Physics3D });

        foreach (GameObject item in _listTeamBlue)
            MovePlayerToScene(item, _game);

        foreach (GameObject item in _listTeamRed)
            MovePlayerToScene(item, _game);

        _listTeamRed.Clear();
        _listTeamBlue.Clear();
    }

    private void MovePlayerToScene(GameObject player, string sceneName)
    {
        player.GetComponent<NetworkIdentity>().connectionToClient.Send(new SceneMessage { sceneName = sceneName });

        GameObject obj = player;
        GameObject newPlayer = Object.Instantiate(_player, GetStartPosition());
        newPlayer.transform.SetParent(null);
        SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneByName(sceneName));
        NetworkServer.ReplacePlayerForConnection(player.GetComponent<NetworkIdentity>().connectionToClient, newPlayer);
        Object.Destroy(obj);
    }
}