using Mirror;
using UnityEngine;

public class DisplayRotator : NetworkBehaviour
{
    private Transform _target;

    private void Awake() =>
        _target = Camera.main.transform;

    private void Update() =>
        RotateToCamera();

    private void RotateToCamera()
    {
        if (isLocalPlayer)
            gameObject.SetActive(false);
        else
            transform.LookAt(_target);
    }
}