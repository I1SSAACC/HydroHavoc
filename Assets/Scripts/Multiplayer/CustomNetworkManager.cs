using Mirror;
using System;

public class CustomNetworkManager : NetworkManager
{
    public event Action ServerStarted;
    public event Action ServerStopped;
    public event Action ClientStarted;
    public event Action ClientStopped;

    public override void OnStartServer() => ServerStarted?.Invoke();

    public override void OnStartClient() => ClientStarted?.Invoke();

    public override void OnStopServer() => ServerStopped?.Invoke();

    public override void OnStopClient() => ClientStopped?.Invoke();
}
