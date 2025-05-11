using Mirror;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.SceneManagement;

public struct MatchmakingMessage : NetworkMessage{}
public struct LeaveMatchmakingMessage : NetworkMessage{}

public struct PlayerInfo{public NetworkConnectionToClient connection;}

public class MatchManager : MonoBehaviour
{
    [SerializeField] private int requiredPlayers = 1;
    private List<PlayerInfo> playersInQueue = new List<PlayerInfo>();

    private void Start()
    {
        CustomNetworkManager netMan = NetworkManager.singleton as CustomNetworkManager;
        netMan.onStartServer += OnStartServer;
        netMan.onStopServer += OnStopServer;
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

    private void OnMatchmakingMessageReceived(NetworkConnectionToClient conn, MatchmakingMessage message)
    {
        PlayerInfo playerInfo = new PlayerInfo() { connection = conn };
        playersInQueue.Add(playerInfo);

        if (playersInQueue.Count >= requiredPlayers)
            StartMatch();
    }

    private void OnLeaveMatchmakingMessageReceived(NetworkConnectionToClient conn, LeaveMatchmakingMessage message)
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
            GameObject newPlayer = Instantiate(NetworkManager.singleton.playerPrefab);
            SceneManager.MoveGameObjectToScene(newPlayer, SceneManager.GetSceneAt(SceneManager.loadedSceneCount - 1));
            NetworkServer.AddPlayerForConnection(conn, newPlayer);
        }
    }

    private void OnDestroy()
    {
        CustomNetworkManager netMan = NetworkManager.singleton as CustomNetworkManager;
        if (netMan == null) return;
        netMan.onStartServer -= OnStartServer;
        netMan.onStopServer -= OnStopServer;
    }
}
