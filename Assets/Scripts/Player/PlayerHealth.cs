using UnityEngine;
using Mirror;
using System;

public class PlayerHealth : NetworkBehaviour
{
    private const float MaxValue = 100;

    [SerializeField] private PlayerHealthView _view;

    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private float _health;

    private CanvasPlayerHealth _sliderHealth;

    public override void OnStartServer() =>
        SetMaxHealth();

    public override void OnStartClient() =>
        SetMaxHealth();

    private void SetMaxHealth()
    {
        _health = MaxValue * 0.75f;
        _sliderHealth = FindFirstObjectByType<CanvasPlayerHealth>();
    }

    [Command]
    public void CmdTakeDamage(float amount) =>
        TakeDamage(amount);

    [Server]
    public void TakeDamage(float amount)
    {
        if (amount < 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Значение не может быть отрицательным");

        _health = Mathf.Max(_health - amount, 0);
    }

    private void OnHealthChanged(float _, float newHealth)
    {
        //if (isLocalPlayer == false)
        //    return;

        _health = newHealth;
        if (_sliderHealth == null)
        {
            if (isLocalPlayer == false)
                return;
            
            _sliderHealth = FindFirstObjectByType<CanvasPlayerHealth>();
            Debug.Log("Косяк косячный");
        }

        _sliderHealth.UpdateHealth(newHealth, MaxValue);
        _view.UpdateValue(newHealth);
    }
}