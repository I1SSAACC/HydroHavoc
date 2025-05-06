using UnityEngine;

public class PlayerAnimator
{
    private const string MotionSpeed = nameof(MotionSpeed);
    private const string SideSpeed = nameof(SideSpeed);
    private const string IsGrounded = nameof(IsGrounded);
    private const string IsJump = nameof(IsJump);
    private const string IsFreeFall = nameof(IsFreeFall);
    private const string IsCrouching = nameof(IsCrouching);

    private readonly Animator _animator;
    private readonly DeltaMovementCalculator _deltaCalculator;
    private readonly MovementAnimationSmoother _smoother = new();

    private int _animIDMotionSpeed;
    private int _animIDSideSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDCrouching;

    private readonly bool _isSmooth = true;

    public PlayerAnimator(Transform player)
    {
        _animator = player.GetComponentInChildren<Animator>(true);
        AssignAnimationIDs();

        _deltaCalculator = new(player);
    }

    private void AssignAnimationIDs()
    {
        _animIDMotionSpeed = Animator.StringToHash(MotionSpeed);
        _animIDSideSpeed = Animator.StringToHash(SideSpeed);
        _animIDGrounded = Animator.StringToHash(IsGrounded);
        _animIDJump = Animator.StringToHash(IsJump);
        _animIDFreeFall = Animator.StringToHash(IsFreeFall);
        _animIDCrouching = Animator.StringToHash(IsCrouching);
    }

    public void UpdateSpeedMovement()
    {
        Vector2 inputs = _deltaCalculator.GetNormalizedDelta();

        if (_isSmooth)
            inputs = _smoother.CalculateMovementSmoothedValue(inputs);

        SetSpeed(inputs);
    }

    public void SetGrounded(bool isGrounded) =>
        _animator.SetBool(_animIDGrounded, isGrounded);

    public void EnableJump() =>
        _animator.SetBool(_animIDJump, true);

    public void DisableJump() =>
        _animator.SetBool(_animIDJump, false);

    public void EnableFreeFall() =>
        _animator.SetBool(_animIDFreeFall, true);

    public void DisableFreeFall() =>
        _animator.SetBool(_animIDFreeFall, false);

    public void EnableCrouching() =>
        _animator.SetBool(_animIDCrouching, true);

    public void DisableCrouching() =>
        _animator.SetBool(_animIDCrouching, false);

    private void SetSpeed(Vector2 value)
    {
        _animator.SetFloat(_animIDSideSpeed, value.x);
        _animator.SetFloat(_animIDMotionSpeed, value.y);
    }
}