using UnityEngine;

public class CameraIsHer : MonoBehaviour
{
    [SerializeField] private Transform _camera;

    public Transform Camera => _camera;
}