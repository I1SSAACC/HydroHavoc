using StarterAssets;
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
        private const string Speed = nameof(Speed);
        private const string Grounded = nameof(Grounded);
        private const string Jump = nameof(Jump);
        private const string FreeFall = nameof(FreeFall);
        private const string MotionSpeed = nameof(MotionSpeed);


        [Header("Player")]
        [SerializeField] private float _moveSpeed = 2.0f;
        [SerializeField] private float _rotationSpeed = 1.0f;
        [SerializeField] private float _sprintSpeed = 5.335f;

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

        private Mover _mover;

        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        private float _speedMovement;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private Transform _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;

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
            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();

            _mover = new(_input, _controller, _moveSpeed, _sprintSpeed);
            _mainCamera = Camera.main.transform;
        }

        private void Start()
        {
            _cinemachineTargetYaw = _cameraTarget.transform.rotation.eulerAngles.y;            

#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
			Debug.LogError( "Пакет Starter Assets не имеет зависимостей. Используйте Tools/Starter Assets/Reinstall Dependencies, чтобы исправить это");
#endif

            AssignAnimationIDs();
            ResetTimeoutOnStart();
        }

        private void Update()
        {
            if (_animator == null)
                _hasAnimator = TryGetComponent(out _animator);

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

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash(Speed);
            _animIDGrounded = Animator.StringToHash(Grounded);
            _animIDJump = Animator.StringToHash(Jump);
            _animIDFreeFall = Animator.StringToHash(FreeFall);
            _animIDMotionSpeed = Animator.StringToHash(MotionSpeed);
        }

        private void CheckGrounded()
        {
            Vector3 spherePosition = new Vector3(
                transform.position.x,
                transform.position.y - _groundedOffset,
                transform.position.z);

            _isGrounded = Physics.CheckSphere(spherePosition, _groundedRadius, _groundLayers, QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
                _animator.SetBool(_animIDGrounded, _isGrounded);
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
            //float targetSpeed = _input.IsSprint ? _moveSpeed : _sprintSpeed;

            //if (_input.move == Vector2.zero)
            //    targetSpeed = 0.0f;

            //float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            //float speedOffset = 0.1f;
            //float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

            //if (currentHorizontalSpeed < targetSpeed - speedOffset ||
            //    currentHorizontalSpeed > targetSpeed + speedOffset)
            //{
            //    _speedMovement = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude, Time.deltaTime * _speedChangeRate);
            //    _speedMovement = Utils.RoundThreeDecimalPlaces(_speedMovement);
            //}
            //else
            //{
            //    _speedMovement = targetSpeed;
            //}

            //Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

            //_targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _mainCamera.transform.eulerAngles.y;
            //float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity, _rotationSmoothTime);
            //transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);

            //Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            //_controller.Move(targetDirection.normalized * (_speedMovement * Time.deltaTime) + new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
            _mover.Move(_input.move, _verticalVelocity);
            _animationBlend = Mathf.Lerp(_animationBlend, _mover.TargetSpeed, Time.deltaTime * _speedChangeRate);

            if (_animationBlend < 0.01f)
                _animationBlend = 0f;

            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, _mover.InputMagnitude);
            }
        }

        private void JumpAndGravity()
        {
            if (_isGrounded)
            {
                _fallTimeoutDelta = _fallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                    _verticalVelocity = -2f;

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(_jumpHeight * -2f * _gravity);

                    if (_hasAnimator)
                        _animator.SetBool(_animIDJump, true);
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
                {
                    if (_hasAnimator)
                        _animator.SetBool(_animIDFreeFall, true);
                }

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
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            Gizmos.color = _isGrounded ? transparentGreen : transparentRed;
            Gizmos.DrawSphere(new Vector3(transform.position.x, transform.position.y - _groundedOffset, transform.position.z), _groundedRadius);
        }
    }
}

public class Mover
{
    private const float SpeedChangeRate = 10.0f;

    private readonly CharacterController _controller;
    private readonly StarterAssetsInputs _input;
    private readonly float _moveSpeed;
    private readonly float _sprintSpeed;
    private readonly Transform _camera;

    private float _targetSpeed;
    private float _inputMagnitude;

    public Mover(StarterAssetsInputs input, CharacterController controller, float moveSpeed, float sprintSpeed)
    {
        _input = input;
        _controller = controller;
        _moveSpeed = moveSpeed;
        _sprintSpeed = sprintSpeed;
        _camera = Camera.main.transform;
    }

    public float TargetSpeed => _targetSpeed;

    public float InputMagnitude => _inputMagnitude;

    public void Move(Vector2 direction, float verticalVelocity)
    {
        _targetSpeed = _input.IsSprint ? _moveSpeed : _sprintSpeed;

        if (_input.move == Vector2.zero)
            _targetSpeed = 0.0f;

        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
        float speedOffset = 0.1f;
        _inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        float speedMovement;

        if (currentHorizontalSpeed < _targetSpeed - speedOffset ||
            currentHorizontalSpeed > _targetSpeed + speedOffset)
        {
            speedMovement = Mathf.Lerp(currentHorizontalSpeed, _targetSpeed * _inputMagnitude, Time.deltaTime * SpeedChangeRate);
            speedMovement = Utils.RoundThreeDecimalPlaces(speedMovement);
        }
        else
        {
            speedMovement = _targetSpeed;
        }

        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        float targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _camera.eulerAngles.y;
        //float rotation = Mathf.SmoothDampAngle(_controller.transform.eulerAngles.y, targetRotation, ref _rotationVelocity, 0);
        _controller.transform.rotation = Quaternion.Euler(0.0f, targetRotation, 0.0f);

        Vector3 targetDirection = Quaternion.Euler(0.0f, targetRotation, 0.0f) * Vector3.forward;

        _controller.Move(targetDirection.normalized * (speedMovement * Time.deltaTime) + new Vector3(0.0f, verticalVelocity, 0.0f) * Time.deltaTime);
    }
}