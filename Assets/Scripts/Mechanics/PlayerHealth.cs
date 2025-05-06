using UnityEngine;
using Mirror;
using System;

public class PlayerHealth : NetworkBehaviour
{
    private const float MaxValue = 100;

    [SerializeField] private PlayerHealthView _view;

    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private float _health;

    public override void OnStartServer()
    {
        base.OnStartServer();
        _health = MaxValue;
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

    private void OnHealthChanged(float _, float newHealth) =>
        _view.UpdateValue(newHealth);
}