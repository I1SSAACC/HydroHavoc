using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using System;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerInput))]
    public class SyncController : NetworkBehaviour
    {
        [SerializeField] private PlayerCameraTarget _target;

        private void Start()
        {
            if (isLocalPlayer)
                Sync();
        }

        private void Sync()
        {
            CharacterController CharacterController = GetComponent<CharacterController>();
            CharacterController.enabled = true;

            Player ThirdPersonController = GetComponent<Player>();
            ThirdPersonController.enabled = true;

            PlayerInput PlayerInput = GetComponent<PlayerInput>();
            PlayerInput.enabled = true;

            CinemachineVirtualCamera cinemachineVirtualCamera = FindAnyObjectByType<CinemachineVirtualCamera>();

            if (cinemachineVirtualCamera == null)
                throw new NullReferenceException($"Не удалось найти компонент {typeof(CinemachineVirtualCamera)} на сцене");

            cinemachineVirtualCamera.Follow = _target.transform;
        }
    }
}