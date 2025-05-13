using UnityEngine;

public class EyesLookToCameraTarget : MonoBehaviour
{
    [SerializeField] private PlayerCameraTarget _target;

    private Transform _targetTransform;

    private void Awake() =>
        _targetTransform = _target.transform;

    private void LateUpdate() =>
        transform.rotation = _targetTransform.rotation;
}