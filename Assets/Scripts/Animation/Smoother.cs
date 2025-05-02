using UnityEngine;

public class Smoother
{
    private float _speed;
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
}