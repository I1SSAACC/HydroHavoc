using UnityEngine;
using Mirror;

public class WaterZone : NetworkBehaviour
{
    [SerializeField] private int damageAmount = 5;
    private float damageTimer = 0f;
    [SerializeField] private float damageInterval = 1f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealth playerHealth = other.GetComponent<PlayerHealth>();
            if (playerHealth != null && isServer)
                playerHealth.TakeDamage(damageAmount);
        }
    }

    private void Update()
    {
        Collider[] playersInZone = Physics.OverlapSphere(transform.position, 1f);
        foreach (var player in playersInZone)
        {
            if (player.CompareTag("Player"))
            {
                PlayerHealth playerHealth = player.GetComponent<PlayerHealth>();
                if (playerHealth != null && isServer)
                {
                    damageTimer += Time.deltaTime;
                    if (damageTimer >= damageInterval)
                        playerHealth.TakeDamage(damageAmount);
                        damageTimer = 0f;
                }
            }
        }
    }
}
