using UnityEngine;

public class Smoother
{
    private readonly float _maxDeltaPerSecond;
    private Vector2 _currentVector;

    public Smoother(float maxDeltaPerSecond)
    {
        _maxDeltaPerSecond = maxDeltaPerSecond;
    }

    public Vector2 GetSmoothedValue(Vector2 targetVector)
    {
        _currentVector = Vector2.MoveTowards(
            _currentVector, 
            targetVector, 
            _maxDeltaPerSecond * Time.deltaTime);

        return _currentVector;
    }
}