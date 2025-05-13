using System.Collections;
using UnityEngine;

public class Croucher
{
    private readonly Transform _player;
    private readonly CharacterController _controller;
    private readonly MonoBehaviour _monoBehaviour;

    private float _standingHeight;
    private float _crouchingHeight;

    private Vector3 _standingCenter;
    private Vector3 _crouchingCenter;

    private Coroutine _crouchCoroutine;
    private float _heightVelocity;
    private Vector3 _centerVelocity;

    private bool _isCrouch;
    private float _smoothTime;

    public Croucher(Transform transform)
    {
        _player = transform;
        _controller = transform.GetComponent<CharacterController>();
        _monoBehaviour = transform.GetComponent<MonoBehaviour>();

        InitializeParameters();
    }

    public bool IsCrouch => _isCrouch;

    private void InitializeParameters()
    {
        _smoothTime = PlayerParams.CrouchSmoothTime;
        _standingHeight = _controller.height;
        _standingCenter = _controller.center;

        _crouchingHeight = _standingHeight * PlayerParams.CrouchHeightMultiplier;
        _crouchingCenter = CalculateCrouchingCenter();
    }

    private Vector3 CalculateCrouchingCenter()
    {
        float yOffset = (_standingHeight - _crouchingHeight) * 0.5f;
        return _standingCenter + Vector3.down * yOffset;
    }

    public void Crouch()
    {
        if (_isCrouch)
            return;

        _isCrouch = true;
        StartCrouchRoutine(true);
    }


    public void StandUp()
    {
        if (_isCrouch == false)
            return;

        if (IsHitCeiling())
        {
            Debug.Log("Can't stand up - obstacle above!");
            return;
        }

        _isCrouch = false;
        StartCrouchRoutine(false);
    }

    private bool IsHitCeiling()
    {
        float rayLength = _standingHeight - _crouchingHeight;
        Vector3 rayStart = _player.position + _controller.center;
        return Physics.Raycast(rayStart, Vector3.up, rayLength);
    }

    private void StartCrouchRoutine(bool isCrouching)
    {
        if (_crouchCoroutine != null)
            _monoBehaviour.StopCoroutine(_crouchCoroutine);

        _crouchCoroutine = _monoBehaviour.StartCoroutine(CrouchRoutine(isCrouching));
    }

    private IEnumerator CrouchRoutine(bool isCrouching)
    {
        float targetHeight = isCrouching ? _crouchingHeight : _standingHeight;
        Vector3 targetCenter = isCrouching ? _crouchingCenter : _standingCenter;

        while (!IsStateReached(targetHeight, targetCenter))
        {
            UpdateControllerParameters(targetHeight, targetCenter);
            yield return null;
        }

        FinalizeState(targetHeight, targetCenter);
    }

    private bool IsStateReached(float targetHeight, Vector3 targetCenter)
    {
        return Mathf.Abs(_controller.height - targetHeight) < 0.01f &&
               Vector3.Distance(_controller.center, targetCenter) < 0.01f;
    }

    private void UpdateControllerParameters(float targetHeight, Vector3 targetCenter)
    {
        _controller.height = Mathf.SmoothDamp(
            _controller.height,
            targetHeight,
            ref _heightVelocity,
            _smoothTime
        );

        _controller.center = Vector3.SmoothDamp(
            _controller.center,
            targetCenter,
            ref _centerVelocity,
            _smoothTime
        );
    }

    private void FinalizeState(float targetHeight, Vector3 targetCenter)
    {
        _controller.height = targetHeight;
        _controller.center = targetCenter;
        _crouchCoroutine = null;
    }
}