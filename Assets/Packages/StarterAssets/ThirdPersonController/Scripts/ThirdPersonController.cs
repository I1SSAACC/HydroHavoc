using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        [SerializeField] private Mover _mover;
        [SerializeField] private float _rotationSpeed = 1.0f;

        [SerializeField][Range(0.0f, 0.3f)] private float _rotationSmoothTime = 0.12f;
        [SerializeField] private float _speedChangeRate = 10.0f;

        [SerializeField] private AudioClip _landingAudioClip;
        [SerializeField] private AudioClip[] _footstepAudioClips;
        [SerializeField][Range(0, 1)] private float _footstepAudioVolume = 0.5f;

        [Space(10)]
        [SerializeField] private float _jumpHeight = 1.2f;
        [SerializeField] private float _gravity = -10.0f;
        [SerializeField] private float _jumpTimeout = 0.50f;
        [SerializeField] private float _fallTimeout = 0.15f;

        [Header("Grounded")]
        [SerializeField] private bool _isGrounded = true;
        [SerializeField] private float _groundedOffset = -0.14f;
        [SerializeField] private float _groundedRadius = 0.28f;
        [SerializeField] private LayerMask _groundLayers;

        [Header("Cinemachine")]
        [SerializeField] private PlayerCameraTarget _cameraTarget;
        [SerializeField] private Vector2 _rotationVerticalLimits = new(-30f, 70f);
        [SerializeField] private float _cameraAngleOverride = 0.0f;

        [SerializeField] private bool _lockCameraPosition = false;

        private AnimatorWrapper _animatorWrapper;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _animationBlend;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

#if ENABLE_INPUT_SYSTEM
        private PlayerInput _playerInput;
#endif        
        private CharacterController _controller;
        private StarterAssetsInputs _input;

        private const float _threshold = 0.01f;

        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }

        private void Awake()
        {
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            _mover = new(_input, _controller);
            _animatorWrapper = new(transform);
        }

        private void Start()
        {
            _cinemachineTargetYaw = _cameraTarget.transform.rotation.eulerAngles.y;

#if ENABLE_INPUT_SYSTEM
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Пакет Starter Assets не имеет зависимостей. Используйте Tools/Starter Assets/Reinstall Dependencies, чтобы исправить это");
#endif
            ResetTimeoutOnStart();
        }

        private void Update()
        {
            JumpAndGravity();
            CheckGrounded();
            Move();
        }

        private void LateUpdate() =>
            CameraRotation();

        private void ResetTimeoutOnStart()
        {
            _jumpTimeoutDelta = _jumpTimeout;
            _fallTimeoutDelta = _fallTimeout;
        }

        private void CheckGrounded()
        {
            Vector3 spherePosition = new(
                transform.position.x,
                transform.position.y - _groundedOffset,
                transform.position.z);

            _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

            _animatorWrapper.SetGrounded(_isGrounded);
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && _lockCameraPosition == false)
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;
                deltaTimeMultiplier *= _rotationSpeed;

                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, _rotationVerticalLimits.x, _rotationVerticalLimits.y);

            _cameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + _cameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
        }

        private void Move()
        {
            _mover.Move(_verticalVelocity);
            _animationBlend = Mathf.Lerp(_animationBlend, _mover.TargetSpeed, Time.deltaTime * _speedChangeRate);

            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            _animatorWrapper.SetSpeed(_animationBlend);
            _animatorWrapper.SetMotionSpeed(_mover.InputMagnitude);
        }

        private void JumpAndGravity()
        {
            if (_isGrounded)
            {
                _fallTimeoutDelta = _fallTimeout;

                _animatorWrapper.DisableJump();
                _animatorWrapper.DisableFreeFall();

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

                    _animatorWrapper.EnableJump();
                }

                if (_jumpTimeoutDelta >= 0.0f)
                    _jumpTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                _jumpTimeoutDelta = _jumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                    _fallTimeoutDelta -= Time.deltaTime;
                else
                    _animatorWrapper.EnableFreeFall();

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)
                _verticalVelocity += _gravity * Time.deltaTime;
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f)
                lfAngle += 360f;

            if (lfAngle > 360f)
                lfAngle -= 360f;

            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                return;

            if (_footstepAudioClips.Length <= 0)
                return;

            var index = Random.Range(0, _footstepAudioClips.Length);
            AudioSource.PlayClipAtPoint(_footstepAudioClips[index], transform.TransformPoint(_controller.center), _footstepAudioVolume);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
                AudioSource.PlayClipAtPoint(_landingAudioClip, transform.TransformPoint(_controller.center), _footstepAudioVolume);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = _isGrounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(new(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
        }
    }
}