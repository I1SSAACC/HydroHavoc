using UnityEngine;

public class PlayerCameraTarget : MonoBehaviour 
{
    [SerializeField] private CameraHeightTarget _target;

    private void Update()
    {
        transform.position = _target.transform.position;
    }
}