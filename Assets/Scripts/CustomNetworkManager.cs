using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public event Action ServerStarted;
    public event Action ServerStoped;
    public event Action ClientStarted;
    public event Action ClientStoped;

    public override void OnStartServer() => ServerStarted?.Invoke();

    public override void OnStartClient() => ClientStarted?.Invoke();

    public override void OnStopServer() => ServerStoped?.Invoke();

    public override void OnStopClient() => ClientStoped?.Invoke();
}