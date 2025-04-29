using TMPro;
using UnityEngine;

public class CanvasPlayerName : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;

    public void UpdateName(string text) =>
        _text.text = text;
}