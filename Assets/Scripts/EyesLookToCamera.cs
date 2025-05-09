using UnityEngine;

public class EyesLookToCamera : MonoBehaviour
{
    private Transform _target;

    private void LateUpdate()
    {
        if (_target == null)
            return;

        transform.rotation = _target.rotation;
    }

    public void Follow(Transform target) =>
        _target = target;
}