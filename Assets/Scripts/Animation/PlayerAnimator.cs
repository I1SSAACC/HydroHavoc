using UnityEngine;
public class PlayerAnimator
{
    private const string Speed = nameof(Speed);
    private const string Grounded = nameof(Grounded);
    private const string Jump = nameof(Jump);
    private const string FreeFall = nameof(FreeFall);
    private const string MotionSpeed = nameof(MotionSpeed);

    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private readonly Animator _animator;

    public PlayerAnimator(Transform player)
    {
        AssignAnimationIDs();
        _animator = player.GetComponentInChildren<Animator>(true);
    }

    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash(Speed);
        _animIDMotionSpeed = Animator.StringToHash(MotionSpeed);
        _animIDGrounded = Animator.StringToHash(Grounded);
        _animIDJump = Animator.StringToHash(Jump);
        _animIDFreeFall = Animator.StringToHash(FreeFall);
    }

    public void SetSpeed(float value) =>
        _animator.SetFloat(_animIDSpeed, value);

    public void SetMotionSpeed(float value) =>
        _animator.SetFloat(_animIDMotionSpeed, value);

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
}