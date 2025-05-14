using Mirror;
using System;
using UnityEngine;

public class CustomNetworkManager : NetworkManager
{
    public event Action ServerStarted;
    public event Action ServerStopped;
    public event Action ClientStarted;
    public event Action ClientStopped;

    private static CustomNetworkManager _instance;

    public static CustomNetworkManager Instance
    {
        get
        {
            if (_instance == null)
                Debug.LogWarning("Создали синглтон");

            return _instance;
        }
    }

    public override void Awake()
    {
        base.Awake();

        if (_instance != null && _instance != this)
        {
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public override void OnClientDisconnect()
    {
        Debug.LogWarning("Клиент откючился");
    }

    public override void OnStartClient()
    {
        Debug.LogWarning("Старт клиента");
        ClientStarted?.Invoke();
    }

    public override void OnStopClient()
    {
        Debug.LogWarning("OnStopClient()");
        ClientStopped?.Invoke();
    }
}