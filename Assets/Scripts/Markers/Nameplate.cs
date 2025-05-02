using Mirror;
using UnityEngine;

public class Nameplate : NetworkBehaviour
{
    private Transform _target;

    private void Awake() =>
        _target = Camera.main.transform;

    private void Update() =>
        RotateToCamera();

    private void RotateToCamera()
    {
        if (isLocalPlayer == false)
            transform.LookAt(_target);
    }
}