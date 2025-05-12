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
    [SerializeField] private int requiredPlayers;
    [SerializeField] private GameObject playerPrefab;
    [SerializeField] private GameObject uiCanvas;
    private List<PlayerInfo> playersInQueue = new List<PlayerInfo>();

    private void Start()
    {
        CustomNetworkManager netMan = NetworkManager.singleton as CustomNetworkManager;
        netMan.ServerStarted += OnStartServer;
        netMan.ServerStoped += OnStopServer;
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
        PlayerInfo playerInfo = new PlayerInfo() { connection = conn };
        playersInQueue.Add(playerInfo);

        if (playersInQueue.Count >= requiredPlayers)
            StartMatch();
    }

    private void OnLeaveMatchmakingMessageReceived(NetworkConnectionToClient conn, LeaveMatchmakingMessage _)
    {
        PlayerInfo playerToRemove = playersInQueue.Find(p => p.connection == conn);
        if (playerToRemove.connection != null)
            playersInQueue.Remove(playerToRemove);
    }

    private void StartMatch()
    {
        NetworkConnectionToClient[] connections = new NetworkConnectionToClient[playersInQueue.Count];
        for (int i = 0; i < playersInQueue.Count; i++)
            connections[i] = playersInQueue[i].connection;

        playersInQueue.Clear();
        StartCoroutine(CreateMatch(connections));
    }

    private IEnumerator CreateMatch(NetworkConnectionToClient[] conns)
    {
        yield return SceneManager.LoadSceneAsync("Playground", new LoadSceneParameters(LoadSceneMode.Additive));

        foreach (var conn in conns)
        {
            GameObject oldPlayer = conn.identity?.gameObject;

            GameObject newPlayer = Instantiate(playerPrefab);
            SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));

            if (oldPlayer != null)
                NetworkServer.Destroy(oldPlayer);

            NetworkServer.ReplacePlayerForConnection(conn, newPlayer, ReplacePlayerOptions.KeepAuthority);
        }

        foreach (var conn in conns)
        {
            var playerObject = conn.identity?.gameObject;
            if (playerObject != null && uiCanvas != null)
            {
                uiCanvas.SetActive(false);
                GameObject.Find("MainCamera").GetComponent<AudioListener>().enabled = false;
            }
        }
    }

    private void OnDestroy()
    {
        CustomNetworkManager netMan = NetworkManager.singleton as CustomNetworkManager;
        if (netMan == null) return;
        netMan.ServerStarted -= OnStartServer;
        netMan.ServerStoped -= OnStopServer;
    }
}
