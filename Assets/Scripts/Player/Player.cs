using System;
using UnityEngine;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    [RequireComponent(typeof(StarterAssetsInputs))]
    public class Player : MonoBehaviour
    {
        [SerializeField] private AudioClip _landingAudioClip;
        [SerializeField] private AudioClip[] _footstepAudioClips;
        [SerializeField][Range(0, 1)] private float _footstepAudioVolume = 0.5f;

        private PlayerMover _mover;
        private PlayerRotator _rotator;
        private PlayerJumper _jumper;
        private PlayerAnimator _animator;
        private PlayerCroucher _croucher;

        private CharacterController _controller;
        private StarterAssetsInputs _input;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            _mover = new(transform);
            _rotator = new(_input, transform);
            _jumper = new(_mover);
            _animator = new(transform);
            _croucher = new(transform);
        }

        private void Update()
        {
            Move();

            _animator.SetGrounded(_controller.isGrounded);

            if (_controller.isGrounded)
            {
                _animator.DisableJump();
                _animator.DisableFreeFall();
            }
            else
                _animator.EnableFreeFall();
        }

        private void LateUpdate()
        {
            _animator.UpdateSpeedMovement();
            _rotator.RotateCamera();
        }

        private void OnEnable()
        {
            _input.JumpPressed += OnJumpPressed;
            _input.CrouchPressed += OnCrouchPressed;
            _input.CrouchUnpressed += OnCrouchUnpressed;
        }

        private void OnDisable()
        {
            _input.JumpPressed -= OnJumpPressed;
            _input.CrouchPressed -= OnCrouchPressed;
            _input.CrouchUnpressed -= OnCrouchUnpressed;
        }

        private void OnJumpPressed()
        {
            if (_controller.isGrounded == false)
                return;

            _jumper.Jump();
            _animator.EnableJump();
        }

        private void OnCrouchPressed()
        {
            _croucher.EnableCrouching();
            _animator.EnableCrouching();
        }

        private void OnCrouchUnpressed()
        {
            _croucher.DisableCrouching();
            _animator.DisableCrouching();
        }

        private void Move()
        {
            Vector2 input = _input.Move;

            if (_croucher.IsCrouching && _controller.isGrounded)
                input *= PlayerParams.CrouchingStepMultiplierSpeed;
            else if (_input.IsWalking)
                input *= PlayerParams.SlowingStepMultiplierSpeed;

            _mover.Move(input);            
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight < 0.5f)
                return;

            int index = UnityEngine.Random.Range(0, _footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(_controller.transform.position), _footstepAudioVolume);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                AudioSource.PlayClipAtPoint(_landingAudioClip, transform.TransformPoint(_controller.transform.position), _footstepAudioVolume);
        }
    }
}