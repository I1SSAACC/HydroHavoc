using UnityEngine;
using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public Action onStartServer, onStopServer, onStartClient, onStopClient;
    public override void OnStartServer() => onStartServer?.Invoke();
    public override void OnStartClient() => onStartClient?.Invoke();
    public override void OnStopServer() => onStopServer?.Invoke();
    public override void OnStopClient() => onStopClient?.Invoke();
}
