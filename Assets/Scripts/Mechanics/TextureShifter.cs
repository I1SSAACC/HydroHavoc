using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class TextureShifter : MonoBehaviour
{
    [SerializeField] private float _speedX;
    [SerializeField] private float _speedY;

    private Material _material;
    private Vector2 _offset;

    private void Awake() =>
        _material = GetComponent<Renderer>().material;

    private void Update()
    {
        if (_material != null)
            Move();
    }

    private void Move()
    {
        if (_offset.x >= 1000 || _offset.y >= 1000)
            _offset = Vector2.zero;

        _offset.x += _speedX * Time.deltaTime;
        _offset.y += _speedY * Time.deltaTime;

        _material.mainTextureOffset = _offset;
    }
}