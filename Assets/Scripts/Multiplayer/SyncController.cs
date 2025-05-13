using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using System;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Player))]
[RequireComponent(typeof(PlayerInput))]
public class SyncController : NetworkBehaviour
{
    [SerializeField] private PlayerCameraTarget _target;
    [SerializeField] private HeadHider _head;

    private void Start()
    {
        if (isLocalPlayer)
            Sync();
    }

    private void Sync()
    {
        GetComponent<CharacterController>().enabled = true;
        GetComponent<Player>().enabled = true;
        GetComponent<PlayerInput>().enabled = true;

        CinemachineVirtualCamera cinemachineVirtualCamera = FindCinemachine();
        cinemachineVirtualCamera.Follow = _target.transform;
        _head.Hide();
    }

    private CinemachineVirtualCamera FindCinemachine()
    {
        CinemachineVirtualCamera cinemachineVirtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

        if (cinemachineVirtualCamera == null)
            throw new NullReferenceException($"Не удалось найти компонент {typeof(CinemachineVirtualCamera)} на сцене");

        return cinemachineVirtualCamera;
    }
}