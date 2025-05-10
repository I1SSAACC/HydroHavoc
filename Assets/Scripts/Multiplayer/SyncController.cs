using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;
using System;
using UnityEditor;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(Player))]
    [RequireComponent(typeof(PlayerInput))]
    public class SyncController : NetworkBehaviour
    {
        [SerializeField] private PlayerCameraTarget _target;
        [SerializeField] private EyesLookToCamera _eyes;
        [SerializeField] private HeadHider _head;

        [SerializeField] private GameObject PrefabCanvas;

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

            CinemachineVirtualCamera cinemachineVirtualCamera = FindFirstObjectByType<CinemachineVirtualCamera>();

            if (cinemachineVirtualCamera == null)
                throw new NullReferenceException($"Не удалось найти компонент {typeof(CinemachineVirtualCamera)} на сцене");

            cinemachineVirtualCamera.Follow = _target.transform;

            CameraIsHer CameraIsHer = cinemachineVirtualCamera.GetComponent<CameraIsHer>();

            _eyes.Follow(CameraIsHer.Camera);

            _head.Hide();
        }
    }
}