using Mirror;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using System;
using static UnityEditor.Progress;

public class CNetworkManager : NetworkManager
{
    public static CNetworkManager CustomNetworkManager;

    [Scene]
    [SerializeField]
    private string Game;

    public GameObject Hero;

    private int SceneIndecs;

    private Scene StartGameScene;

    public int MaxPlayerTeams;

    public List<GameObject> ListTeamRed = new List<GameObject>();
    public List<GameObject> ListTeamBlue = new List<GameObject>();

    public override void Awake()
    {
        base.Awake();
        CustomNetworkManager = this;
    }

    public override void OnClientDisconnect() =>
        base.OnClientDisconnect();

    public override void OnStartServer()
    {
        base.OnStartServer();
        NetworkServer.RegisterHandler<SearchMessage>(SpawnAcaunt);
    }

    public override void OnClientConnect()
    {
        base.OnClientConnect();
        NetworkClient.Send(default(SearchMessage));
    }

    public void SpawnAcaunt(NetworkConnectionToClient conn, SearchMessage Send)
    {
        GameObject gameObject = UnityEngine.Object.Instantiate(playerPrefab);
        NetworkServer.AddPlayerForConnection(conn, gameObject);
    }

    public void TeamCheck(GameObject Heros)
    {
        if (ListTeamRed.Count == MaxPlayerTeams)
            ListTeamBlue.Add(Heros);
        else
            ListTeamRed.Add(Heros);
        if (ListTeamBlue.Count == MaxPlayerTeams)
            OnClientStartSearching();

        ListTeamRed = ListTeamRed.FindAll((GameObject x) => x != null);
        ListTeamBlue = ListTeamBlue.FindAll((GameObject x) => x != null);
    }
    public void RemoveFromTeam(GameObject player)
    {
        if (ListTeamRed.Contains(player))
            ListTeamRed.Remove(player);
        else if (ListTeamBlue.Contains(player))
            ListTeamBlue.Remove(player);
    }

    public void OnClientStartSearching()
    {
        if (string.IsNullOrEmpty(Game))
        {
            Debug.LogError("Game scene name is not set.");
            return;
        }

        SceneManager.LoadSceneAsync(Game, new LoadSceneParameters
        {
            loadSceneMode = LoadSceneMode.Additive,
            localPhysicsMode = LocalPhysicsMode.Physics3D
        });

        SceneIndecs++;
        StartGameScene = SceneManager.GetSceneAt(SceneIndecs);

        foreach (GameObject item in ListTeamBlue)
        {
            if (item == null)
            {
                Debug.LogWarning("Item in ListTeamBlue is null.");
                continue;
            }

            var networkIdentity = item.GetComponent<NetworkIdentity>();
            if (networkIdentity == null || networkIdentity.connectionToClient == null)
            {
                Debug.LogWarning("NetworkIdentity or connectionToClient is null for item in ListTeamBlue.");
                continue;
            }

            networkIdentity.connectionToClient.Send(new SceneMessage
            {
                sceneName = Game
            });

            GameObject obj = item;
            GameObject gameObject = UnityEngine.Object.Instantiate(Hero, GetStartPosition().position, Quaternion.identity);
            gameObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(gameObject, StartGameScene);
            NetworkServer.ReplacePlayerForConnection(networkIdentity.connectionToClient, gameObject);
            UnityEngine.Object.Destroy(obj);
        }

        foreach (GameObject item2 in ListTeamRed)
        {
            if (item2 == null)
            {
                Debug.LogWarning("Item in ListTeamBlue is null.");
                continue;
            }

            var networkIdentity = item2.GetComponent<NetworkIdentity>();
            if (networkIdentity == null || networkIdentity.connectionToClient == null)
            {
                Debug.LogWarning("NetworkIdentity or connectionToClient is null for item in ListTeamBlue.");
                continue;
            }

            networkIdentity.connectionToClient.Send(new SceneMessage
            {
                sceneName = Game
            });

            GameObject obj = item2;
            GameObject gameObject = UnityEngine.Object.Instantiate(Hero, GetStartPosition().position, Quaternion.identity);
            gameObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(gameObject, StartGameScene);
            NetworkServer.ReplacePlayerForConnection(networkIdentity.connectionToClient, gameObject);
            UnityEngine.Object.Destroy(obj);
        }

        ListTeamRed.Clear();
        ListTeamBlue.Clear();
    }
}