using UnityEngine;
using Mirror;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private int _health = 100;

    [SerializeField] private TMP_Text _healthText;

    private void Start() =>
        UpdateHealthText();

    [Command]
    public void CmdTakeDamage(int amount) =>
        TakeDamage(amount);

    [Server]
    public void TakeDamage(int amount)
    {
        _health -= amount;
        if (_health < 0) _health = 0;

        RpcUpdateHealth(_health);
    }

    [ClientRpc]
    private void RpcUpdateHealth(int newHealth)
    {
        _health = newHealth;
        UpdateHealthText();
    }

    private void OnHealthChanged(int oldHealth, int newHealth) =>
        UpdateHealthText();

    private void UpdateHealthText()
    {
        if (_healthText != null)
            _healthText.text = "HP: " + _health;
    }
}
