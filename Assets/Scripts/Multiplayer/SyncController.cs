using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Mirror;
using Cinemachine;

namespace StarterAssets
{
	public class SyncController : NetworkBehaviour
	{
		public Transform Target;

		void Start()
		{
			if (isLocalPlayer)
			{
				CharacterController CharacterController = GetComponent<CharacterController>();
                CharacterController.enabled = true;

				ThirdPersonController ThirdPersonController = GetComponent<ThirdPersonController>();
				ThirdPersonController.enabled = true;

				PlayerInput PlayerInput = GetComponent<PlayerInput>();
				PlayerInput.enabled = true;

				GameObject PlayerFollowCamera = GameObject.Find("PlayerFollowCamera");

				CinemachineVirtualCamera CinemachineVirtualCamera = PlayerFollowCamera.GetComponent<CinemachineVirtualCamera>();
				CinemachineVirtualCamera.Follow = Target;
			}
		}
	}
}
