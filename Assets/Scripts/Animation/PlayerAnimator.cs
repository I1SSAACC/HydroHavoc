using UnityEngine;
public class PlayerAnimator
{
    private const float SpeedSmooth = 1.5f;

    private const string MotionSpeed = nameof(MotionSpeed);
    private const string SideSpeed = nameof(SideSpeed);
    private const string Grounded = nameof(Grounded);
    private const string Jump = nameof(Jump);
    private const string FreeFall = nameof(FreeFall);

    private int _animIDMotionSpeed;
    private int _animIDSideSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;

    private readonly Animator _animator;
    private readonly DeltaMovementCalculator _deltaCalculator;
    private readonly Smoother _smoother;

    public PlayerAnimator(Transform player)
    {
        _animator = player.GetComponentInChildren<Animator>(true);
        AssignAnimationIDs();

        _deltaCalculator = new(player);
        _smoother = new(SpeedSmooth);
    }

    private void AssignAnimationIDs()
    {
        _animIDMotionSpeed = Animator.StringToHash(MotionSpeed);
        _animIDSideSpeed = Animator.StringToHash(SideSpeed);
        _animIDGrounded = Animator.StringToHash(Grounded);
        _animIDJump = Animator.StringToHash(Jump);
        _animIDFreeFall = Animator.StringToHash(FreeFall);
    }

    public void UpdateSpeedMovement()
    {
        Vector2 inputs = _deltaCalculator.GetNormalizedDelta();
        inputs = _smoother.GetSmoothValue(inputs);
        SetSpeed(inputs);

        Debug.Log($"Smooth inputs = {inputs}");
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

    private void SetSpeed(Vector2 value)
    {
        _animator.SetFloat(_animIDSideSpeed, value.x);
        _animator.SetFloat(_animIDMotionSpeed, value.y);
    }
}
