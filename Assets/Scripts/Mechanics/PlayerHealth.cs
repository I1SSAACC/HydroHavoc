using UnityEngine;
using Mirror;
using TMPro;

public class PlayerHealth : NetworkBehaviour
{
    [SyncVar(hook = nameof(OnHealthChanged))]
    [SerializeField] private int health = 100;

    [SerializeField] private TMP_Text healthText;

    private void Start() =>
        UpdateHealthText();

    [Server]
    public void TakeDamage(int amount)
    {
        health -= amount;
        if (health < 0) health = 0;

        RpcUpdateHealth(health);
    }

    [ClientRpc]
    private void RpcUpdateHealth(int newHealth)
    {
        health = newHealth;
        UpdateHealthText();
    }

    private void OnHealthChanged(int oldHealth, int newHealth) =>
        UpdateHealthText();

    private void UpdateHealthText()
    {
        if (healthText != null)
            healthText.text = "HP: " + health;
    }
}
