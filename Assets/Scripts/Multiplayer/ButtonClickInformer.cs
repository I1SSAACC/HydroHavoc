using System;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonClickInformer : MonoBehaviour
{
    [SerializeField] private Button _showButton;

    private Button _button;

    public event Action Clicked;

    private void Awake() =>
        _button = GetComponent<Button>();

    private void OnEnable() =>
        _button.onClick.AddListener(OnClick);

    private void OnDisable() =>
        _button.onClick.RemoveListener(OnClick);

    private void OnClick()
    {
        gameObject.SetActive(false);
        _showButton.gameObject.SetActive(true);        

        Clicked?.Invoke();
    }
}