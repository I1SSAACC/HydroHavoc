using TMPro;
using UnityEngine;

public class PlayerHealthView : MonoBehaviour
{
    private const string HP = nameof(HP);

    [SerializeField] private TMP_Text _text;

    public void UpdateValue(float value)
    {
        _text.text = $"{HP}: {Mathf.RoundToInt(value)}";
    }
}
