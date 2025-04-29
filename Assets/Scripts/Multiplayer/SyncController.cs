using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using System;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(ThirdPersonController))]
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

            ThirdPersonController ThirdPersonController = GetComponent<ThirdPersonController>();
            ThirdPersonController.enabled = true;

            PlayerInput PlayerInput = GetComponent<PlayerInput>();
            PlayerInput.enabled = true;

            PlayerFollowCamera foolowCamera = FindObjectOfType<PlayerFollowCamera>();

            if (foolowCamera == null)
                throw new NullReferenceException($"Не удалось найти компонент {typeof(PlayerFollowCamera)} на сцене");

            CinemachineVirtualCamera CinemachineVirtualCamera = foolowCamera.GetComponent<CinemachineVirtualCamera>();
            CinemachineVirtualCamera.Follow = _target.transform;
        }
    }
}