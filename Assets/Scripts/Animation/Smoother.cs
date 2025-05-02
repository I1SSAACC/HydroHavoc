using UnityEngine;

public class Smoother
{
    private readonly float _speed;
    private Vector2 _currentValues;
    private float _currentValue;

    public Smoother(float speed)
    {
        _speed = speed;
    }

    public float GetSmoothValue(float targetValue)
    {
        _currentValue = Mathf.MoveTowards(_currentValue, targetValue, _speed * Time.deltaTime);

        return _currentValue;
    }

    public Vector2 GetSmoothValue(Vector2 targetValues)
    {
        _currentValues = Vector2.MoveTowards(_currentValues, targetValues, _speed * Time.deltaTime);

        return _currentValues;
    }
}