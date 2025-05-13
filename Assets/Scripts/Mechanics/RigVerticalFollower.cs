using UnityEngine;

public class RigVerticalFollower : MonoBehaviour
{
    private const float Speed = 10f;

    [SerializeField] private CameraHeightTarget _cameraTarget;

    private Transform _target;

    private void Awake() =>
        _target = _cameraTarget.transform;

    private void Update() =>
        MaintainHeight();

    private void MaintainHeight()
    {
        Vector3 tempPosition = transform.position;
        tempPosition.y = Mathf.Lerp(tempPosition.y, _target.position.y, Speed * Time.deltaTime);
        transform.position = tempPosition;
    }
}