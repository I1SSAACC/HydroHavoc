using System;
using UnityEngine;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(InputInformer))]
public class Player : MonoBehaviour
{
    [SerializeField] private AudioClip _landingAudioClip;
    [SerializeField] private AudioClip[] _footstepAudioClips;
    [SerializeField][Range(0, 1)] private float _footstepAudioVolume = 0.5f;
    [SerializeField] private LayerMask _groundLayer;

    private PlayerAnimator _animator;
    private PlayerMover _mover;
    private PlayerRotator _rotator;
    private Jumper _jumper;
    private Croucher _croucher;

    private CharacterController _controller;
    private InputInformer _input;

    private void Awake()
    {
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<InputInformer>();

        _mover = new(transform);
        _rotator = new(_input, transform);
        _jumper = new(_mover);
        _animator = new(transform);
        _croucher = new(transform);
    }

    private void Update()
    {
        Move();
        bool isGrounded = IsGrounded();
        _animator.SetGrounded(isGrounded);
        if (isGrounded)
        {
            _animator.DisableJump();
            _animator.DisableFreeFall();
        }
        else
        {
            _animator.EnableFreeFall();
        }
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
        if (!IsGrounded())
            return;

        _jumper.Jump();
        _animator.EnableJump();
    }

    private void OnCrouchPressed()
    {
        _croucher.Crouch();
        _animator.EnableCrouching();
    }

    private void OnCrouchUnpressed()
    {
        _croucher.StandUp();
        _animator.DisableCrouching();
    }

    private void Move()
    {
        Vector2 input = _input.Move;

        if (_croucher.IsCrouch && IsGrounded())
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

    private bool IsGrounded() =>
        Physics.Raycast(transform.position, Vector3.down, PlayerParams.GroundCheckDistance, _groundLayer) || _controller.isGrounded;
}