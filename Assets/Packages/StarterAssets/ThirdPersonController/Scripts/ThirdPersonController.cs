using System;
using UnityEngine;

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private float _speedChangeRate = 10.0f;
        [SerializeField] private AudioClip _landingAudioClip;
        [SerializeField] private AudioClip[] _footstepAudioClips;
        [SerializeField][Range(0, 1)] private float _footstepAudioVolume = 0.5f;
        [SerializeField] private LayerMask _groundLayers;

        private Mover _mover;
        private CameraRotator _rotator;
        private Jumper _jumper;
        private PlayerAnimator _animatorWrapper;

        private float _animationBlend;

        private CharacterController _controller;
        private StarterAssetsInputs _input;

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            _mover = new(_controller, _input);
            _rotator = new(_input, transform);
            _jumper = new(_input, transform, _groundLayers); 
            _animatorWrapper = new(transform);
        }

        private void Update()
        {
            ProcessJump();
            CheckGrounded();
            Move();
        }

        private void LateUpdate() =>
            _rotator.RotateCamera();

        private void OnEnable()
        {
            _jumper.JumpAppeared += OnJumpAppered;
            _jumper.FallsFreely += OnFallsFreely;
            _jumper.JumpCompleted += OnJumpCompleted;
        }

        private void OnDisable()
        {
            _jumper.JumpAppeared -= OnJumpAppered;
            _jumper.FallsFreely -= OnFallsFreely;
            _jumper.JumpCompleted -= OnJumpCompleted;
        }

        private void OnJumpAppered() =>
            _animatorWrapper.EnableJump();

        private void OnFallsFreely() =>
            _animatorWrapper.EnableFreeFall();

        private void OnJumpCompleted()
        {
            _animatorWrapper.DisableJump();
            _animatorWrapper.DisableFreeFall();
        }

        private void CheckGrounded()
        {
            _jumper.CheckGrounded();
            _animatorWrapper.SetGrounded(_jumper.IsGrounded);
        }

        private void Move()
        {
            _mover.Move(_jumper.VerticalVelocity);
            _animationBlend = Mathf.Lerp(_animationBlend, _mover.TargetSpeed, Time.deltaTime * _speedChangeRate);

            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            _animatorWrapper.SetSpeed(_animationBlend);
            _animatorWrapper.SetMotionSpeed(_mover.InputMagnitude);
        }

        private void ProcessJump() =>
            _jumper.ProcessJump();

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