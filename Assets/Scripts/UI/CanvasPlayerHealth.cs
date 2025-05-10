using UnityEngine;
using UnityEngine.UI;

public class CanvasPlayerHealth : MonoBehaviour
{
    private const float MinValue = 0;
    private const float MaxValue = 1;

    [SerializeField] private Slider _slider;

    private void Awake()
    {
        _slider.minValue = MinValue;
        _slider.maxValue = MaxValue;
    }

    public void UpdateHealth(float health, float maxHealth)
    {
        float value = health / maxHealth;
        _slider.value = Mathf.Clamp(value, MinValue, MaxValue);
    }
}